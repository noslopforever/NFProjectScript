using nf.protoscript.syntaxtree;
using nf.protoscript.translator.DefaultScheme;
using nf.protoscript.translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nf.protoscript;

namespace npsParser.test.ExpressionTranslator
{
    partial class Program
    {
        private static InfoTranslatorAbstract _CreateHardcodeTranslator()
        {
            var translator = new InfoTranslatorDefault();

            // Constant: ${ValueString}
            translator.AddExprScheme<STNodeConstant>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${ValueString}"))
                );

            // Constant<string>: "${ValueString}"
            translator.AddExprSelector<STNodeConstant>("Get", 1
                , expr => { return expr.ValueType == CommonTypeInfos.String || expr.ValueType == CommonTypeInfos.TypeRef; }
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("\"${ValueString}\""))
                );

            // BinaryOp: ${LHS.Get()} ${OpCode} ${RHS.Get()}
            translator.AddExprScheme<STNodeBinaryOp>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${LHS.Get()} ${OpCode} ${RHS.Get()}"))
                );

            // UnaryOp: ${OpCode} ${RHS.Get()}
            translator.AddExprScheme<STNodeUnaryOp>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${OpCode}${RHS.Get()}"))
                );

            // Assign: ${LHS.Set(RHS.Get())}
            translator.AddExprScheme<STNodeAssign>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${LHS.Set(RHS.Get())}"))
                );

            // Default HostPresent (VAR_NAME): ${VAR_NAME}
            translator.AddScheme("HostPresent"
                , new InfoTranslateSchemeDefault(new string[] { "VAR_NAME" }, ElementParser.ParseElements("${VAR_NAME}"))
                );
            // TODO Global and class member hosts (:: and this->)
            //// HostPresent (VAR_NAME): ::${ParentType.Name}.${VAR_NAME}
            //AddSelector("HostPresent"
            //    , new nf.protoscript.SchemeSelectors.TranslateSchemeSelector_Lambda(
            //        0
            //        //, ctx => IsGlobal(ctx.Parent)
            //        , ctx => true
            //        , new InfoTranslateSchemeDefault
            //        (
            //            new ElementConstString("::")
            //            , ElementExpr.TEST_CreateTestExpr(
            //                new STNodeMemberAccess(new STNodeVar("ParentType"), "Name")
            //                )
            //            , new ElementConstString(".")
            //            , ElementExpr.TEST_CreateTestExpr(new STNodeVar("VAR_NAME"))
            //        )
            //    )
            //    );

            // Var Get: ${HostPresent(IDName)}
            translator.AddExprScheme<STNodeVar>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${HostPresent(IDName)}"))
                );
            // Var Set (RHS): ${HostPresent(IDName)} = $RHS
            translator.AddExprScheme<STNodeVar>("Set", 1
                , new InfoTranslateSchemeDefault(new string[1] { "RHS" }, ElementParser.ParseElements("${HostPresent(IDName)} = ${RHS}"))
                );
            // Member Get: ${LHS.Get()}.${IDName}
            translator.AddExprScheme<STNodeMemberAccess>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${LHS.Get()}.${IDName}"))
                );
            // Member Set (RHS): ${LHS.Get()}.${IDName} = ${RHS}
            translator.AddExprScheme<STNodeMemberAccess>("Set", 1
                , new InfoTranslateSchemeDefault(new string[1] { "RHS" }, ElementParser.ParseElements("${LHS.Get()}.${IDName} = ${RHS}"))
                );

            // Call: ${FuncExpr.Get()}(${For("Param", ", ", "Get")}
            translator.AddExprScheme<STNodeCall>("Get", 1
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${FuncExpr.Get()}(${For("Param", ", ", "Get")})
                        """
                        )
                    )
                );

            // Collection access: ${CollExpr.Get()}[${$For("Param", "][", "Get")}]
            translator.AddExprScheme<STNodeCollectionAccess>("Get", 1
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${CollExpr.Get()}[${For("Param", "][", "Get")}]
                        """
                        )
                    )
                );

            // Sequence: 
            translator.AddExprScheme<STNodeSequence>("Get", 1
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${For("", $NL, "Get")}
                        """
                        )
                    )
                );
            return translator;
        }

        //var exprTrans = new ExprTranslatorDefault();,

        //// Shared and generic schemes.
        //{

        //    // Default HostPresent Call
        //    exprTrans.AddScheme("HostPresent"
        //        // %{HostPresent}% in the context.
        //        , new STNodeTranslateSnippet(
        //            new ElementNodeValue("HostPresent")
        //        )
        //    );

        //    // HostPresent for member call (HOST.MEMBER)
        //    exprTrans.AddSchemeSelector(
        //        "HostPresent"
        //        , new STNodeTranslateSchemeSelector_Lambda(
        //            "HostPresent"
        //            , 0
        //            , ctx =>
        //            {
        //                return ctx is ExprTranslatorAbstract.MemberContext;
        //            }
        //            // TRef(%{HOST}%).
        //            , new STNodeTranslateSchemeDefault(
        //                new STNodeTranslateSnippet(
        //                    new ElementConstString("TRef(")
        //                    , new ElementNodeValue("HOST")
        //                    , new ElementConstString(").")
        //                )
        //            )
        //        )
        //    );

        //    exprTrans.AddScheme("Getter"
        //        , new STNodeTranslateSnippet(
        //            new ElementConstString("get")
        //            , new ElementNodeValue("VarName")
        //            , new ElementConstString("()")
        //        )
        //    );

        //    exprTrans.AddScheme("Setter"
        //        , new STNodeTranslateSnippet(
        //            new ElementConstString("set")
        //            , new ElementNodeValue("VarName")
        //            , new ElementConstString("(")
        //            , new ElementNodeValue("RHS")
        //            , new ElementConstString(")")
        //        )
        //    );

        //    exprTrans.AddScheme("InitTempVarBySelf"
        //        , new STNodeTranslateSnippet(
        //            new ElementConstString("auto ")
        //            , new ElementTempVar("Var")
        //            , new ElementConstString(" = ")
        //            , new ElementCallOther(ExprTranslatorAbstract.SystemScheme_VarGet)
        //        )
        //    );

        //}

        //// Default host object scheme for methods
        //#region Default Schemes

        //{
        //    exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_Null
        //        , new Dictionary<string, STNodeTranslateSnippet>()
        //        {
        //            //Present "null",
        //            ["Present"] = new STNodeTranslateSnippet(
        //                new ElementConstString("null")
        //            )
        //        }
        //    );
        //    exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_Const
        //        , new Dictionary<string, STNodeTranslateSnippet>()
        //        {
        //            //Present "%{ValueString}%",
        //            ["Present"] = new STNodeTranslateSnippet(
        //                new ElementNodeValue("ValueString")
        //            )
        //        }
        //    );
        //    exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarInit
        //        , new Dictionary<string, STNodeTranslateSnippet>()
        //        {
        //            //Present "%{Host}%%{VarName}% = %{RHS}%",
        //            ["Present"] = new STNodeTranslateSnippet(
        //                new ElementConstString("// Init ")
        //                , new ElementNodeValue("VarName")
        //                , new ElementNewLine()
        //                , new ElementCallOther("HostPresent")
        //                , new ElementNodeValue("VarName")
        //                , new ElementConstString(" = ")
        //                , new ElementNodeValue("RHS")
        //            )
        //        }
        //    );
        //    exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarGet
        //        , new Dictionary<string, STNodeTranslateSnippet>()
        //        {
        //            //Present "%{Host}%%{VarName}%",
        //            ["Present"] = new STNodeTranslateSnippet(
        //                new ElementCallOther("HostPresent")
        //                , new ElementNodeValue("VarName")
        //            )
        //        }
        //    );
        //    exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarRef
        //        , new Dictionary<string, STNodeTranslateSnippet>()
        //        {
        //            //Present "%{Host}%%{VarName}%",
        //            ["Present"] = new STNodeTranslateSnippet(
        //                new ElementCallOther("HostPresent")
        //                , new ElementNodeValue("VarName")
        //            )
        //        }
        //    );
        //    exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_VarSet
        //        , new Dictionary<string, STNodeTranslateSnippet>()
        //        {
        //            //Present "%{Host}%%{VarName}% = %{RHS}%",
        //            ["Present"] = new STNodeTranslateSnippet(
        //                new ElementCallOther("HostPresent")
        //                , new ElementNodeValue("VarName")
        //                , new ElementConstString(" = ")
        //                , new ElementNodeValue("RHS")
        //            )
        //        }
        //    );
        //    exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_BinOp
        //        , new Dictionary<string, STNodeTranslateSnippet>()
        //        {
        //            // Present: "%{LHS}% %{OpCode}% %{RHS}%"
        //            ["Present"] = new STNodeTranslateSnippet(
        //                new ElementNodeValue("LHS")
        //                , new ElementConstString(" ")
        //                , new ElementNodeValue("OpCode")
        //                , new ElementConstString(" ")
        //                , new ElementNodeValue("RHS")
        //            )
        //        }
        //    );
        //    //exprTrans.AddScheme(ExprTranslatorAbstract.SystemScheme_BinOp
        //    //    , new Dictionary<string, STNodeTranslateSnippet>()
        //    //    {
        //    //        ["PreStatement"] = new STNodeTranslateSnippet(
        //    //            new ElementConstString("auto ")
        //    //            , new ElementTempVar("LHS")
        //    //            , new ElementConstString(" = ")
        //    //            , new ElementNodeValue("LHS")
        //    //            , new ElementNewLine()
        //    //            , new ElementConstString("auto ")
        //    //            , new ElementTempVar("RHS")
        //    //            , new ElementConstString(" = ")
        //    //            , new ElementNodeValue("RHS")
        //    //        )
        //    //        ,
        //    //        // Present: "%{LHS}% %{OpCode}% %{RHS}%"
        //    //        ["Present"] = new STNodeTranslateSnippet(
        //    //            new ElementTempVar("LHS")
        //    //            , new ElementConstString(" ")
        //    //            , new ElementNodeValue("OpCode")
        //    //            , new ElementConstString(" ")
        //    //            , new ElementTempVar("RHS")
        //    //        )
        //    //    }
        //    //);

        //}

        //#endregion
        //// ~ Default Schemes


        //// Scheme Selectors
        //#region Special Scheme Selectors

        //{
        //    #region Special Member Access for SetterProperty

        //    // Special Member Access for RO Property 
        //    // TODO disable SET/REF
        //    {
        //        Func<ITranslatingContext, bool> condROProperty = ctx =>
        //        {
        //            var varCtx = ctx as ExprTranslatorAbstract.IVariableContext;
        //            if (varCtx == null)
        //            {
        //                return false;
        //            }
        //            var readonlyAttr = varCtx.BoundElementInfo.FindTheFirstSubInfoWithName<AttributeInfo>("readonly");
        //            if (null != readonlyAttr)
        //            {
        //                return true;
        //            }
        //            return false;
        //        };

        //        exprTrans.AddSchemeSelector(
        //            ExprTranslatorAbstract.SystemScheme_VarGet
        //            , new STNodeTranslateSchemeSelector_Lambda(
        //                "ROProperty_GET"
        //                , 0
        //                , condROProperty
        //                , new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
        //                {
        //                    ["Present"] = new STNodeTranslateSnippet(
        //                        new ElementCallOther("HostPresent")
        //                        , new ElementCallOther("Getter")
        //                        )
        //                }
        //                //, new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
        //                //{
        //                //    ["PreStatement"] = new STNodeTranslateSnippet(
        //                //        new ElementConstString("auto ")
        //                //        , new ElementTempVar("Var")
        //                //        , new ElementConstString(" = ")
        //                //        , new ElementCallOther("HostPresent")
        //                //        , new ElementCallOther("Getter")
        //                //    )
        //                //    ,
        //                //    ["Present"] = new STNodeTranslateSnippet(
        //                //        new ElementTempVar("Var")
        //                //    )
        //                //}
        //                )
        //            )
        //        );
        //    }

        //    // Special Member Access for SetterProperty 
        //    {
        //        Func<ITranslatingContext, bool> condSetterProperty = ctx =>
        //        {
        //            var varCtx = ctx as ExprTranslatorAbstract.IVariableContext;
        //            if (varCtx == null)
        //            {
        //                return false;
        //            }
        //            var setterAttr = varCtx.BoundElementInfo.FindTheFirstSubInfoWithName<AttributeInfo>("setter");
        //            if (null != setterAttr)
        //            {
        //                return true;
        //            }
        //            return false;
        //        };

        //        // Special SET process for a setter property
        //        exprTrans.AddSchemeSelector(
        //            ExprTranslatorAbstract.SystemScheme_VarSet
        //            , new STNodeTranslateSchemeSelector_Lambda(
        //                "SetterProperty_SET"
        //                , 0
        //                , condSetterProperty
        //                , new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
        //                {
        //                    ["Present"] = new STNodeTranslateSnippet(
        //                        new ElementCallOther("HostPresent")
        //                        , new ElementCallOther("Setter")
        //                        )
        //                }
        //                )
        //            )
        //        );

        //        // Handle the 'set-back' pattern in REF process for a setter property.
        //        exprTrans.AddSchemeSelector(
        //            ExprTranslatorAbstract.SystemScheme_VarRef
        //            , new STNodeTranslateSchemeSelector_Lambda(
        //                "SetterProperty_REF"
        //                , 0
        //                , condSetterProperty
        //                , new STNodeTranslateSchemeDefault(new Dictionary<string, STNodeTranslateSnippet>()
        //                {
        //                    ["PreStatement"] = new STNodeTranslateSnippet(
        //                        new ElementConstString("// PreStatement for SetterProperty_Ref ")
        //                        , new ElementNodeValue("VarName")
        //                        , new ElementNewLine()
        //                        , new ElementCallOther("InitTempVarBySelf")
        //                        )
        //                    ,
        //                    ["Present"] = new STNodeTranslateSnippet(
        //                        new ElementTempVar("Var")
        //                        )
        //                    ,
        //                    // // PostStatement for SetterProperty_Ref: %{VarName}%
        //                    // %{$$SYS_VAR_SET}|RHS=%{$TempVar}%%
        //                    ["PostStatementRev"] = new STNodeTranslateSnippet(
        //                        new ElementConstString("// PostStatement for SetterProperty_Ref ")
        //                        , new ElementNodeValue("VarName")
        //                        , new ElementNewLine()
        //                        , new ElementCallOther(ExprTranslatorAbstract.SystemScheme_VarSet
        //                            , new Dictionary<string, STNodeTranslateSnippet>()
        //                            {
        //                                ["RHS"] = new STNodeTranslateSnippet(
        //                                    new ElementTempVar("Var")
        //                                    )
        //                            }
        //                            )
        //                        )
        //                }
        //                )
        //            )
        //        );
        //    }
        //    // ~ Special Member Access for SetterProperty
        //    #endregion
        //}

        //#endregion
        //// ~ Scheme Selectors

        //TestCases.BasicLanguage().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
        //TestCases.BasicDataBinding().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
        //TestCases.BasicExprs().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
        //TestCases.AdvancedExpressions().ForeachSubInfo<TypeInfo>(type => _GenerateMethodsForType(exprTrans, type));
    }

    //private static void _GenerateMethodsForType(ExprTranslatorDefault InTranslator, nf.protoscript.TypeInfo InTargetType)
    //{
    //    Console.WriteLine($"Code emit sequences for Type: {InTargetType.Name}");
    //    // ctor
    //    {
    //        Console.WriteLine($"    Code emit sequences for Ctor:");

    //        var ctorEnv = new ExprTranslateEnvironmentDefault(InTargetType
    //            , new ExprTranslateEnvironmentDefault.Scope[]
    //            {
    //                new ExprTranslateEnvironmentDefault.Scope(InTargetType, "this", "this->")
    //                , new ExprTranslateEnvironmentDefault.Scope(InTargetType.ParentInfo, "global", "::")
    //            }
    //        );
    //        IMethodBodyContext ctorCtx = new VirtualMethodBodyContext(null, InTargetType, "ctor", ctorEnv);

    //        // Gather all property-init expressions and convert them to STNodeInits.
    //        List<ISyntaxTreeNode> initSyntaxes = new List<ISyntaxTreeNode>();
    //        InTargetType.ForeachSubInfoByHeader<ElementInfo>("member", memberInfo =>
    //        {
    //            if (memberInfo.InitSyntax != null)
    //            {
    //                initSyntaxes.Add(new STNodeMemberInit(memberInfo, memberInfo.InitSyntax));
    //            }
    //        });
    //        // Translate STNodeInits.
    //        STNodeSequence seq = new STNodeSequence(initSyntaxes.ToArray());
    //        var codes = InTranslator.Translate(ctorCtx, seq);
    //        foreach (var code in codes)
    //        {
    //            Console.WriteLine($"        " + code);
    //        }

    //    }

    //    // Methods
    //    InTargetType.ForeachSubInfoByHeader<ElementInfo>("method", mtdInfo =>
    //    {
    //        Console.WriteLine($"    Code emit sequences for {mtdInfo.Name}:");

    //        var mtdEnv = new ExprTranslateEnvironmentDefault(mtdInfo
    //            , new ExprTranslateEnvironmentDefault.Scope[]
    //            {
    //                new ExprTranslateEnvironmentDefault.Scope(mtdInfo, "local", "")
    //                , new ExprTranslateEnvironmentDefault.Scope(InTargetType, "this", "this->")
    //                , new ExprTranslateEnvironmentDefault.Scope(InTargetType.ParentInfo, "global", "::")
    //            }
    //            );
    //        MethodBodyContext funcBodyCtx = new MethodBodyContext(null, mtdInfo, mtdEnv);
    //        var codes = InTranslator.Translate(funcBodyCtx, mtdInfo.InitSyntax);
    //        foreach (var code in codes)
    //        {
    //            Console.WriteLine("        " + code);
    //        }
    //    });
    //}

}
