using System;
using System.Collections.Generic;
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
                exprTrans.DefaultInitTempVarScheme = new STNodeTranslateSchemeDefault(
                    new STNodeTranslateSnippet(
                        new ElementConstString("auto ")
                        , new ElementReplaceSubNodeValue("TEMPVARNAME")
                        , new ElementConstString(" = ")
                        , new ElementReplaceSubNodeValue("TEMPVARVALUE")
                        )
                    );
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
                //exprTrans.DefaultBinOpScheme = new STNodeTranslateSchemeDefault()
                //{
                //    // Present: "%{LHS}% %{OpCode}% %{RHS}%"
                //    Present = new STNodeTranslateSnippet(
                //        new ElementReplaceSubNodeValue("LHS")
                //        , new ElementConstString(" ")
                //        , new ElementReplaceSubNodeValue("OpCode")
                //        , new ElementConstString(" ")
                //        , new ElementReplaceSubNodeValue("RHS")
                //    )
                //};
                exprTrans.DefaultBinOpScheme = new STNodeTranslateSchemeDefault(
                    // Present: "%{LHS}% %{OpCode}% %{RHS}%"
                    new STNodeTranslateSnippet(
                        new ElementTempVar("LHS")
                        , new ElementConstString(" ")
                        , new ElementReplaceSubNodeValue("OpCode")
                        , new ElementConstString(" ")
                        , new ElementTempVar("RHS")
                    )
                    //, new Dictionary<string, STNodeTranslateSnippet>()
                    //{
                    //    ["PreStatement"] = new STNodeTranslateSnippet(
                    //        new ElementNewTempVar("LHS")
                    //        , new ElementNewTempVar("RHS")
                    //        )
                    //}
                );
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



    static class TestSetBackErrors
    {
        static void foo(ref int P0, ref int P1)
        {
            P0++;
            P1++;
        }

        class A
        {
            public int _p;
            public int p { get { return _p; } set { _p = value; } }
            public ref int refp { get { return ref _p; } }
        }

        static void main()
        {
            int procID = Process.GetCurrentProcess().Id;

            {
                A a = new A();
                foo(ref a.refp, ref a.refp);
                Console.WriteLine(a.p); // 2, success
            }
            {
                //A a = new A();
                //foo(ref a.p, ref a.p);
                //Console.WriteLine(a.p);
            }
            {
                A a = new A();
                int t0 = a.p;
                int t1 = a.p;
                foo(ref t0, ref t1);
                a.p = t0;
                a.p = t1;
                Console.WriteLine($"ap {a.p}, t0 {t0}, t1 {t1}"); // 1 1 1, fail (of course)
            }

        }

    }

}