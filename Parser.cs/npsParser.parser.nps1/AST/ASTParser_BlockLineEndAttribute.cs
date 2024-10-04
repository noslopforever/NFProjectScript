using nf.protoscript.syntaxtree;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse attributes that appear after a definition but before the end of the line.
    /// 
    /// Example:
    /// unit Arrow @massunit @fakeunit
    ///            ^-------^
    /// </summary>
    class ASTParser_BlockLineEndAttribute
        : ASTParser_Base<STNode_AttributeDef>
    {
        /// <inheritdoc />
        public override STNode_AttributeDef Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a syntax tree node representing a single line-end attribute.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <returns>The parsed syntax tree node, or null if parsing fails.</returns>
        /// <exception cref="ParserException"></exception>
        public static STNode_AttributeDef StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try to check the '@' token.
            // @Attr = InitExpr
            // ^
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.At))
            {
                return null;
            }
            // Consume the '@' token.
            RefStartIndex++;

            // Try to check the attribute identifier.
            // @Attr = InitExpr
            //  ^--^
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                var idToken = InTokens[RefStartIndex];
                // Consume the attribute identifier. (e.g., 'Attr' in the example) 
                RefStartIndex++;

                // Create a new attribute Def using the parsed attribute identifier.
                STNode_AttributeDef attrDef = new STNode_AttributeDef(idToken.Code);

                // Try to parse the init-expr if it exists.
                // @Attr = InitExpr
                //       ^--------^a
                var initExpr = ASTParser_BlockInitExpr.StaticParse(InTokens, ref RefStartIndex);
                if (initExpr != null)
                {
                    attrDef._Internal_SetInitExpr(initExpr);
                }
                return attrDef;
            }
            else
            {
                // If the expected identifier is not found, throw an exception.
                throw new ParserException(
                    ParserErrorType.AST_UnexpectedToken
                    , InTokens[RefStartIndex]
                    , CommonTokenTypes.ID.ToString()
                    );
            }

        }
    }


    /// <summary>
    /// Parser to parse all line-end attributes.
    /// 
    /// Examples:
    /// unit Arrow @massunit @fakeunit
    ///            ^-----------------^
    /// </summary>
    class ASTParser_BlockLineEndAttributes
        : ASTParser_Base<STNode_AttributeDefs>
    {
        /// <inheritdoc />
        public override STNode_AttributeDefs Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            STNode_AttributeDefs attrDefs = new STNode_AttributeDefs();

            // Continue parsing line-end attributes as long as the next token is a '@' token.
            while (InTokens[RefStartIndex].Check(CommonTokenTypes.At))
            {
                var attrDef = ASTParser_BlockLineEndAttribute.StaticParse(InTokens, ref RefStartIndex);
                attrDefs.Add(attrDef);
            }

            return attrDefs;
        }
    }

}