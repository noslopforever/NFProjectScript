using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse elements.
    /// 
    /// Example:
    /// -n [Min=0][Max=100] HP
    ///                     ^^
    ///                     
    /// </summary>
    internal class ASTParser_BlockDefElem
        : ASTParser_Base<STNode_ElementDef>
    {

        /// <inheritdoc />
        public override STNode_ElementDef Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }


        /// <summary>
        /// Static method to parse the tokens into a syntax tree node representing an element definition.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <returns>The parsed syntax tree node, or null if parsing fails.</returns>
        public static STNode_ElementDef StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Check if the current token is an ID.
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                return null;
            }

            // Save the current ID and move to the next token.
            var elemNameToken = InTokens[RefStartIndex];
            RefStartIndex++;

            // Create a new element definition with the ID as the name.
            var result = new STNode_ElementDef(elemNameToken.Code);
            return result;
        }


    }
}