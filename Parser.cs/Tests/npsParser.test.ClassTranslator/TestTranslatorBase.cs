using nf.protoscript;
using nf.protoscript.translator;
using nf.protoscript.translator.DefaultSnippetElements;
using nf.protoscript.syntaxtree;
using System.Collections.Generic;
using System;

namespace npsParser.test.ClassTranslator
{

    internal partial class TestTranslatorBase
        : InfoTranslatorDefault
    {
        public TestTranslatorBase()
        {
            // Constant: ${ValueString}
            AddExprScheme<STNodeConstant>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], "${ValueString}")
                );
            // Constant<string>: "${ValueString}"
            AddExprSelector<STNodeConstant>("Get", 1
                , expr => { return expr.Type == CommonTypeInfos.String || expr.Type == CommonTypeInfos.TypeRef; }
                , new InfoTranslateSchemeDefault(new string[0], "\"${ValueString}\"")
                );
            // BinaryOp: ${LHS.Get()} ${OpCode} ${RHS.Get()}
            AddExprScheme<STNodeBinaryOp>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], "${LHS.Get()} ${OpCode} ${RHS.Get()}")
                );
            // UnaryOp: ${OpCode} ${RHS.Get()}
            AddExprScheme<STNodeUnaryOp>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], "${OpCode}${RHS.Get()}")
                );
            // Assign: ${LHS.Set(RHS.Get())}
            AddExprScheme<STNodeAssign>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], "${LHS.Set(RHS.Get())}")
                );

            // Default HostPresent (VAR_NAME): ${VAR_NAME}
            AddScheme("HostPresent"
                , new InfoTranslateSchemeDefault(new string[] { "VAR_NAME" }, "${VAR_NAME}")
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
            AddExprScheme<STNodeVar>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], "${HostPresent(IDName)}")
                );
            // Var Set (RHS): ${HostPresent(IDName)} = $RHS
            AddExprScheme<STNodeVar>("Set", 1
                , new InfoTranslateSchemeDefault(new string[1] { "RHS" }, "${HostPresent(IDName)} = ${RHS}")
                );
            // Member Get: ${LHS.Get()}.${IDName}
            AddExprScheme<STNodeMemberAccess>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], "${LHS.Get()}.${IDName}")
                );
            // Member Set (RHS): ${LHS.Get()}.${IDName} = ${RHS}
            AddExprScheme<STNodeMemberAccess>("Set", 1
                , new InfoTranslateSchemeDefault(new string[1] { "RHS" }, "${LHS.Get()}.${IDName} = ${RHS}")
                );

            // Call: ${FuncExpr.Get()}(${For("Param", ", ", "Get")}
            AddExprScheme<STNodeCall>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], """${FuncExpr.Get()}(${For("Param", ", ", "Get")})"""
                    )
                );

            // Collection access: ${CollExpr.Get()}[${$For("Param", "][", "Get")}]
            AddExprScheme<STNodeCollectionAccess>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], """${CollExpr.Get()}[${For("Param", "][", "Get")}]""")
                );

            // Sequence: 
            AddExprScheme<STNodeSequence>("Get", 1
                , new InfoTranslateSchemeDefault(new string[0], """${For("", $NL, "Get")}""")
                );

        }
    }
}