using nf.protoscript;
using nf.protoscript.translator;
using nf.protoscript.translator.DefaultScheme;
using nf.protoscript.translator.DefaultScheme.Elements;
using nf.protoscript.syntaxtree;
using System.Collections.Generic;
using System;

namespace nf.protoscript.test
{

    internal partial class TestTranslatorBase
        : InfoTranslatorDefault
    {
        public TestTranslatorBase()
        {
            // Constant: ${ValueString}
            AddExprScheme<STNodeConstant>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${ValueString}"))
                );
            // Constant<string>: "${ValueString}"
            AddExprSelector<STNodeConstant>("Get", 1
                , expr => { return expr.ValueType == CommonTypeInfos.String || expr.ValueType == CommonTypeInfos.TypeRef; }
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("\"${ValueString}\""))
                );

            // BinaryOp: ${LHS.Get()} ${OpCode} ${RHS.Get()}
            AddExprScheme<STNodeBinaryOp>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${LHS.Get()} ${OpCode} ${RHS.Get()}"))
                );

            // UnaryOp: ${OpCode} ${RHS.Get()}
            AddExprScheme<STNodeUnaryOp>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${OpCode}${RHS.Get()}"))
                );

            // Assign: ${LHS.Set(RHS.Get())}
            AddExprScheme<STNodeAssign>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${LHS.Set(RHS.Get())}"))
                );

            // Default HostPresent (VAR_NAME): ${VAR_NAME}
            AddScheme("HostPresent"
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
            AddExprScheme<STNodeVar>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${HostPresent(IDName)}"))
                );
            // Var Set (RHS): ${HostPresent(IDName)} = $RHS
            AddExprScheme<STNodeVar>("Set", 1
                , new InfoTranslateSchemeDefault(new string[1] { "RHS" }, ElementParser.ParseElements("${HostPresent(IDName)} = ${RHS}"))
                );
            // Member Get: ${LHS.Get()}.${IDName}
            AddExprScheme<STNodeMemberAccess>("Get", 1
                , new InfoTranslateSchemeDefault(ElementParser.ParseElements("${LHS.Get()}.${IDName}"))
                );
            // Member Set (RHS): ${LHS.Get()}.${IDName} = ${RHS}
            AddExprScheme<STNodeMemberAccess>("Set", 1
                , new InfoTranslateSchemeDefault(new string[1] { "RHS" }, ElementParser.ParseElements("${LHS.Get()}.${IDName} = ${RHS}"))
                );

            // Call: ${FuncExpr.Get()}(${For("Param", ", ", "Get")}
            AddExprScheme<STNodeCall>("Get", 1
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${FuncExpr.Get()}(${For("Param", ", ", "Get")})
                        """
                        )
                    )
                );

            // Collection access: ${CollExpr.Get()}[${$For("Param", "][", "Get")}]
            AddExprScheme<STNodeCollectionAccess>("Get", 1
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${CollExpr.Get()}[${For("Param", "][", "Get")}]
                        """
                        )
                    )
                );

            // Sequence: 
            AddExprScheme<STNodeSequence>("Get", 1
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${For("", $NL, "Get")}
                        """
                        )
                    )
                );

        }
    }
}