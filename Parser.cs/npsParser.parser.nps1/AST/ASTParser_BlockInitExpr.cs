using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse initialization expressions.
    /// 
    /// Example:
    /// -n [Min=0][Max=100] HP = 100
    ///                        ^---^
    /// </summary>
    internal class ASTParser_BlockInitExpr
        : ASTParser_Base<ISyntaxTreeNode>
    {

        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a syntax tree node representing an initialization expression.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <returns>The parsed syntax tree node, or null if parsing false.</returns>
        public static ISyntaxTreeNode StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try checking for and consuming the '=' operator.
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.Operator, "="))
            {
                return null;
            }
            RefStartIndex++;

            // Parse the next tokens as an expression.
            ASTParser_Expression exprParser = new ASTParser_Expression();
            var expr = exprParser.Parse(InTokens, ref RefStartIndex);
            return expr;
        }
    }

}