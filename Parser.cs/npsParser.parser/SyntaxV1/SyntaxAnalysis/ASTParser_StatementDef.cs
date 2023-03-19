using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Try parse tokens as a CommonDef statement.
    /// Like element definitions:
    ///     - HP:n = 100
    ///     +OverrideHP = 100
    ///     -getHP():n = return 100
    /// or parameter definitions:
    ///     SomeMethod(InParam0:n = 10, InParam1:s = "id")
    ///                ^-------------^  ^---------------^
    /// </summary>
    class ASTParser_StatementDef
        : ASTParser_Base<STNode_DefBase>
    {
        // sub blocks.

        ASTParser_BlockType _TypeBlockParser = new ASTParser_BlockType();
        ASTParser_BlockAttributes _AttributesParser = new ASTParser_BlockAttributes();
        ASTParser_BlockFunctionDef _FuncDefParser = new ASTParser_BlockFunctionDef();
        ASTParser_BlockElemDef _ElemParser = new ASTParser_BlockElemDef();
        ASTParser_BlockInitExpr _InitParser = new ASTParser_BlockInitExpr();

        public bool OnlyCheckMember { get; set; } = false;

        public bool OnlyCheckFunction { get; set; } = false;

        public override STNode_DefBase Parse(TokenList InTokenList)
        {
            // try parse attributes:
            STNode_AttributeDefs attrs = _AttributesParser.Parse(InTokenList);

            // try parse function-def
            // -n getHP()
            //    ^-----^
            //
            STNode_DefBase resultDef = null;
            if (!OnlyCheckMember)
            {
                resultDef = _FuncDefParser.Parse(InTokenList);
            }

            // ... then try parse variable-def
            // -n HP
            //    ^^
            if (resultDef == null
                && !OnlyCheckFunction
                )
            {
                resultDef = _ElemParser.Parse(InTokenList);
            }

            if (resultDef == null)
            {
                // TODO log error
                throw new NotImplementedException();
                return null;
            }

            // Assign attributes which have already been parsed before.
            if (attrs != null)
            {
                resultDef._Internal_SetAttributes(attrs);
            }

            // try parse type signature in postfix-mode.
            // -HP:n
            //    ^^
            if (InTokenList.CheckToken(token.ETokenType.Colon))
            {
                InTokenList.Consume();
                STNode_TypeSignature typeSig = _TypeBlockParser.Parse(InTokenList);
                if (typeSig == null)
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
                resultDef._Internal_SetType(typeSig);
            }

            // try parse init-expressions
            if (InTokenList.CheckToken(token.ETokenType.Assign))
            {
                syntaxtree.STNodeBase initExpr = _InitParser.Parse(InTokenList);
                if (initExpr == null)
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }

                // Assign init-expressions.
                resultDef._Internal_SetInitExpr(initExpr);
            }

            return resultDef;
        }

    }

}