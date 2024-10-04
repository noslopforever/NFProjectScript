using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Parser to parse function definitions.
    /// 
    /// Example:
    /// -n [Pure] getHP() = return 100
    ///           ^-----^
    /// 
    /// </summary>
    internal class ASTParser_BlockDefFunction
        : ASTParser_Base<STNode_FunctionDef>
    {
        /// <inheritdoc />
        public override STNode_FunctionDef Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a syntax tree node representing a function definition.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <returns>The parsed syntax tree node, or null if parsing fails.</returns>
        public static STNode_FunctionDef StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Check if the current token is an ID.
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                return null;
            }

            // Get the function name and move to the next token.
            // -n getSth(InParam0, InParam1)
            //    ^----^
            var funcNameToken = InTokens[RefStartIndex];
            RefStartIndex++;

            // Handle parameter lists if they exist.
            // -n getSth(InParam0, InParam1)
            //          ^------------------^
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.OpenParen))
            {
                throw new ParserException(
                    ParserErrorType.AST_UnexpectedToken
                    , InTokens[RefStartIndex]
                    , "("
                    );
            }
            var paramsParser = new ASTParser_BlockParamList();
            var paramDefs = paramsParser.Parse(InTokens, ref RefStartIndex);
            if (paramDefs == null)
            {
                // TODO unexpecetd parameter list.
                throw new NotImplementedException();
            }

            // Create and return the result function definition.
            STNode_FunctionDef funcDef = new STNode_FunctionDef(funcNameToken.Code, paramDefs);
            return funcDef;
        }

    }
}