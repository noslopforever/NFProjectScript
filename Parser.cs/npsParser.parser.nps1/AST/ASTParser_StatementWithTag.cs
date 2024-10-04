using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{


    /// <summary>
    /// Parses a statement with an optional tag.
    /// The format is [tag]:[statement].
    /// </summary>
    class ASTParser_StatementWithTag
        : ASTParser_Base<ISyntaxTreeNode>
    {
        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a statement with an optional tag.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">A reference to the current index in the token list. Updated as tokens are consumed.</param>
        /// <returns>The parsed statement node, or null if parsing fails.</returns>
        public static ISyntaxTreeNode StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Parse the tag and consume tag tokens:
            // Tag: OtherExpressions()
            // ^--^
            // ID Colon
            string tag = "";
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID)
                && RefStartIndex + 1 < InTokens.Count
                && InTokens[RefStartIndex + 1].Check(CommonTokenTypes.Colon)
                )
            {
                // Extract the tag from the current token.
                tag = InTokens[RefStartIndex].Code;

                // Consume the ID and Colon tokens.
                RefStartIndex++;
                RefStartIndex++;

                // TODO support tag in ISyntaxTreeNode.
                throw new NotImplementedException("Tag support in ISyntaxTreeNode is not implemented.");
            }

            // Parse the statement after the tag (if any).
            var statement = ASTParser_Statement.StaticParse(InTokens, ref RefStartIndex);

            // TODO Uncomment this line once the SetTag method is implemented.
            //statement.SetTag(tag);

            return statement;
        }


    }

}