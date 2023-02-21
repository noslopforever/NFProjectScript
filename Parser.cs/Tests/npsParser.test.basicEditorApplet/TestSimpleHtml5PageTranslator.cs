using nf.protoscript;
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

            // Parse members/methods and fill Extras.
            InProjInfo.ForeachSubInfo<TypeInfo>(typeInfo =>
            {
                _ParseMembersAndMethods(typeInfo);
            });

            // generate classes.
            List<string> classesCodes = new List<string>();
            foreach (Info info in InProjInfo.SubInfos)
            {
                // TypeInfo, generate class for it.
                if (info is TypeInfo)
                {
                    TypeInfo typeInfo = info as TypeInfo;
                    var typeCodes = _GenerateClassForType(typeInfo);
                    classesCodes.AddRange(typeCodes);
                }
            }

            foreach (var ln in classesCodes)
            {
                System.Console.WriteLine(ln);
            }


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

                pageLns.Add("            $(document).ready(function(){");
                // editor conception:
                Info editorInfo = InProjInfo.FindTheFirstSubInfo<Info>(i => i.Header == "editor" && !(i is TypeInfo));
                string editorName = editorInfo.Name;
                string editorClassName = editorInfo.IsExtraContains("InlineEditorTypeName") ? editorInfo.Extra.InlineEditorTypeName : "Editor";

                // new editor instance
                pageLns.Add($"                {editorName} = new {editorClassName}();");

                // override properties
                // TODO bad smell, merge it with the same call in _GenerateElementCode.
                {
                    List<string> memberSetCodes = new List<string>();
                    _GenerateOverrideProperties(memberSetCodes, editorInfo, editorName);
                    foreach (var ln in memberSetCodes)
                    {
                        pageLns.Add($"                {ln}");
                    }
                }
                // generate elements
                {
                    List<string> elemCodes = new List<string>();
                    // editor controls
                    editorInfo.ForeachSubInfo<Info>(editorSub => {
                        _GenerateElementCode(editorSub, elemCodes, $"{editorName}.UIRoot");
                    });
                    foreach (var ln in elemCodes)
                    {
                        pageLns.Add($"                {ln}");
                    }

                }
                pageLns.Add($"                $(\"#UIRoot\")[0].appendChild({editorName}.UIRoot.createElements());");

                pageLns.Add("            });");


                pageLns.Add($"        </script>");
                pageLns.Add($"    </head>");
                pageLns.Add($"    <body>");

                string[] editorViewCodeLns = _GenerateEditorView(editorInfo);
                foreach (var str in editorViewCodeLns)
                {
                    pageLns.Add("        " + str);
                }

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

        /// <summary>
        /// Parse members/methods and fill Extras.
        /// </summary>
        /// <param name="InTypeInfo"></param>
        private void _ParseMembersAndMethods(TypeInfo InTypeInfo)
        {
            InTypeInfo.Extra.ClassName = InTypeInfo.Name;

            bool genDSForClass = false;
            InTypeInfo.ForeachSubInfo<MemberInfo>(m =>
            {
                m.Extra.JSDecls = new List<string>();
                if (m.InitSyntax != null)
                {
                    IList<string> codes = JsInstruction.GenCodeForExpr(new JsFunction(InTypeInfo), m.InitSyntax);
                    // TODO fix bad smell. codes[0], same as TestCppTranslator.cs
                    m.Extra.JSDecls.Add($"this.{m.Name} = {codes[0]};");
                }
                else
                {
                    m.Extra.JSDecls.Add($"this.{m.Name} = null;");
                }


                if (DataBindingFeature.IsDataSourceProperty(m))
                {
                    // Generate DataSource codes for the class.
                    if (!genDSForClass)
                    {
                        m.Extra.JSDecls.Add($"this.DSComp = new DataSourceComponent();");
                    }
                    genDSForClass = true;

                    // Special member-setter codes
                    m.Extra.MemberGetExprCode = $"{m.Name}";
                    m.Extra.MemberSetExprCode = $"set{m.Name}($RHS)";
                    m.Extra.MemberRefExprCode = $"{m.Name}";

                    m.Extra.JSFuncs = new JsFunction[]
                    {
                        new JsFunction(InTypeInfo)
                        {
                            Name = $"set{m.Name}",
                            Params = new string[]{ "val" },
                            BodyLines = new string[]
                            {
                                $"this.{m.Name} = val;",
                                $"this.DSComp.Trigger(\"{m.Name}\");"
                            }
                        },
                    };
                }
                else
                {
                    m.Extra.MemberGetExprCode = $"{m.Name}";
                    m.Extra.MemberSetExprCode = $"{m.Name} = ($RHS)";
                    m.Extra.MemberRefExprCode = $"{m.Name}";
                }

            });


            // TODO methods, states, others.

        }

        /// <summary>
        /// Generate class codes.
        /// </summary>
        /// <param name="InTypeInfo"></param>
        private string[] _GenerateClassForType(TypeInfo InTypeInfo)
        {
            List<string> resultLns = new List<string>();

            resultLns.Add($"class {InTypeInfo.Extra.ClassName} {{");

            // Constructor and member decl
            resultLns.Add($"    constructor() {{");

            foreach (Info subInfo in InTypeInfo.SubInfos)
            {
                if (!subInfo.IsExtraContains("JSDecls"))
                { continue; }

                IEnumerable<string> decls = subInfo.Extra.JSDecls;
                foreach (string decl in decls)
                {
                    resultLns.Add("        " + decl);
                }
            }

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
        private void _GenerateElementCode(Info InInfo, List<string> RefResultStrings, string InParentElemName)
        {
            if (!_IsUIElement(InInfo))
            { return; }

            // parent which hold this element.
            string parentName = InParentElemName;

            // this element's name.
            string elemName = InInfo.Name;

            // header: determines class of the element
            string elemClassName = "$ERROR_UICtrl_ClassName";
            elemClassName = _GetElementClassNameFromHeader(InInfo, elemClassName);

            // ## let code line
            RefResultStrings.Add($"let {elemName} = new {elemClassName}({parentName});");
            RefResultStrings.Add("{");

            // named element: register to it's parent's element dictionary.
            // TODO a better way to describe nameless Infos.
            if (!elemName.StartsWith("Anonymous_"))
            {
                RefResultStrings.Add($"    {parentName}.{elemName} = {elemName};");
            }


            // ## Gather all override properties
            List<string> memberSetCodes = new List<string>();
            _GenerateOverrideProperties(memberSetCodes, InInfo, elemName);
            foreach (var ln in memberSetCodes)
            {
                RefResultStrings.Add($"    {parentName}.{elemName} = {elemName};");
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
            RefResultStrings.Add($"    // begin data bindings of {elemName}.");
            foreach (var dbCode in dbCodes)
            {
                RefResultStrings.Add($"    {dbCode}");
            }
            RefResultStrings.Add($"    // ~ end data bindings of {elemName}.");


            // ## Handle child code-gen and indents.
            List<string> subCodes = new List<string>();
            InInfo.ForeachSubInfo<Info>(subInfo =>
            {
                _GenerateElementCode(subInfo, subCodes, elemName);
            });
            foreach (var subCode in subCodes)
            {
                RefResultStrings.Add($"    {subCode}");
            }

            RefResultStrings.Add("}");
        }

        private static void _GenerateOverrideProperties(List<string> RefCodes, Info InInfo, string elemName)
        {
            InInfo.ForeachSubInfo<MemberInfo>(member =>
            {
                if (member.InitSyntax == null)
                {
                    // TODO Find if it is an object, which can be overrided by object-template.
                    string objTemplClassName = member.Archetype.Name;
                    RefCodes.Add($"{elemName}.{member.Name} = new {objTemplClassName}();");
                }
                else
                {
                    // TODO which context should fit?
                    IList<string> codes = JsInstruction.GenCodeForExpr(new JsFunction(InInfo), member.InitSyntax);
                    // TODO fix bad smell. codes[0], same as TestCppTranslator.cs
                    RefCodes.Add($"{elemName}.{member.Name} = {codes[0]};");
                }
            });
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

        private static bool _IsUIElement(Info InInfo)
        {
            // ignore attributes.
            if (InInfo is AttributeInfo
                || InInfo is MemberInfo
                || InInfo is MethodInfo
                )
            { return false; }

            var uiHeaders = new string[]
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
            if (-1 == Array.IndexOf(uiHeaders, InInfo.Header))
            {
                return false;
            }
            return true;
        }

        private static string _GetElementClassNameFromHeader(Info elem, string elemClassName)
        {
            switch (elem.Header)
            {
                case "panel": elemClassName = "Panel"; break;
                case "label": elemClassName = "Label"; break;
                case "button": elemClassName = "Button"; break;
                case "checkbox": elemClassName = "CheckBox"; break;
                case "radiobox": elemClassName = "RadioBox"; break;
                case "selector": elemClassName = "Selector"; break;
                case "listview": elemClassName = "ListView"; break;
                case "tabview": elemClassName = "TabView"; break;
                case "treeview": elemClassName = "TreeView"; break;
            }

            return elemClassName;
        }

        /// <summary>
        /// Generate codes for editor's view-model
        /// </summary>
        /// <param name="InEditorInfo"></param>
        private string[] _GenerateEditorView(Info InEditorInfo)
        {
            List<string> resultStringLns = new List<string>();

            {
                resultStringLns.Add("<div id=\"EditorViewRoot\">");

                {
                    resultStringLns.Add("    <div id=\"BackgroundRoot\">");

                    // end BackgroundRoot
                    resultStringLns.Add($"    </div>");
                }

                {
                    resultStringLns.Add("    <div id=\"UIRoot\">");

                    // end UIRoot
                    resultStringLns.Add($"    </div>");
                }

                {
                    resultStringLns.Add("    <div id=\"FloatingRoot\">");
                    // end FloatingRoot
                    resultStringLns.Add($"    </div>");
                }

                // end EditorViewRoot
                resultStringLns.Add($"</div>");
            }

            return resultStringLns.ToArray();
        }


    }



}
