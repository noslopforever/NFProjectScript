using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{


    /// <summary>
    /// Try parse tokens as a member-def statement.
    /// 
    /// Samples:
    ///     # Member definitions:
    ///     
    ///     ## Member definition with inline attributes and pre-type.
    ///     -n HP [Min=0][Max=100] = 100
    ///     
    ///     ## Member definition with post-type.
    ///     - HP:n = 100
    ///     
    ///     ## Member definition with no type.
    ///     -HP = 100
    ///     - HP = 100
    /// 
    ///     # parameter definitions, in this case, only support post-type or no-type definitions:
    ///     SomeMethod(InParam0:n = 10, InParam1:s = "id")
    ///                ^-------------^  ^---------------^
    /// </summary>
    class ASTParser_StatementDefMember
        : ASTParser_Base<STNode_ElementDef>
    {
        public override STNode_ElementDef Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        public static STNode_ElementDef StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // integer  HP      [UIMin = 0][UIMax = 100]    :integer = 100
            // ^-----^  ^^      ^----------------------^    ^------^ ^---^
            // TypeSig  BlockID  InlineAttributes           TypeSig  Init-Expr

            // Check if the current token is an identifier (ID).
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                return null;
            }

            // Attempt to parse the pre-type definition.
            // This method ensures that it is a valid type definition (if present), and subsequent parsing can start from BlockFunctionDef.
            int preTypeIndex = RefStartIndex;
            STNode_TypeSignature preTypeSig = _StaticTryParsePreType(InTokens, ref preTypeIndex);
            if (preTypeSig != null)
            {
                // Successfully parsed a valid pre-type definition, skip all type tokens.
                RefStartIndex = preTypeIndex;
            }

            // Parse the block ID, create an element definition.
            var idToken = InTokens[RefStartIndex];
            RefStartIndex++;
            var result = new STNode_ElementDef(idToken.Code);
            result._Internal_SetType(preTypeSig);

            // Try to parse inline-attributes
            STNode_AttributeDefs inlineAttrs = ASTParser_BlockInlineAttributes.StaticParse(InTokens, ref RefStartIndex);
            if (inlineAttrs != null)
            {
                result._Internal_AddAttributes(inlineAttrs);
            }

            // Try to parse the post type signature.
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.Colon))
            {
                // Consume the ':' token
                RefStartIndex++;
                
                // Try to parse next tokens as a BlockType
                var postTypeSig = ASTParser_BlockType.StaticParse(InTokens, ref RefStartIndex);
                if (postTypeSig != null)
                {
                    result._Internal_SetType(postTypeSig);
                }
            }

            // Try to parse the initialization expression.
            var expr = ASTParser_BlockInitExpr.StaticParse(InTokens, ref RefStartIndex);
            if (expr != null)
            {
                result._Internal_SetInitExpr(expr);
            }

            return result;
        }


        /// <summary>
        /// Tries to parse the current token as a pre-type signature.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">A reference to the current index in the token list. Updated as tokens are consumed.</param>
        /// <returns>The parsed type signature, or null if no valid type signature is found.</returns>
        internal static STNode_TypeSignature _StaticTryParsePreType(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try parse the current token as a type signature first.
            var typeSig = ASTParser_BlockType.StaticParse(InTokens, ref RefStartIndex);
            // If a valid type signature cannot be recognized, it means there is no pre-type, so return null.
            if (typeSig == null)
            {
                return null;
            }

            // Consider this example: integer[2] getHP()
            // No matter how complex the type signature is, the token after the type definition must be an identifier (ID).
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                return null;
            }

            return typeSig;
        }


    }

}