using nf.protoscript;
using nf.protoscript.translator;
using nf.protoscript.translator.expression;
using nf.protoscript.translator.expression.DefaultSnippetElements;
using nf.protoscript.translator.expression.SchemeSelectors;
using System;
using System.Collections.Generic;

namespace npsParser.test.ClassTranslator
{
    internal class TestExprTranslatorCpp
        : ExprTranslatorDefault
    {
        public TestExprTranslatorCpp()
        {
            // Shared and generic schemes.
            {

                // Default HostPresent Call
                AddScheme("HostPresent"
                    // %{HostPresent}% in the context.
                    , new STNodeTranslateSnippet(
                        new ElementNodeValue("HostPresent")
                    )
                );

                // HostPresent for member call (HOST.MEMBER)
                AddSchemeSelector(
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

                AddScheme("Getter"
                    , new STNodeTranslateSnippet(
                        new ElementConstString("get")
                        , new ElementNodeValue("VarName")
                        , new ElementConstString("()")
                    )
                );

                AddScheme("Setter"
                    , new STNodeTranslateSnippet(
                        new ElementConstString("set")
                        , new ElementNodeValue("VarName")
                        , new ElementConstString("(")
                        , new ElementNodeValue("RHS")
                        , new ElementConstString(")")
                    )
                );

                AddScheme("InitTempVarBySelf"
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
                AddScheme(ExprTranslatorAbstract.SystemScheme_Null
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "null",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementConstString("null")
                        )
                    }
                );
                AddScheme(ExprTranslatorAbstract.SystemScheme_Const
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{ValueString}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementNodeValue("ValueString")
                        )
                    }
                );
                AddScheme(ExprTranslatorAbstract.SystemScheme_VarGet
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{Host}%%{VarName}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementCallOther("HostPresent")
                            , new ElementNodeValue("VarName")
                        )
                    }
                );
                AddScheme(ExprTranslatorAbstract.SystemScheme_VarRef
                    , new Dictionary<string, STNodeTranslateSnippet>()
                    {
                        //Present "%{Host}%%{VarName}%",
                        ["Present"] = new STNodeTranslateSnippet(
                            new ElementCallOther("HostPresent")
                            , new ElementNodeValue("VarName")
                        )
                    }
                );
                AddScheme(ExprTranslatorAbstract.SystemScheme_VarSet
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
                AddScheme(ExprTranslatorAbstract.SystemScheme_BinOp
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
                //AddScheme(ExprTranslatorAbstract.SystemScheme_BinOp
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

                    AddSchemeSelector(
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
                    AddSchemeSelector(
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
                    AddSchemeSelector(
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

        }
    }
}