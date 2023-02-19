using nf.protoscript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                //      
                Info editorInfo = InProjInfo.FindTheFirstSubInfo<Info>(i => i.Header == "editor" && !(i is TypeInfo));
                string editorName = editorInfo.Name;
                string editorClassName = editorInfo.IsExtraContains("InlineEditorTypeName") ? editorInfo.Extra.InlineEditorTypeName : "Editor";
                pageLns.Add($"                var {editorName} = new {editorClassName}();");
                {
                    // editor models
                    string[] editorModelCodeLns = _GenerateEditorModel(editorInfo, $"{editorName}.UIRoot");
                    foreach (var ln in editorModelCodeLns)
                    {
                        pageLns.Add("                " + ln);
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
                        m.Extra.JSDecls.Add($"this.gsDataSourceComponent = new DataSourceComponent();");
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
                                $"this.gsDataSourceComponent.Trigger(\"{m.Name}\");"
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

                // TODO generate databinding registeration
                //out string dbPathName = "";
                //if (DataBindingFeature.IsDataBindingTarget(m, out dbPathName))
                //{
                //    string regCode = $"GearsetDataBindingManager.inst().RegDataListener(\"{dbPathName}, {m.Name}\")";
                //}

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
        /// <param name="InEditorInfo"></param>
        /// <returns></returns>
        private string[] _GenerateEditorModel(Info InEditorInfo, string InParentElemName)
        {
            List<string> resultStringLns = new List<string>();
            _GenerateElementModel(InEditorInfo, resultStringLns, InParentElemName);

            return resultStringLns.ToArray();
        }

        private void _GenerateElementModel(Info InInfo, List<string> RefResultStrings, string InParentElemName)
        {
            // recursive
            InInfo.ForeachSubInfo<Info>(elem =>
            {
                // ignore attributes.
                if (elem is AttributeInfo)
                { return; }

                // parent which hold this element.
                string parentName = InParentElemName;

                // this element's name.
                string elemName = elem.Name;

                // header: determines class of the element
                string elemClassName = "$ERROR_UICtrl_ClassName";
                switch (elem.Header)
                {
                    case "panel": elemClassName = "Panel"; break;
                    case "label": elemClassName = "Label"; break;
                }

                // let code line
                RefResultStrings.Add($"let {elemName} = new {elemClassName}({parentName});");
                RefResultStrings.Add("{");

                // named element: register to it's parent's element dictionary.
                // TODO a better way to describe nameless Infos.
                if (!elemName.StartsWith("Anonymous_"))
                {
                    RefResultStrings.Add($"    {parentName}.{elemName} = {elemName};");
                }

                // TODO attributes


                // TODO data bindings


                // Handle child code-gen and indents.
                List<string> subResultStrs = new List<string>();
                _GenerateElementModel(elem, subResultStrs, elemName);
                foreach (var subStr in subResultStrs)
                {
                    RefResultStrings.Add($"    {subStr}");
                }

                RefResultStrings.Add("}");

            }
            );
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
