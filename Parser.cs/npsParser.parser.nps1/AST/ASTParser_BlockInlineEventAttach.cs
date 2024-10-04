using nf.protoscript.syntaxtree;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse inline event attachments.
    /// 
    /// Example:
    /// >> OnClick += cout(mousePt)
    ///            ^--------------^
    /// 
    /// </summary>
    internal class ASTParser_BlockInlineEventAttach
        : ASTParser_Base<ISyntaxTreeNode>
    {
        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a syntax tree node representing an inline event attachment.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <returns>The parsed syntax tree node, or null if parsing fails.</returns>
        public static ISyntaxTreeNode StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try checking for and consuming the '+=' operator
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.Operator, "+="))
            {
                return null;
            }
            RefStartIndex++;

            // Parse the next tokens as an expression statement.
            var expr = ASTParser_Statement.StaticParse(InTokens, ref RefStartIndex);
            return expr;
        }
    }
}