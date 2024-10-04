using System;
using System.Collections.Generic;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Base class of all Abstract Syntax Tree (AST) Parsers.
    /// </summary>
    abstract class ASTParser_Base
        : IASTParser
    {

        /// <summary>
        /// Helper method to simplify calling sub-parsers and handling the result.
        /// </summary>
        /// <typeparam name="T">The type of the parser, which must derived from <see cref="ASTParser_Base{U}"/>.</typeparam>
        /// <typeparam name="U">The type of the syntax tree node that the parser will produce.</typeparam>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <param name="InSuccessAction">The action to perform if parsing is successful.</param>
        /// <param name="InNullAction">The action to perform if the result is null (optional).</param>
        /// <param name="InExAction">The action to perform if an exception occurs (optional).</param>
        public static void StaticParseAST<T, U>(
            IReadOnlyList<IToken> InTokens
            , ref int RefStartIndex
            , Action<ISyntaxTreeNode> InSuccessAction
            , Action InNullAction = null
            , Action<Exception> InExAction = null
            )
            where U : ISyntaxTreeNode
            where T : ASTParser_Base<U>, new()
        {
            U result = default(U);
            T parser = new T();
            try
            {
                // Parse the tokens using the specific parser.
                result = parser.Parse(InTokens, ref RefStartIndex);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during parsing.
                if (InExAction != null)
                {
                    InExAction(ex);
                }
                else
                {
                    throw ex;
                }
            }

            // Call SuccessAction if the result is null, otherwise NullAction.
            if (result != null)
            {
                InSuccessAction(result);
            }
            else if (InNullAction != null)
            {
                InNullAction();
            }
        }

        /// <inheritdoc />
        public abstract ISyntaxTreeNode ParseTokens(IReadOnlyList<IToken> InTokens, ref int RefStartIndex);

    }

    /// <summary>
    /// Base class for all generic Abstract Syntax Tree (AST) parsers.
    /// </summary>
    abstract class ASTParser_Base<T>
        : ASTParser_Base
        where T : ISyntaxTreeNode
    {
        /// <summary>
        /// Parses tokens into a syntax tree node.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        /// <returns>The parsed syntax tree node, or null if parsing fails.</returns>
        public abstract T Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex);

        /// <inheritdoc />
        public sealed override ISyntaxTreeNode ParseTokens(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Delegate the actual parsing to the generic Parse method.
            return Parse(InTokens, ref RefStartIndex);
        }

    }

}