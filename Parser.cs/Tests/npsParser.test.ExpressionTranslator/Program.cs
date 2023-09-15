using System;
using System.Diagnostics;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using nf.protoscript.test;
using nf.protoscript.translator.expression;
using nf.protoscript.translator.expression.DefaultSnippetElements;

namespace npsParser.test.ExpressionTranslator
{

    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            Console.WriteLine("Which translator do you want to test?");

            var exprTrans = new ExprTranslatorDefault();
            {
                exprTrans.DefaultVarGetScheme = new STNodeTranslateSchemeDefault()
                {
                    //Present "%{VarName}%",
                    Present = new STNodeTranslateSnippet(
                                new ElementVarName()
                            ),
                }
                ;
                exprTrans.DefaultVarRefScheme = new STNodeTranslateSchemeDefault()
                {
                    //Present "%{VarName}%",
                    Present = new STNodeTranslateSnippet(
                                new ElementVarName()
                            ),
                }
                ;
                exprTrans.DefaultVarSetScheme = new STNodeTranslateSchemeDefault()
                {
                    //Present: "%{VarName}%",
                    Present = new STNodeTranslateSnippet(
                                new ElementVarName()
                                , new ElementConstString(" = ")
                                , new ElementReplaceSubNodeValue("RHS")
                            ),
                }
                ;
                exprTrans.DefaultConstScheme = new STNodeTranslateSchemeDefault()
                {
                    //Present: "%{ValueString}%)",
                    Present = new STNodeTranslateSnippet(
                                new ElementConstNodeValueString()
                            ),
                }
                ;
            }

            TypeInfo testExprs = TestCases.BasicExpressions();
            testExprs.ForeachSubInfoByHeader<ElementInfo>("method", mtdInfo =>
                {
                    Console.WriteLine($"Code emit sequences for {mtdInfo.Name}:");

                    var context = new ExprTranslateContextDefault(mtdInfo, "self");
                    var codes = exprTrans.Translate(context, mtdInfo.InitSyntax);
                    foreach (var code in codes)
                    {
                        Console.WriteLine("    " + code);
                    }
                    //{
                    //    var generator = new ExprCodeGen_Log();
                    //    generator.GenFunctionCodes(mtdInfo.Name, mtdInfo, mtdInfo.InitSyntax);

                    //    foreach (string codeLn in generator.Results)
                    //    {
                    //        Console.WriteLine("    " + codeLn);
                    //    }
                    //}

                    //// domain based generator
                    //{
                    //    var generator = new ExprCodeGenerator_DomainBased();

                    //    // Register domain code templates
                    //    {
                    //        // Default Domain Template:
                    //        //      %PRE
                    //        //      %PRESENT%
                    //        //      %POST%
                    //        DomainBasedCodeTemplate defaultTemplate = new DomainBasedCodeTemplate();
                    //        defaultTemplate.AddSnippet(new SnippetDomain("Pre"));
                    //        defaultTemplate.AddSnippet(new SnippetPresent());
                    //        defaultTemplate.AddSnippet(new SnippetDomain("Post"));
                    //        generator.SetDefaultCodeTemplate(defaultTemplate);

                    //    }

                    //    generator.GenFunctionCodes(mtdInfo.Name, mtdInfo, mtdInfo.InitSyntax);

                    //}


                });

        }
    }
}
