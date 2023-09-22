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


            // Default host object scheme for methods
            var exprTrans = new ExprTranslatorDefault();
            {
                exprTrans.DefaultHostAccessScheme = new STNodeTranslateSchemeDefault()
                {
                    Present = new STNodeTranslateSnippet(
                        new ElementConstString("TToRef(")
                        , new ElementReplaceSubNodeValue("HOSTOBJ")
                        , new ElementConstString(").")
                        ),
                };

                exprTrans.DefaultVarGetScheme = new STNodeTranslateSchemeDefault()
                {
                    //Present "%{VarName}%",
                    Present = new STNodeTranslateSnippet(
                                new ElementReplaceSubNodeValue("HOST")
                                , new ElementVarName()
                            ),
                }
                ;
                exprTrans.DefaultVarRefScheme = new STNodeTranslateSchemeDefault()
                {
                    //Present "%{VarName}%",
                    Present = new STNodeTranslateSnippet(
                                new ElementReplaceSubNodeValue("HOST")
                                , new ElementVarName()
                            ),
                }
                ;
                exprTrans.DefaultVarSetScheme = new STNodeTranslateSchemeDefault()
                {
                    //Present: "%{VarName}%",
                    Present = new STNodeTranslateSnippet(
                                new ElementReplaceSubNodeValue("HOST")
                                , new ElementVarName()
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
                exprTrans.DefaultBinOpScheme = new STNodeTranslateSchemeDefault()
                {
                    // Present: "%{LHS}% %{OpCode}% %{RHS}%"
                    Present = new STNodeTranslateSnippet(
                        new ElementReplaceSubNodeValue("LHS")
                        , new ElementConstString(" ")
                        , new ElementReplaceSubNodeValue("OpCode")
                        , new ElementConstString(" ")
                        , new ElementReplaceSubNodeValue("RHS")
                    )
                };
            }


            //TestCases.BasicLanguage().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
            //TestCases.BasicDataBinding().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
            _GenerateMethodsForType(exprTrans, TestCases.BasicExpressions());
            _GenerateMethodsForType(exprTrans, TestCases.BinOpExpressions());
            TestCases.AdvancedExpressions().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
        }

        private static void _GenerateMethodsForType(ExprTranslatorDefault InTranslator, TypeInfo InTargetType)
        {
            Console.WriteLine($"Code emit sequences for Type: {InTargetType.Name}");
            // ctor
            {
                Console.WriteLine($"    Code emit sequences for Ctor:");

                var context = new ExprTranslateContextDefault(InTargetType
                    , new ExprTranslateContextDefault.Scope[]
                    {
                        new ExprTranslateContextDefault.Scope(InTargetType, "this", "this->")
                        , new ExprTranslateContextDefault.Scope(InTargetType.ParentInfo, "global", "::")
                    }
                    );

                InTargetType.ForeachSubInfoByHeader<ElementInfo>("property", memberInfo =>
                {
                    if (memberInfo.InitSyntax != null)
                    {
                        var codes = InTranslator.Translate(context, memberInfo.InitSyntax);
                        foreach (var code in codes)
                        {
                            // TODO "this->member = %{RHS}%" should comes from MemberInitScheme
                            Console.WriteLine($"        this->{memberInfo.Name} = " + code);
                        }
                    }
                });

            }

            // Methods
            InTargetType.ForeachSubInfoByHeader<ElementInfo>("method", mtdInfo =>
            {
                Console.WriteLine($"    Code emit sequences for {mtdInfo.Name}:");

                var context = new ExprTranslateContextDefault(mtdInfo
                    , new ExprTranslateContextDefault.Scope[]
                    {
                        new ExprTranslateContextDefault.Scope(mtdInfo, "local", "")
                        , new ExprTranslateContextDefault.Scope(InTargetType, "this", "this->")
                        , new ExprTranslateContextDefault.Scope(InTargetType.ParentInfo, "global", "::")
                    }
                    );
                var codes = InTranslator.Translate(context, mtdInfo.InitSyntax);
                foreach (var code in codes)
                {
                    Console.WriteLine("        " + code);
                }
            });
        }
    }
}
