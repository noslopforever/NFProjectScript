﻿using nf.protoscript;
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

            List<string> classesCodes = new List<string>();
            {
                // Parse members/methods and fill Extras.
                InProjInfo.ForeachSubInfo<TypeInfo>(typeInfo =>
                {
                    _ParseType(typeInfo);
                });
                // generate classes.
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
            }

            // Parse globals
            List<string> globalCodes = new List<string>();
            {
                InProjInfo.ForeachSubInfo<ElementInfo>(elemInfo =>
                {
                    if (elemInfo.InitSyntax != null)
                    {
                        IList<string> codes = JsInstruction.GenCodeForExpr(new JsFunction(InProjInfo), elemInfo.InitSyntax);
                        // TODO fix bad smell. codes[0], same as TestCppTranslator.cs
                        globalCodes.Add($"{elemInfo.Name} = {codes[0]};");
                    }
                    else
                    {
                        globalCodes.Add($"{elemInfo.Name} = null;");
                    }

                    // Special global applet: call it's main() to start the global applet
                    if (elemInfo.Header == "global" && elemInfo.Name == "applet")
                    {
                        globalCodes.Add($"{elemInfo.Name}.main();");
                    }
                });

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

                // globals and startups.
                pageLns.Add("            $(document).ready(function(){");
                foreach (var ln in globalCodes)
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

        /// <summary>
        /// Parse members/methods and fill Extras.
        /// </summary>
        /// <param name="InTypeInfo"></param>
        private void _ParseType(TypeInfo InTypeInfo)
        {
            InTypeInfo.Extra.ClassName = InTypeInfo.Name;

            AttributeInfo baseAttr = InTypeInfo.FindTheFirstSubInfoWithHeader<AttributeInfo>("base");
            if (baseAttr != null)
            {
                var baseValConst = baseAttr.InitSyntaxTree as syntaxtree.STNodeConstant;
                if (baseValConst != null)
                {
                    var baseType = baseValConst.Value as TypeInfo;
                    if (baseType != null)
                    {
                        InTypeInfo.Extra.ClassBase = _GetTypeClassName(baseType);
                    }
                }
            }

            // Generate code for members.
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




            //// override properties
            //// TODO bad smell, merge it with the same call in _GenerateElementCode.
            //{
            //    List<string> memberSetCodes = new List<string>();
            //    _GenerateOverrideProperties(memberSetCodes, editorInfo, editorName);
            //    foreach (var ln in memberSetCodes)
            //    {
            //        pageLns.Add($"                {ln}");
            //    }
            //}

            // generate code for elements
            InTypeInfo.ForeachSubInfo<ElementInfo>(elemInfo =>
            {
                // editor controls
                List<string> elemLns = new List<string>();
                _GenerateElementCode(elemInfo, elemLns, $"this.UIRoot");
                elemInfo.Extra.JSDecls = elemLns;
            }
            );

            // TODO methods, states, others.
        }

        /// <summary>
        /// Generate class codes.
        /// </summary>
        /// <param name="InTypeInfo"></param>
        private string[] _GenerateClassForType(TypeInfo InTypeInfo)
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

            // Gather decls from sub infos.
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
        private void _GenerateElementCode(ElementInfo InInfo, List<string> RefResultStrings, string InParentElemName)
        {
            if (!_IsUIElement(InInfo))
            { return; }

            // parent which hold this element.
            string parentName = InParentElemName;

            // this element's name.
            string elemName = InInfo.Name;

            // header: determines class of the element
            string elemClassName = _GetTypeClassName(InInfo.ElementType);

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
            InInfo.ForeachSubInfo<ElementInfo>(subInfo =>
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
