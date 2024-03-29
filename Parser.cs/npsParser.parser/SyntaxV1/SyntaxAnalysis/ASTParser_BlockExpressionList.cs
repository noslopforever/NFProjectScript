﻿using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse an expression list like:
    /// a, b+c, d, 10/5
    /// </summary>
    internal class ASTParser_BlockExpressionList
        : ASTParser_Base<syntaxtree.STNodeSequence>
    {
        public ASTParser_BlockExpressionList(ETokenType InStartToken, ETokenType InEndToken)
        {
            StartToken = InStartToken;
            EndToken = InEndToken;
        }

        /// <summary>
        /// Start token which indicates an expression list
        /// </summary>
        public ETokenType StartToken { get; }

        /// <summary>
        /// End token which indicates the end of expression list.
        /// </summary>
        public ETokenType EndToken { get; }

        public override syntaxtree.STNodeSequence Parse(TokenList InTokenList)
        {
            if (InTokenList.CheckToken(StartToken))
            {
                // consume the StartToken.
                InTokenList.Consume();

                List<syntaxtree.STNodeBase> exprList = new List<syntaxtree.STNodeBase>();
                while (true)
                {
                    if (InTokenList.CheckToken(EndToken))
                    { break; }

                    // If matches the end of the TL, failed and break.
                    if (InTokenList.IsEnd)
                    {
                        break;
                    }

                    ASTParser_Expression exprParser = new ASTParser_Expression();
                    var exprStartToken = InTokenList.CurrentToken;
                    var expr = exprParser.Parse(InTokenList) as syntaxtree.STNodeBase;
                    if (expr == null)
                    {
                        // If expr parse failed, throw error
                        throw new ParserException(
                            ParserErrorType.AST_InvalidExpression
                            , exprStartToken
                            );
                    }
                    exprList.Add(expr);

                    // (a + b, 10 / 5)
                    //       ^       ^
                    if (!InTokenList.EnsureOrConsumeTo(new ETokenType[] { ETokenType.Comma, EndToken }))
                    {
                        throw new ParserException(
                            ParserErrorType.AST_UnexpectedToken
                            , InTokenList.CurrentToken
                            , $"{ETokenType.Comma}|{EndToken}"
                            );
                    }
                    if (InTokenList.CheckToken(ETokenType.Comma))
                    { InTokenList.Consume(); }
                }

                // Ensure and consume the EndToken.
                if (!InTokenList.EnsureOrConsumeTo(EndToken))
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokenList.CurrentToken
                        , EndToken
                        );
                }
                InTokenList.Consume();

                return new syntaxtree.STNodeSequence(exprList.ToArray());
            }

            return null;
        }

    }

}