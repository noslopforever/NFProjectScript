﻿using System;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parse tokens to return statement.
    /// </summary>
    internal class ASTParser_StatementReturn
        : ASTParser_Base<syntaxtree.STNodeBase>
    {

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            if (InTokenList.CheckToken(ETokenType.ID))
            {
                if (InTokenList.CurrentToken.Code.ToLower() != "return")
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokenList.CurrentToken
                        , "ID:Return"
                        );
                }
                // Consume "return" keyword.
                InTokenList.Consume();

                // Parse other codes as expressions.
                ASTParser_Expression exprParser = new ASTParser_Expression();
                var expr = exprParser.Parse(InTokenList);

                return new syntaxtree.STNodeReturn(expr);
            }

            return null;
        }

    }
}