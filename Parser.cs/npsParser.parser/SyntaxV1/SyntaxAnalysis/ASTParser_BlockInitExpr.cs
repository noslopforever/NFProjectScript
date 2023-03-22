using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse init-expressions.
    /// 
    /// -n [Min=0][Max=100] HP = 100
    ///                        ^---^
    /// </summary>
    internal class ASTParser_BlockInitExpr
        : ASTParser_Base<syntaxtree.STNodeBase>
    {
        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            if (!InTokenList.CheckToken(ETokenType.Assign, "="))
            {
                return null;
            }

            // Consume '='
            InTokenList.Consume();

            // Take next tokens as an expression.
            ASTParser_Expression exprParser = new ASTParser_Expression();
            var expr = exprParser.Parse(InTokenList);
            return expr;
        }
    }

}