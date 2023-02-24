using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace nf.protoscript.test
{

    /// <summary>
    /// a translator that translate projects into a simple Html5 web page.
    /// </summary>
    class SimpleHtml5PageTranslator
    {
        public void Translate(ProjectInfo InProjInfo)
        {
            // DataBinding tier 1: gather all databinding names
            foreach (Info info in InProjInfo.SubInfos)
            {
                // Gather data-binding properties.
                DataBindingFeature.GatherDataBindingNames(info);
            }
            // DataBinding tier 2: Mark properties that may dispatch OnPropertyChanged event when they are modified and updated.
            foreach (Info info in InProjInfo.SubInfos)
            {
                // Gather data-binding properties.
                DataBindingFeature.GatherDataSourceProperties(info);
            }

            // tier P1: Type parse.
            // Parse elements recursively to gather Types used by next translation stages.
            //         JSDecls: js member declaration and init in ctor
            //         JSFuncs: js functions
            // foreach non-attribute infos.
            InProjInfo.ForeachSubInfoExclude<Info, AttributeInfo>(info =>
            {
                InProjInfo.Extra.GlobalCodes = new List<string>();

                if (info is TypeInfo
                    && info.Header == "model"
                    )
                {
                    // Parse info as a type
                    _ParseType(info as TypeInfo);
                }
                else if (info is ElementInfo
                    && info.Header == "global"
                    )
                {
                    // Project's sub-object, take it as a global object.
                    var elemInfo = info as ElementInfo;
                    elemInfo.Extra.GlobalCodes = new List<string>();

                    if (elemInfo.InitSyntax != null)
                    {
                        IList<string> codes = JsInstruction.GenCodeForExpr(new JsFunction(InProjInfo), elemInfo.InitSyntax);
                        // TODO fix bad smell. codes[0], same as TestCppTranslator.cs
                        InProjInfo.Extra.GlobalCodes.Add($"{elemInfo.Name} = {codes[0]};");
                    }
                    else
                    {
                        InProjInfo.Extra.GlobalCodes.Add($"{elemInfo.Name} = null;");
                    }

                    // Special global applet: call it's main() to start the global applet
                    if (elemInfo.Header == "global" && elemInfo.Name == "applet")
                    {
                        InProjInfo.Extra.GlobalCodes.Add($"{elemInfo.Name}.main();");
                    }
                }
                else
                {
                    // Unrecognize infos in this level.
                    Console.WriteLine($"ERROR: Unrecognize info [{info.Header}:{info.Name}] in this level.");
                }
            });


            // tier T1: generate class codes
            //     Foreach type, generate class declarations.
            //         For all subs of the type, gather JSDecls and JSFuncs, and generate member/methods.
            List<string> classesCodes = new List<string>();
            // generate classes.
            foreach (Info info in InProjInfo.SubInfos)
            {
                // TypeInfo, generate class for it.
                if (info is TypeInfo)
                {
                    TypeInfo typeInfo = info as TypeInfo;
                    var typeCodes = _GenerateClassCodesForType(typeInfo);
                    classesCodes.AddRange(typeCodes);
                }
            }
            foreach (var ln in classesCodes)
            {
                System.Console.WriteLine(ln);
            }

            // tier T2: generate page codes

            // generate Pages and applet codes for the 'Editor'.
            {
                List<string> pageLns = new List<string>();
                pageLns.Add($"<!DOCTYPE html>");
                pageLns.Add($"<html>");
                pageLns.Add($"    <head>");
                pageLns.Add($"        <meta charset=\"utf-8\">");
                pageLns.Add($"        <title>A simplest Html Page</title>");
                pageLns.Add($"        <script src = \"https://cdn.staticfile.org/jquery/1.10.2/jquery.min.js\"></script>");
                pageLns.Add($"        <script src = \"../../../GearsetDataBindingSample.js\"></script>");
                pageLns.Add($"        <script src = \"../../../GearsetContentObjectSample.js\"></script>");
                pageLns.Add($"        <script>");

                // class defines
                foreach (var ln in classesCodes)
                {
                    pageLns.Add("            " + ln);
                }

                // globals and startups.
                pageLns.Add("            $(document).ready(function(){");
                foreach (var ln in InProjInfo.Extra.GlobalCodes)
                {
                    pageLns.Add("                " + ln);
                }
                pageLns.Add("            });");

                pageLns.Add($"        </script>");
                pageLns.Add($"    </head>");
                pageLns.Add($"    <body>");
                pageLns.Add($"        <div id=\"EditorViewRoot\">");
                pageLns.Add($"            <div id=\"BackgroundRoot\">");
                pageLns.Add($"            </div>");
                pageLns.Add($"            <div id=\"UIRoot\">");
                pageLns.Add($"            </div>");
                pageLns.Add($"            <div id=\"FloatingRoot\">");
                pageLns.Add($"            </div>");
                pageLns.Add($"        </div>");
                pageLns.Add("    </body>");
                pageLns.Add("</html>");


                // Write to console
                Console.WriteLine(">>>> WEB PAGE <<<<");
                foreach (var str in pageLns)
                {
                    Console.WriteLine(str);
                }

                // Write to file
                const string GTestHtmlFileName = "index.html";
                File.WriteAllLines(GTestHtmlFileName, pageLns);

                // Windows: use shell exec to open the "index.html"
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var procStartInfo = new ProcessStartInfo("cmd", $"/c start {GTestHtmlFileName}") { CreateNoWindow = true };
                    Process.Start(procStartInfo);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", GTestHtmlFileName);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", GTestHtmlFileName);
                }

            }
        }

        private void _ParseElement(TypeInfo InTypeScope, ElementInfo InInfo)
        {
            // Generate code for members.
            if (InInfo.Header == "property")
            {
                InInfo.Extra.JSDecls = new List<string>();
                string initCode = "null";
                if (InInfo.InitSyntax != null)
                {
                    IList<string> codes = JsInstruction.GenCodeForExpr(new JsFunction(InTypeScope), InInfo.InitSyntax);
                    // TODO fix bad smell. codes[0], same as TestCppTranslator.cs
                    initCode = codes[0];
                }

                // Filter element by DataBinding feature.
                if (DataBindingFeature.IsDataSourceProperty(InInfo))
                {
                    // Generate DataSource codes for the class.
                    if (!InTypeScope.Extra.DSGenerated)
                    {
                        InInfo.Extra.JSDecls.Add($"this.DSComp = new DataSourceComponent();");
                        InTypeScope.Extra.DSGenerated = true;
                    }

                    // internal member used by getter/setter
                    InInfo.Extra.JSDecls.Add($"this._{InInfo.Name} = {initCode};");

                    // getter/setter
                    InInfo.Extra.JSFuncs = new JsFunction[]
                    {
                        new JsFunction(InTypeScope)
                        {
                            Name = $"get {InInfo.Name}",
                            Params = new string[]{ "" },
                            BodyLines = new string[]
                            {
                                $"return this._{InInfo.Name};"
                            }
                        },
                        new JsFunction(InTypeScope)
                        {
                            Name = $"set {InInfo.Name}",
                            Params = new string[]{ "val" },
                            BodyLines = new string[]
                            {
                                $"this._{InInfo.Name} = val;",
                                $"this.DSComp.Trigger(\"{InInfo.Name}\");"
                            }
                        },
                    };

                    // Special member-setter codes
                    InInfo.Extra.MemberGetExprCode = $"$OWNER{InInfo.Name}";
                    InInfo.Extra.MemberSetExprCode = $"$OWNER{InInfo.Name} = ($RHS)";
                    InInfo.Extra.MemberRefExprCode = $"$OWNER{InInfo.Name}";
                }
                else
                {
                    // use member directly.
                    InInfo.Extra.JSDecls.Add($"this.{InInfo.Name} = null;");

                    InInfo.Extra.MemberGetExprCode = $"$OWNER{InInfo.Name}";
                    InInfo.Extra.MemberSetExprCode = $"$OWNER{InInfo.Name} = ($RHS)";
                    InInfo.Extra.MemberRefExprCode = $"$OWNER{InInfo.Name}";
                }
            }
            else if (InInfo.Header == "ovr-property")
            {
                InInfo.Extra.JSDecls = new List<string>();
                string parentName = (InInfo.ParentInfo is TypeInfo) ? "this" : InInfo.ParentInfo.Name;
                if (InInfo.InitSyntax == null)
                {
                    // TODO Find if it is an object, which can be overrided by object-template.
                    string objTemplClassName = InInfo.ElementType.Name;
                    InInfo.Extra.JSDecls.Add($"{parentName}.{InInfo.Name} = new {objTemplClassName}();");
                }
                else
                {
                    // TODO which context should fit?
                    IList<string> codes = JsInstruction.GenCodeForExpr(new JsFunction(InTypeScope), InInfo.InitSyntax);
                    // TODO fix bad smell. codes[0], same as TestCppTranslator.cs
                    InInfo.Extra.JSDecls.Add($"{parentName}.{InInfo.Name} = {codes[0]};");
                }
            }
            else if (InInfo.Header == "method"
                || InInfo.Header == "command"
                )
            {
                var jsFunc = new JsFunction(InInfo.ParentInfo);
                jsFunc.Name = InInfo.Name;

                // gather parameters
                List<string> paramNames = new List<string>();
                InInfo.ForeachSubInfoExclude<ElementInfo, AttributeInfo>(mtdSub =>
                {
                    if (mtdSub.Header != "param")
                    { return; }

                    paramNames.Add(mtdSub.Name);

                    // TODO return handler
                }
                );
                jsFunc.Params = paramNames.ToArray();
 
                // translate sequences.
                STNodeSequence exprSeq = InInfo.InitSyntax as STNodeSequence;
                List<string> codeLns = new List<string>();
                foreach (var stNode in exprSeq.NodeList)
                {
                    IList<string> codeList = JsInstruction.GenCodeForExpr(jsFunc, stNode);
                    foreach (string code in codeList)
                    {
                        codeLns.Add(code + ";");
                    }
                }
                jsFunc.BodyLines = codeLns.ToArray();

                // register methods
                InInfo.Extra.JSFuncs = new JsFunction[] { jsFunc };
            }
            else if (InInfo.Header == "ui")
            {
                string parentName = (InInfo.ParentInfo is TypeInfo) ? "this.UIRoot" : InInfo.ParentInfo.Name;

                // handle UI sub-elements.
                _GenerateElementCode(InInfo, parentName);
            }

            // TODO states, others.


            // foreach sub and generate codes for them.
            InInfo.ForeachSubInfo<ElementInfo>(sub => _ParseElement(InTypeScope, sub));
        }

        /// <summary>
        /// Parse members/methods and fill Extras.
        /// </summary>
        /// <param name="InTypeInfo"></param>
        private void _ParseType(TypeInfo InTypeInfo)
        {
            InTypeInfo.Extra.ClassName = InTypeInfo.Name;

            if (InTypeInfo.BaseType != null)
            { InTypeInfo.Extra.ClassBase = _GetTypeClassName(InTypeInfo.BaseType); }

            InTypeInfo.Extra.DSGenerated = false;
            InTypeInfo.ForeachSubInfoExclude<ElementInfo, AttributeInfo>(elemInfo => _ParseElement(InTypeInfo, elemInfo));
        }

        /// <summary>
        /// Generate class codes.
        /// </summary>
        /// <param name="InTypeInfo"></param>
        private string[] _GenerateClassCodesForType(TypeInfo InTypeInfo)
        {
            List<string> resultLns = new List<string>();

            bool hasBase = false;
            if (InTypeInfo.IsExtraContains("ClassBase"))
            {
                resultLns.Add($"class {InTypeInfo.Extra.ClassName} extends {InTypeInfo.Extra.ClassBase} {{");
                hasBase = true;
            }
            else
            {
                resultLns.Add($"class {InTypeInfo.Extra.ClassName} {{");
            }

            // Constructor and member decl
            resultLns.Add($"    constructor() {{");
            if (hasBase)
            {
                resultLns.Add($"        super();");

            }

            // Generate member declaration codes.
            void _GenerateMemberDecls(List<string> RefCodes, ElementInfo InElementInfo, int InIndent)
            {
                string indentStr = "";
                for (int i = 0; i < InIndent; i++)
                { indentStr += "    "; }

                RefCodes.Add(indentStr + "{");

                if (InElementInfo.IsExtraContains("JSDecls"))
                {
                    IEnumerable<string> decls = InElementInfo.Extra.JSDecls;
                    foreach (string decl in decls)
                    {
                        // write indent
                        RefCodes.Add(indentStr + "    " + decl);
                    }
                }
                // recursive
                InElementInfo.ForeachSubInfo<ElementInfo>(subSub => _GenerateMemberDecls(RefCodes, subSub, InIndent + 1));

                RefCodes.Add(indentStr + "}");
            }

            // Gather decls from sub infos.
            InTypeInfo.ForeachSubInfo<ElementInfo>(elemInfo => _GenerateMemberDecls(resultLns, elemInfo, 2));

            resultLns.Add($"    }}");

            // Methods
            foreach (Info subInfo in InTypeInfo.SubInfos)
            {
                if (!subInfo.IsExtraContains("JSFuncs"))
                { continue; }

                IEnumerable<JsFunction> funcs = subInfo.Extra.JSFuncs;
                foreach (JsFunction func in funcs)
                {
                    resultLns.Add("    " + func.FuncDeclCode + " {");
                    foreach (string bodyLn in func.BodyLines)
                    {
                        resultLns.Add("        " + bodyLn);
                    }
                    resultLns.Add("    }");
                }
            }

            resultLns.Add($"}}");

            return resultLns.ToArray();
        }


        /// <summary>
        /// Generate codes for editor's model
        /// </summary>
        /// <param name="InInfo"></param>
        private void _GenerateElementCode(ElementInfo InInfo, string InParentElemName)
        {
            if (!_IsUIElement(InInfo))
            { return; }

            InInfo.Extra.JSDecls = new List<string>();

            // parent which hold this element.
            string parentName = InParentElemName;

            // this element's name.
            string elemName = InInfo.Name;

            // header: determines class of the element
            string elemClassName = _GetTypeClassName(InInfo.ElementType);

            // ## let code line
            InInfo.Extra.JSDecls.Add($"let {elemName} = new {elemClassName}({parentName});");

            // named element: register to it's parent's element dictionary.
            // TODO a better way to describe nameless Infos.
            if (!elemName.StartsWith("Anonymous_"))
            {
                InInfo.Extra.JSDecls.Add($"{parentName}.{elemName} = {elemName};");
            }

            // ## data bindings
            List<string> dbCodes = new List<string>();
            InInfo.ForeachSubInfoByHeader<AttributeInfo>("db", attr =>
            {
                var stdb = attr.InitSyntaxTree as syntaxtree.STNodeDataBinding;
                if (stdb == null)
                {
                    throw new InvalidProgramException();
                }
                dynamic jsdbs = _ConvertDBSettingsToJS(stdb.Settings);
                dbCodes.Add($"{{");
                dbCodes.Add($"    let dbSettings = DataBindingSettings.New(\"{jsdbs.srcType}\", \"{jsdbs.srcName}\",");
                dbCodes.Add($"        \"{jsdbs.srcPath}\", ");
                dbCodes.Add($"        \"{jsdbs.tarType}\", \"{jsdbs.tarName}\",");
                dbCodes.Add($"        \"{jsdbs.tarPath}\"");
                dbCodes.Add($"    );");
                dbCodes.Add($"    {elemName}.dataBindings.push(DynamicDataBinding.New({elemName}, dbSettings));");
                dbCodes.Add($"}}");
            });
            InInfo.Extra.JSDecls.Add($"// begin data bindings of {elemName}.");
            foreach (var dbCode in dbCodes)
            {
                InInfo.Extra.JSDecls.Add($"{dbCode}");
            }
            InInfo.Extra.JSDecls.Add($"// ~ end data bindings of {elemName}.");

        }

        private static dynamic _ConvertDBSettingsToJS(DataBindingSettings InSettings)
        {
            dynamic jsdbs = new ExpandoObject();
            jsdbs.srcType = _ConvertDBObjTypeToJS(InSettings.SourceObjectType);
            jsdbs.srcName = InSettings.SourceObjectName;
            jsdbs.srcPath = InSettings.SourcePath.ToString();
            jsdbs.tarType = _ConvertDBObjTypeToJS(InSettings.TargetObjectType);
            jsdbs.tarName = InSettings.TargetObjectName;
            jsdbs.tarPath = InSettings.TargetPath.ToString();
            return jsdbs;
        }

        private static string _ConvertDBObjTypeToJS(EDataBindingObjectType InObjType)
        {
            switch (InObjType)
            {
                case EDataBindingObjectType.This: return "this";
                case EDataBindingObjectType.DataContext: return "dataContext";
                case EDataBindingObjectType.StaticGlobal: return "global";
                case EDataBindingObjectType.Resource: return "resource";
                case EDataBindingObjectType.Ancestor: return "ancestor";
            }
            return "";
        }

        private static bool _IsUIElement(ElementInfo InInfo)
        {
            if (InInfo.ElementType == null
                || InInfo.ElementType.ParentInfo != SystemTypePackageInfo.Instance)
            {
                return false;
            }

            var uiCtrlNames = new string[]
            {
                "panel",
                "label",
                "button",
                "checkbox",
                "radiobox",
                "selector",
                "listview",
                "tabview",
                "treeview",
            };
            if (-1 == Array.IndexOf(uiCtrlNames, InInfo.ElementType.Name))
            {
                return false;
            }
            return true;
        }

        private static string _GetTypeClassName(TypeInfo InType)
        {
            if (InType.ParentInfo == SystemTypePackageInfo.Instance)
            {
                switch (InType.Name)
                {
                    case "panel": return "Panel";
                    case "label": return "Label";
                    case "button": return "Button";
                    case "checkbox": return "CheckBox";
                    case "radiobox": return "RadioBox";
                    case "selector": return "Selector";
                    case "listview": return "ListView";
                    case "tabview": return "TabView";
                    case "treeview": return "TreeView";
                    case "editor": return "Editor";
                    case "applet": return "Applet";
                }
                return "$ERROR_UICtrl_ClassName";
            }

            string elemClassName = InType.Name;
            return elemClassName;
        }


    }



}
