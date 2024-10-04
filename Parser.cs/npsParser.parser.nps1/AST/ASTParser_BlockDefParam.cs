using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Try parse tokens as a param-def statement.
    /// 
    /// Example:
    /// + SomeMethod(InParam0:n = 10, InParam1:s = "id")
    ///              ^-------------^  ^---------------^
    /// </summary>
    class ASTParser_BlockDefParam
        : ASTParser_Base<STNode_ElementDef>
    {
        public ASTParser_BlockDefParam()
        {
        }

        /// <inheritdoc />
        public override STNode_ElementDef Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a syntax tree node representing a parameter definition.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <returns>The parsed syntax tree node, or null if parsing fails.</returns>
        public static STNode_ElementDef StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // [UIMin = 0][UIMax = 100]   HP    :integer = 100
            // ^----------------------^   ^^    ^------^ ^---^
            // InlineAttributes        BlockID  TypeSig  Init-Expr

            // Check if the current token is an ID (BlockID) or a '[' token (InlineAttributes).
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID)
                && !InTokens[RefStartIndex].Check(CommonTokenTypes.OpenBracket)
                )
            {
                return null;
            }

            // Try to parse inline attributes.
            var inlineAttrs = ASTParser_BlockInlineAttributes.StaticParse(InTokens, ref RefStartIndex);

            // Attempt to consume the BlockID (e.g., 'HP' in the example).
            var idToken = InTokens[RefStartIndex];
            RefStartIndex++;

            // Create the new element definition using the BlockID.
            // If inline attributes were successfully parsed, add them to the new element definition.
            var result = new STNode_ElementDef(idToken.Code);
            if (inlineAttrs != null)
            {
                result._Internal_AddAttributes(inlineAttrs);
            }

            // Try to parse {:Type} block.
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.Colon))
            {
                RefStartIndex ++;
                var typeSig = ASTParser_BlockType.StaticParse(InTokens, ref RefStartIndex);
                if (typeSig != null)
                {
                    result._Internal_SetType(typeSig);
                }
            }

            // Try to parse the Init-Expr block.
            var expr = ASTParser_BlockInitExpr.StaticParse(InTokens, ref RefStartIndex);
            if (expr != null)
            {
                result._Internal_SetInitExpr(expr);
            }

            return result;
        }
    }

}