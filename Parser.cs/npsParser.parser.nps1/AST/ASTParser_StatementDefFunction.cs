using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse tokens as a function definition statement.
    /// 
    /// Examples:
    /// 
    ///     # pre-type function definition:
    ///     +n getHP() = return 100
    ///     
    ///     # post-type function definition:
    ///     + getHP():n = return 100
    /// 
    /// </summary>
    class ASTParser_StatementDefFunction
        : ASTParser_Base<STNode_FunctionDef>
    {
        /// <inheritdoc />
        public override STNode_FunctionDef Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a function definition node.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">A reference to the current index in the token list. Updated as tokens are consumed.</param>
        /// <returns>The parsed function definition node, or null if parsing fails.</returns>
        public static STNode_FunctionDef StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // integer     getHP  (     )      :integer     = return 100        @Min=0 @Max=100        # Health-point
            // ^-----^     ^------------^      ^------^     ^----------^        ^-------------^        ^------------^
            // [o]TypeSig  BlockFunctionDef    [o]TypeSig   [o]Expr-Statement   [o]LineEndAttributes   [o]LineEndComments

            // Attempt to parse the pre-type definition.
            // This method ensures that it is a valid type definition (if present), and subsequent parsing can start from BlockFunctionDef.
            int preTypeIndex = RefStartIndex;
            STNode_TypeSignature preTypeSig = ASTParser_StatementDefMember._StaticTryParsePreType(InTokens, ref preTypeIndex);
            if (preTypeSig != null)
            {
                // Successfully parsed a valid pre-type definition, skip all type tokens.
                RefStartIndex = preTypeIndex;
            }

            // Function definition must start with an identifier (ID).
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                return null;
            }

            // Parse the function definition
            STNode_FunctionDef result = ASTParser_BlockDefFunction.StaticParse(InTokens, ref RefStartIndex);
            result._Internal_SetType(preTypeSig);

            // Attempt to parse the post-type definition, which is the part after the colon, e.g., ":integer".
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.Colon))
            {
                // Consume the ':' token.
                RefStartIndex++;

                // Parse the next tokens as an BlockType.
                var postTypeSig = ASTParser_BlockType.StaticParse(InTokens, ref RefStartIndex);
                if (postTypeSig != null)
                {
                    result._Internal_SetType(postTypeSig);
                }
            }

            // If there is an '=' token, attempt to parse the inline function body, e.g., "= return 100".
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.Operator, "="))
            {
                // consume the '=' operator
                RefStartIndex++;

                // Parse the next tokens as an ExpressionStatement.
                var expr = ASTParser_Statement.StaticParse(InTokens, ref RefStartIndex);
                if (expr != null)
                {
                    result._Internal_SetInitExpr(expr);
                }
            }

            // TODO: Continue parsing end-of-line attributes and comments.

            return result;
        }

    }

}