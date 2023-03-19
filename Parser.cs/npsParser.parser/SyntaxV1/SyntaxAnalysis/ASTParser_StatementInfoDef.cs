using System;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Try parse tokens as a Info-Def statement which starts with a Type-Def block.
    ///     -n [Min=0][Max=100] HP = 100
    ///     -n [Pure] getHP() = return 100
    /// </summary>
    class ASTParser_StatementInfoDef
        : ASTParser_Base<STNode_DefBase>
    {
        ASTParser_BlockType _TypeBlockParser = new ASTParser_BlockType();
        ASTParser_BlockAttributes _AttributesParser = new ASTParser_BlockAttributes();
        ASTParser_BlockFunctionDef _FuncDefParser = new ASTParser_BlockFunctionDef();
        ASTParser_BlockElemDef _ElemParser = new ASTParser_BlockElemDef();
        ASTParser_BlockInitExpr _InitParser = new ASTParser_BlockInitExpr();

        public override STNode_DefBase Parse(TokenList InTokenList)
        {
            // try parse type-block at first
            // -n [Min=0][Max=100] HP = 100.
            //  ^
            STNode_TypeSignature typeSig = _TypeBlockParser.Parse(InTokenList);
            if (typeSig == null)
            {
                // TODO log error
                throw new NotImplementedException();
                return null;
            }

            // try parse attributes:
            // -n [Min=0][Max=100] HP = 100.
            //    ^--------------^
            // -n [Pure] getHP() = return 100
            //    ^----^
            STNode_AttributeDefs attrs = _AttributesParser.Parse(InTokenList);

            // try parse function-def in-front of element-def
            // -n [Pure] getHP() = return 100
            //           ^-----^
            STNode_DefBase resultDef = _FuncDefParser.Parse(InTokenList);

            // ... then element-def
            // -n [Min=0][Max=100] HP = 100.
            //                     ^^
            if (resultDef == null)
            {
                resultDef = _ElemParser.Parse(InTokenList);
            }

            if (resultDef == null)
            {
                // TODO log error
                throw new NotImplementedException();
                return null;
            }

            // Assign attributes/types to the result definition.
            resultDef._Internal_SetType(typeSig);
            resultDef._Internal_SetAttributes(attrs);

            // try parse init-expressions
            if (InTokenList.CheckToken(token.ETokenType.Assign))
            {
                syntaxtree.STNodeBase initExpr = _InitParser.Parse(InTokenList);
                if (typeSig == null)
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
                resultDef._Internal_SetInitExpr(initExpr);
            }

            return resultDef;
        }

    }

}