﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using nf.protoscript.test;
using nf.protoscript.translator;
using nf.protoscript.translator.expression;
using nf.protoscript.translator.expression.DefaultSnippetElements;
using nf.protoscript.translator.expression.SchemeSelectors;

namespace npsParser.test.ExpressionTranslator
{

    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            Console.WriteLine("Which translator do you want to test?");


            var exprTrans = new ExprTranslatorDefault();

            // Shared and generic schemes.
            {

                // Default HostPresent Call
                exprTrans.AddScheme("HostPresent"
                    // %{HostPresent}% in the context.
                    , new STNodeTranslateSnippet(
                        new ElementNodeValue("HostPresent")
                    )
                );

                // HostPresent for member call (HOST.MEMBER)
                exprTrans.AddSchemeSelector(
                    "HostPresent"
                    , new STNodeTranslateSchemeSelector_Lambda(
                        "HostPresent"
                        , 0
                        , ctx =>
                        {
                            return ctx is ExprTranslatorAbstract.MemberContext;
                        }
                        // TRef(%{HOST}%).
                        , new STNodeTranslateSchemeDefault(
                            new STNodeTranslateSnippet(
                                new ElementConstString("TRef(")
                                , new ElementNodeValue("HOST")
                                , new ElementConstString(").")
                            )
                        )
                    )
                );

                exprTrans.AddScheme("Getter"
                    , new STNodeTranslateSnippet(
                        new ElementConstString("get")
                        , new ElementNodeValue("VarName")
                        , new ElementConstString("()")
                    )
                );

                exprTrans.AddScheme("Setter"
                    , new STNodeTranslateSnippet(
                        new ElementConstString("set")
                        , new ElementNodeValue("VarName")
                        , new ElementConstString("(")
                        , new ElementNodeValue("RHS")
                        , new ElementConstString(")")
                    )
                );

                exprTrans.AddScheme("InitTempVarBySelf"
                    , new STNodeTranslateSnippet(
                        new ElementConstString("auto ")
                        , new ElementTempVar("Var")
                        , new ElementConstString(" = ")
                        , new ElementCallOther(ExprTranslatorAbstract.SystemScheme_VarGet)
                    )
                );

            }

            // Default host object scheme for methods
            #region Default Schemes

