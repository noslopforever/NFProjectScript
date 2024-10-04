using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Parser to parse tokens into a type signature.
    /// </summary>
    internal class ASTParser_BlockType
        : ASTParser_Base<STNode_TypeSignature>
    {

        /// <inheritdoc />
        public override STNode_TypeSignature Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <inheritdoc />
        public static STNode_TypeSignature StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Check the type identifier.
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                throw new ParserException(
                    ParserErrorType.AST_UnexpectedToken
                    , InTokens[RefStartIndex]
                    , CommonTokenTypes.ID.ToString()
                    );
            }

            // Get the type identifier and move to the next token.
            var typeCodeToken = InTokens[RefStartIndex];
            RefStartIndex++;

            // TODO impl collections/ delegate-types
            //throw new NotImplementedException();

            // Create and return a new TypeSignature node.
            return new STNode_TypeSignature(typeCodeToken.Code);
        }

    }
}