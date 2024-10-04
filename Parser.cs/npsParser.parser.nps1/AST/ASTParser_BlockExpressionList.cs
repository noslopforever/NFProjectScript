using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse an expression list.
    /// 
    /// Example:
    /// a, b+c, d, 10/5
    /// 
    /// </summary>
    internal class ASTParser_BlockExpressionList
        : ASTParser_ChildListBase<STNodeSequence, List<ISyntaxTreeNode>>
    {

        /// <summary>
        /// Initialize a new instance of the <see cref="ASTParser_BlockExpressionList"/> class.
        /// </summary>
        /// <param name="InStartToken">The token that marks the start of the expression list.</param>
        /// <param name="InEndToken">The token that marks the end of the expression list.</param>
        public ASTParser_BlockExpressionList(string InStartToken, string InEndToken)
            : base(InStartToken, InEndToken, CommonTokenTypes.Comma)
        {
        }

        /// <inheritdoc />
        protected override STNodeSequence FinishResultList(List<ISyntaxTreeNode> InPreparedResultList)
        {
            return new STNodeSequence(InPreparedResultList.ToArray());
        }

        /// <inheritdoc />
        protected override void ParseAndAddSubSTNode(List<ISyntaxTreeNode> InPreparedResultList, IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Let the expression parser to parse each expression.
            // (a + b, 10 / 5, "" )
            //  ^---^  ^----^  ^^
            ASTParser_Expression exprParser = new ASTParser_Expression();
            var exprStartToken = InTokens[RefStartIndex];
            var expr = exprParser.Parse(InTokens, ref RefStartIndex);
            if (expr == null)
            {
                // If expression parsing failed, throw a parser exception.
                throw new ParserException(
                    ParserErrorType.AST_InvalidExpression
                    , exprStartToken
                    );
            }

            // Add the parsed expression to the result list.
            InPreparedResultList.Add(expr);
        }

    }

    ///// <summary>
    ///// Parser to parse an expression list like:
    ///// a, b+c, d, 10/5
    ///// </summary>
    //internal class ASTParser_BlockExpressionList
    //    : ASTParser_Base<STNodeSequence>
    //{
    //    public ASTParser_BlockExpressionList(string InStartToken, string InEndToken)
    //    {
    //        StartToken = InStartToken;
    //        EndToken = InEndToken;
    //    }

    //    /// <summary>
    //    /// Start token which indicates an expression list
    //    /// </summary>
    //    public string StartToken { get; }

    //    /// <summary>
    //    /// End token which indicates the end of expression list.
    //    /// </summary>
    //    public string EndToken { get; }

    //    public override STNodeSequence Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
    //    {
    //        if (InTokens[RefStartIndex].Check(StartToken))
    //        {
    //            // Consume the StartToken.
    //            RefStartIndex++;

    //            // Define the result expression list.
    //            List<ISyntaxTreeNode> exprList = new List<ISyntaxTreeNode>();

    //            // Handle the EndToken after the StartToken
    //            // ( )
    //            //   ^
    //            if (InTokens[RefStartIndex].Check(EndToken))
    //            {
    //                RefStartIndex++;
    //            }
    //            else
    //            {
    //                // Handle expression list
    //                // (a + b, 10 / 5, "" )
    //                //  ^---------------^

    //                while (true)
    //                {
    //                    // If matches the end of the TL, failed and break.
    //                    if (InTokens.Count == RefStartIndex)
    //                    {
    //                        throw new ParserException(ParserErrorType.AST_UnexpectedEnd);
    //                    }

    //                    // Let the expression parser to parse each expression.
    //                    // (a + b, 10 / 5, "" )
    //                    //  ^---^  ^----^  ^^
    //                    ASTParser_Expression exprParser = new ASTParser_Expression();
    //                    var exprStartToken = InTokens[RefStartIndex];
    //                    var expr = exprParser.Parse(InTokens, ref RefStartIndex);
    //                    if (expr == null)
    //                    {
    //                        // If expr parse failed, throw error
    //                        throw new ParserException(
    //                            ParserErrorType.AST_InvalidExpression
    //                            , exprStartToken
    //                            );
    //                    }
    //                    exprList.Add(expr);

    //                    // (a + b, 10 / 5, "" )
    //                    //       ^       ^    ^
    //                    if (InTokens[RefStartIndex].Check(CommonTokenTypes.Comma))
    //                    {
    //                        // Consume the ','
    //                        RefStartIndex++;
    //                    }
    //                    else if (InTokens[RefStartIndex].Check(EndToken))
    //                    {
    //                        // (a + b, 10 / 5, "" )
    //                        //                    ^

    //                        // Found the EndToken, break the loop and exit.
    //                        RefStartIndex++;
    //                        break;
    //                    }
    //                    else
    //                    {
    //                        // (a+b X 10/5, "" X )
    //                        //      ^          ^
    //                        // Found Unexpected Token, throw exception.
    //                        throw new ParserException(
    //                            ParserErrorType.AST_UnexpectedToken
    //                            , InTokens[RefStartIndex]
    //                            , $"{CommonTokenTypes.Comma}|{EndToken}"
    //                            );
    //                    }
    //                } // ~ END while (true)
    //            } // ~ END else 

    //            return new syntaxtree.STNodeSequence(exprList.ToArray());
    //        }

    //        return null;
    //    }

    //}

}