            {
                exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_Null
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "null",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementConstString("null")
                        )
                    }
                );
                exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_Const
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{ValueString}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementNodeValue("ValueString")
                        )
                    }
                );
                exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarInit
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{Host}%%{VarName}% = %{RHS}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementConstString("// Init ")
                            , new ElementNodeValue("VarName")
                            , new ElementNewLine()
                            , new ElementCallOther("HostPresent")
                            , new ElementNodeValue("VarName")
                            , new ElementConstString(" = ")
                            , new ElementNodeValue("RHS")
                        )
                    }
                );
                exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarGet
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{Host}%%{VarName}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementCallOther("HostPresent")
                            , new ElementNodeValue("VarName")
                        )
                    }
                );
                exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarRef
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{Host}%%{VarName}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementCallOther("HostPresent")
                            , new ElementNodeValue("VarName")
                        )
                    }
                );
                exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarSet
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{Host}%%{VarName}% = %{RHS}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementCallOther("HostPresent")
                            , new ElementNodeValue("VarName")
                            , new ElementConstString(" = ")
                            , new ElementNodeValue("RHS")
                        )
                    }
                );
                exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_BinOp
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        // Present: "%{LHS}% %{OpCode}% %{RHS}%"
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementNodeValue("LHS")
                            , new ElementConstString(" ")
                            , new ElementNodeValue("OpCode")
                            , new ElementConstString(" ")
                            , new ElementNodeValue("RHS")
                        )
                    }
                );
                //exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_BinOp
                //    , new Dictionary<string, STNodeTranslateSnippet>()
                //    {
                //        ["PreStatement"] = new STNodeTranslateSnippet(
                //            new ElementConstString("auto ")
                //            , new ElementTempVar("LHS")
                //            , new ElementConstString(" = ")
                //            , new ElementNodeValue("LHS")
                //            , new ElementNewLine()
                //            , new ElementConstString("auto ")
                //            , new ElementTempVar("RHS")
                //            , new ElementConstString(" = ")
                //            , new ElementNodeValue("RHS")
                //        )
                //        ,
                //        // Present: "%{LHS}% %{OpCode}% %{RHS}%"
                //        ["Present"] = new STNodeTranslateSnippet(
                //            new ElementTempVar("LHS")
                //            , new ElementConstString(" ")
                //            , new ElementNodeValue("OpCode")
                //            , new ElementConstString(" ")
                //            , new ElementTempVar("RHS")
                //        )
                //    }
                //);

            }

            #endregion
            // ~ Default Schemes


            // Scheme Selectors
            #region Special Scheme Selectors

            {
                #region Special Member Access for SetterProperty

                // Special Member Access for RO Property 
                // TODO disable SET/REF
                {
                    Func<ITranslatingContext, bool> condROProperty = ctx =>
                    {
                        var varCtx = ctx as ExprTranslatorAbstract.IVariableContext;
                        if (varCtx == null)
                        {
                            return false;
                        }
                        var readonlyAttr = varCtx.BoundElementInfo.FindTheFirstSubInfoWithName<AttributeInfo>("readonly");
                        if (null != readonlyAttr)
                        {
                            return true;
                        }
                        return false;
                    };

                    exprTrans.AddSchemeSelector(
                        ExprTranslatorAbstract.SystemScheme_VarGet
                        , new STNodeTranslateSchemeSelector_Lambda(
                            "ROProperty_GET"
                            , 0
                            , condROProperty
                            , new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
                            {
                                ["Present"] = new STNodeTranslateSnippet(
                                    new ElementCallOther("HostPresent")
                                    , new ElementCallOther("Getter")
                                    )
                            }
                            //, new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
                            //{
                            //    ["PreStatement"] = new STNodeTranslateSnippet(
                            //        new ElementConstString("auto ")
                            //        , new ElementTempVar("Var")
                            //        , new ElementConstString(" = ")
                            //        , new ElementCallOther("HostPresent")
                            //        , new ElementCallOther("Getter")
                            //    )
                            //    ,
                            //    ["Present"] = new STNodeTranslateSnippet(
                            //        new ElementTempVar("Var")
                            //    )
                            //}
                            )
                        )
                    );
                }

                // Special Member Access for SetterProperty 
                {
                    Func<ITranslatingContext, bool> condSetterProperty = ctx =>
                    {
                        var varCtx = ctx as ExprTranslatorAbstract.IVariableContext;
                        if (varCtx == null)
                        {
                            return false;
                        }
                        var setterAttr = varCtx.BoundElementInfo.FindTheFirstSubInfoWithName<AttributeInfo>("setter");
                        if (null != setterAttr)
                        {
                            return true;
                        }
                        return false;
                    };

                    // Special SET process for a setter property
                    exprTrans.AddSchemeSelector(
                        ExprTranslatorAbstract.SystemScheme_VarSet
                        , new STNodeTranslateSchemeSelector_Lambda(
                            "SetterProperty_SET"
                            , 0
                            , condSetterProperty
                            , new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
                            {
                                ["Present"] = new STNodeTranslateSnippet(
                                    new ElementCallOther("HostPresent")
                                    , new ElementCallOther("Setter")
                                    )
                            }
                            )
                        )
                    );

                    // Handle the 'set-back' pattern in REF process for a setter property.
                    exprTrans.AddSchemeSelector(
                        ExprTranslatorAbstract.SystemScheme_VarRef
                        , new STNodeTranslateSchemeSelector_Lambda(
                            "SetterProperty_REF"
                            , 0
                            , condSetterProperty
                            , new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
                            {
                                ["PreStatement"] = new STNodeTranslateSnippet(
                                    new ElementConstString("// PreStatement for SetterProperty_Ref ")
                                    , new ElementNodeValue("VarName")
                                    , new ElementNewLine()
                                    , new ElementCallOther("InitTempVarBySelf")
                                    )
                                ,
                                ["Present"] = new STNodeTranslateSnippet(
                                    new ElementTempVar("Var")
                                    )
                                ,
                                // // PostStatement for SetterProperty_Ref: %{VarName}%
                                // %{$$SYS_VAR_SET}|RHS=%{$TempVar}%%
                                ["PostStatementRev"] = new STNodeTranslateSnippet(
                                    new ElementConstString("// PostStatement for SetterProperty_Ref ")
                                    , new ElementNodeValue("VarName")
                                    , new ElementNewLine()
                                    , new ElementCallOther(ExprTranslatorAbstract.SystemScheme_VarSet
                                        , new Dictionary<string, STNodeTranslateSnippet>()
                                        {
                                            ["RHS"] = new STNodeTranslateSnippet(
                                                new ElementTempVar("Var")
                                                )
                                        }
                                        )
                                    )
                            }
                            )
                        )
                    );
                }
                // ~ Special Member Access for SetterProperty
                #endregion
            }

            #endregion
            // ~ Scheme Selectors

            //TestCases.BasicLanguage().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
            //TestCases.BasicDataBinding().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
            TestCases.BasicExprs().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
            TestCases.AdvancedExpressions().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
        }

        private static void _GenerateMethodsForType(ExprTranslatorDefault InTranslator, nf.protoscript.TypeInfo InTargetType)
        {
            Console.WriteLine($"Code emit sequences for Type: {InTargetType.Name}");
            // ctor
            {
                Console.WriteLine($"    Code emit sequences for Ctor:");

                var ctorEnv = new ExprTranslateEnvironmentDefault(InTargetType
                    , new ExprTranslateEnvironmentDefault.Scope[]
                    {
                        new ExprTranslateEnvironmentDefault.Scope(InTargetType, "this", "this->")
                        , new ExprTranslateEnvironmentDefault.Scope(InTargetType.ParentInfo, "global", "::")
                    }
                );
                IMethodBodyContext ctorCtx = new VirtualMethodBodyContext(null, InTargetType, "ctor", ctorEnv);

                // Gather all property-init expressions and convert them to STNodeInits.
                List<ISyntaxTreeNode> initSyntaxes = new List<ISyntaxTreeNode>();
                InTargetType.ForeachSubInfoByHeader<ElementInfo>("member", memberInfo =>
                {
                    if (memberInfo.InitSyntax != null)
                    {
                        initSyntaxes.Add(new STNodeMemberInit(memberInfo, memberInfo.InitSyntax));
                    }
                });
                // Translate STNodeInits.
                STNodeSequence seq = new STNodeSequence(initSyntaxes.ToArray());
                var codes = InTranslator.Translate(ctorCtx, seq);
                foreach (var code in codes)
                {
                    Console.WriteLine($"        " + code);
                }

            }

            // Methods
            InTargetType.ForeachSubInfoByHeader<ElementInfo>("method", mtdInfo =>
            {
                Console.WriteLine($"    Code emit sequences for {mtdInfo.Name}:");

                var mtdEnv = new ExprTranslateEnvironmentDefault(mtdInfo
                    , new ExprTranslateEnvironmentDefault.Scope[]
                    {
                        new ExprTranslateEnvironmentDefault.Scope(mtdInfo, "local", "")
                        , new ExprTranslateEnvironmentDefault.Scope(InTargetType, "this", "this->")
                        , new ExprTranslateEnvironmentDefault.Scope(InTargetType.ParentInfo, "global", "::")
                    }
                    );
                MethodBodyContext funcBodyCtx = new MethodBodyContext(null, mtdInfo, mtdEnv);
                var codes = InTranslator.Translate(funcBodyCtx, mtdInfo.InitSyntax);
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