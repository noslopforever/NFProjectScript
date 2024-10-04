using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Base parser to parse a list of child syntax tree nodes.
    /// </summary>
    /// <typeparam name="T">The type of the individual child syntax tree node.</typeparam>
    /// <typeparam name="TResultList">The type of the result list that will contain the parsed child nodes.</typeparam>
    internal abstract class ASTParser_ChildListBase<T, TResultList>
        : ASTParser_Base<T>
        where T : class, ISyntaxTreeNode, new()
        where TResultList: class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ChildListBase{T, TResultList}"/> class.
        /// </summary>
        /// <param name="InStartToken">The token that marks the start of the list. (e.g., '(' in "(param1, param2, param3)")</param>
        /// <param name="InEndToken">The token that marks the end of the list. (e.g., ')' in "(param1, param2, param3)")</param>
        /// <param name="InSeperatorToken">The token that separates items in the list. (e.g., ',' in "(param1, param2, param3)")</param>
        protected ASTParser_ChildListBase(
            string InStartToken
            , string InEndToken
            , string InSeperatorToken
            )
        {
            StartToken = InStartToken;
            EndToken = InEndToken;
            SeperatorToken = InSeperatorToken;
        }

        /// <summary>
        /// The token that marks the start of the list. 
        /// </summary>
        /// <example>
        /// (param1, param2, param3)
        /// ^
        /// </example>
        public string StartToken { get; }

        /// <summary>
        /// The token that marks the end of the list. 
        /// </summary>
        /// <example>
        /// (param1, param2, param3)
        ///                        ^
        /// </example>
        public string EndToken { get; }

        /// <summary>
        /// The token that separates items in the list. 
        /// </summary>
        /// <example>
        /// (param1, param2, param3)
        ///        ^       ^
        /// </example>
        public string SeperatorToken { get; }

        /// <inheritdoc />
        public sealed override T Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Attempt to check for and consume the start token (e.g. '(')
            // -n getSth(InParam0, InParam1)
            //          ^------------------^
            if (!InTokens[RefStartIndex].Check(StartToken))
            {
                return default(T);
            }
            RefStartIndex++;

            // Prepare a temporary result list to gather sub nodes.
            var resultList = PrepareResultList();

            // Check for the EndToken (e.g. ')') immediately after the start token.
            if (InTokens[RefStartIndex].Check(EndToken))
            {
                RefStartIndex++;
            }
            else
            {
                while (true)
                {
                    // If we reach the end of the token list and without finding the end token, throw an exception.
                    // -n getSth(InParam0, InParam1 X
                    //                              ^
                    if (RefStartIndex >= InTokens.Count)
                    {
                        throw new ParserException(ParserErrorType.AST_UnexpectedEnd);
                        break;
                    }

                    // Parse and add the sub node to the result list.
                    ParseAndAddSubSTNode(resultList, InTokens, ref RefStartIndex);

                    // Try to check for the separater token (e.g. ',').
                    // -n getSth(InParam0, InParam1)
                    //                   ^         ^
                    if (InTokens[RefStartIndex].Check(SeperatorToken))
                    {
                        // Consume the separater token.
                        ++RefStartIndex;
                    }
                    // or try to check for the end token (e.g. ')' ).
                    // -n getSth(InParam0, InParam1)
                    //                             ^
                    else if (InTokens[RefStartIndex].Check(EndToken))
                    {
                        // Consume the end token and break the loop.
                        ++RefStartIndex;
                        break;
                    }
                    else
                    {
                        // If neither a separator nor an end token is found, throw an exception.
                        throw new ParserException(
                            ParserErrorType.AST_UnexpectedToken
                            , InTokens[RefStartIndex]
                            , $"{SeperatorToken}|{EndToken}"
                            );
                    }
                } // ~ END While true
            } // ~ END else

            return FinishResultList(resultList);
        }

        /// <summary>
        /// Prepare a temporary result list to gather sub nodes.
        /// This method is called before `ParseAndAddSubSTNode` to create a temporary list that will be used to collect the parsed sub nodes.
        /// The temporary list is then passed to `FinishResultList` to generate the final result.
        /// </summary>
        /// <returns>A new instance of the result list.</returns>
        protected virtual TResultList PrepareResultList()
        {
            return new TResultList();
        }

        /// <summary>
        /// Parses tokens into a sub syntax tree node and adds it to the result list.
        /// </summary>
        /// <param name="InPreparedResultList">The prepared result list to which the sub node will be added.</param>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">The reference to the start index in the token list. This will be updated after parsing.</param>
        protected abstract void ParseAndAddSubSTNode(TResultList InPreparedResultList, IReadOnlyList<IToken> InTokens, ref int RefStartIndex);

        /// <summary>
        /// Gathers sub nodes from the temporary result list (prepared by the <see cref="PrepareResultList"/> method) and generates the final result.
        /// </summary>
        /// <param name="InPreparedResultList">The prepared result list containing the parsed sub nodes.</param>
        /// <returns>The final result as a syntax tree node.</returns>
        protected virtual T FinishResultList(TResultList InPreparedResultList)
        {
            return InPreparedResultList as T;
        }

    }


}