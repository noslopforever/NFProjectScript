using nf.protoscript;
using System;
using System.Collections.Generic;
using System.IO;

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
            foreach (Info info in InProjInfo.SubInfos)
            {
                // TypeInfo, generate class for it.
                if (info is TypeInfo)
                {
                    TypeInfo typeInfo = info as TypeInfo;
                    _GenerateClassForType(typeInfo);
                }
            }

            // generate Pages and applet codes
            {
                List<string> pageLns = new List<string>();
                pageLns.Add("<!DOCTYPE html>");
                pageLns.Add("<html>");
                pageLns.Add("    <head>");
                pageLns.Add("        <meta charset=\"utf-8\">");
                pageLns.Add("        <title>A simplest Html Page</title>");
                pageLns.Add("    </head>");
                pageLns.Add("    <body>");

                Info editorInfo = InProjInfo.FindTheFirstSubInfo<Info>(i => i.Header == "editor" && !(i is TypeInfo));
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
                File.WriteAllLines("index.html", pageLns);
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
                        m.Extra.JSDecls.Add($"this.gsDataSourceComponent = GearsetDataSourceComponent.New(this);");
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
                                $"this.gsDataSourceComponent.triggerPropertyChanged(\"{m.Name}\", val);"
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
        private void _GenerateClassForType(TypeInfo InTypeInfo)
        {
            StringWriter sw = new StringWriter();

            sw.WriteLine($"class {InTypeInfo.Extra.ClassName} {{");

            // Constructor and member decl
            sw.WriteLine($"    constructor {{");

            foreach (Info subInfo in InTypeInfo.SubInfos)
            {
                if (!subInfo.IsExtraContains("JSDecls"))
                { continue; }

                IEnumerable<string> decls = subInfo.Extra.JSDecls;
                foreach (string decl in decls)
                {
                    sw.WriteLine("        " + decl);
                }
            }

            sw.WriteLine($"    }}");

            // Methods
            foreach (Info subInfo in InTypeInfo.SubInfos)
            {
                if (!subInfo.IsExtraContains("JSFuncs"))
                { continue; }

                IEnumerable<JsFunction> funcs = subInfo.Extra.JSFuncs;
                foreach (JsFunction func in funcs)
                {
                    sw.WriteLine("    " + func.FuncDeclCode + " {");
                    foreach (string bodyLn in func.BodyLines)
                    {
                        sw.WriteLine("        " + bodyLn);
                    }
                    sw.WriteLine("    }");
                }
            }

            sw.WriteLine($"}}");

            System.Console.Write(sw.ToString());
        }


        /// <summary>
        /// Generate codes for editor's view-model
        /// </summary>
        /// <param name="editorInfo"></param>
        private string[] _GenerateEditorView(Info editorInfo)
        {
            List<string> resultStringLns = new List<string>();

            List<Info> floatingElements = new List<Info>();

            editorInfo.ForeachSubInfo<Info>(elem =>
                {
                    // ignore attributes.
                    if (elem is AttributeInfo)
                    { return; }

                    if (elem.Header == "uipanel")
                    {
                        floatingElements.Add(elem);
                    }
                }
                );

            {
                resultStringLns.Add("<div name=\"EditorViewRoot\">");

                {
                    resultStringLns.Add("<div name=\"BackgroundRoot\">");

                    // end BackgroundRoot
                    resultStringLns.Add($"</div>");
                }

                {
                    resultStringLns.Add("<div name=\"UIRoot\">");

                    // end UIRoot
                    resultStringLns.Add($"</div>");
                }

                {
                    resultStringLns.Add("<div name=\"FloatingRoot\">");
                    foreach (var elem in floatingElements)
                    {
                        string[] codelns = _GenerateViewElement(elem);
                        foreach (var codeln in codelns)
                        {
                            resultStringLns.Add("    " + codeln);
                        }
                    }
                    // end FloatingRoot
                    resultStringLns.Add($"</div>");
                }

                // end EditorViewRoot
                resultStringLns.Add($"</div>");
            }

            return resultStringLns.ToArray();
        }

        private string[] _GenerateViewElement(Info InInfo)
        {
            List<string> codelns = new List<string>();
            if (InInfo.Header == "uipanel")
            {
                // TODO datacontext and databinding
                codelns.Add($"<div name=\"{InInfo.Name}\">");
                // Add sub codes.
                InInfo.ForeachSubInfo<Info>(elem =>
                {
                    // ignore attributes.
                    if (elem is AttributeInfo)
                    { return; }

                    var subCodes = _GenerateViewElement(elem);
                    foreach (var subCode in subCodes)
                    {
                        codelns.Add("    " + subCode);
                    }
                });
                codelns.Add($"</div>");
            }
            else if (InInfo.Header == "label")
            {
                // TODO databinding
                codelns.Add($"<div name=\"{InInfo.Name}\">%Default_Label_Text%</div>");
                // labels have no sub
            }


            return codelns.ToArray();
        }

    }



}
