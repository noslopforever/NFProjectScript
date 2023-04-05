using nf.protoscript.parser.token;
using System;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse attributes which appears after a def but before eol:
    /// unit Arrow @massunit @fakeunit
    ///            ^-------^
    /// </summary>
    class ASTParser_BlockLineEndAttribute
        : ASTParser_Base<STNode_AttributeDef>
    {
        public override STNode_AttributeDef Parse(TokenList InTokenList)
        {
            // @Attr = InitExpr
            // ^
            if (InTokenList.CheckToken(ETokenType.At))
            {
                InTokenList.Consume();

                // @Attr = InitExpr
                //  ^--^
                if (InTokenList.CheckToken(ETokenType.ID))
                {
                    var idToken = InTokenList.CurrentToken;
                    InTokenList.Consume();

                    // New attribute Def
                    STNode_AttributeDef attrDef = new STNode_AttributeDef(idToken.Code);

                    // Try parse init-expr
                    // @Attr = InitExpr
                    //       ^--------^
                    if (InTokenList.CheckToken(ETokenType.Assign, "="))
                    {
                        InTokenList.Consume();
                        var exprParser = new ASTParser_Expression();
                        var expr = exprParser.Parse(InTokenList);
                        attrDef._Internal_SetInitExpr(expr);
                    }
                    return attrDef;
                }
                else
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokenList.CurrentToken
                        , ETokenType.ID.ToString()
                        );
                }
            }
            return null;
        }
    }


    /// <summary>
    /// Parser to parse all line-end attributes:
    /// unit Arrow @massunit @fakeunit
    ///            ^-----------------^
    /// </summary>
    class ASTParser_BlockLineEndAttributes
        : ASTParser_Base<STNode_AttributeDefs>
    {
        public override STNode_AttributeDefs Parse(TokenList InTokenList)
        {
            STNode_AttributeDefs attrDefs = new STNode_AttributeDefs();
            while (InTokenList.CheckToken(ETokenType.At))
            {
                ASTParser_BlockLineEndAttribute attrParser = new ASTParser_BlockLineEndAttribute();
                var attrDef = attrParser.Parse(InTokenList);
                attrDefs.Add(attrDef);
            }

            return attrDefs;
        }
    }

}