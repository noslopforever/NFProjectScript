using System;
using System.Collections.Generic;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parses tokens to create a return statement.
    /// </summary>
    internal class ASTParser_StatementReturn
        : ASTParser_Base<STNodeReturn>
    {

        /// <inheritdoc />
        public override STNodeReturn Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a return statement node.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">A reference to the current index in the token list. Updated as tokens are consumed.</param>
        /// <returns>The parsed return statement node, or null if parsing fails.</returns>
        public static STNodeReturn StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Check if the current token is an identifier (ID).
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                // Ensure the identifier is the "return" keyword.
                if (InTokens[RefStartIndex].Code.ToLower() != "return")
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokens[RefStartIndex]
                        , "ID:Return"
                        );
                }
                // Consume the "return" keyword.
                RefStartIndex++;

                // Parse the following tokens as an expression.
                ASTParser_Expression exprParser = new ASTParser_Expression();
                var expr = exprParser.Parse(InTokens, ref RefStartIndex);

                // Create and return the return statement node.
                return new STNodeReturn(expr);
            }

            // If the current token is not an identifier or it's not the "return" keyword, return null.
            return null;
        }


    }
}