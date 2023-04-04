using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// "TERM" expression handler. Always be the leaf of a handler-tree.
    /// </summary>
    class ASTParser_ExprTerm : ASTParser_ExprBase
    {
        public ASTParser_ExprTerm()
            : base(null)
        {
        }

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // < Number > | < ID > | (EXPR) | [EXPRs] | { EXPR_STATEMENT }
            // EXPR (EXPRS) | EXPR [EXPRS]

            // <ID>
            if (InTokenList.CheckToken(ETokenType.ID))
            {
                Token idToken = InTokenList.CurrentToken;
                var idNode = new syntaxtree.STNodeGetVar(idToken.Code);
                InTokenList.Consume();

                return TryParseFuncCallOrCollAccess(idNode, InTokenList);
            }
            // <Number>
            else if (InTokenList.CheckToken(ETokenType.Integer))
            {
                syntaxtree.STNodeConstant stConst = new syntaxtree.STNodeConstant(int.Parse(InTokenList.CurrentToken.Code));
                InTokenList.Consume();
                return stConst;
            }
            else if (InTokenList.CheckToken(ETokenType.Floating))
            {
                syntaxtree.STNodeConstant stConst = new syntaxtree.STNodeConstant(double.Parse(InTokenList.CurrentToken.Code));
                InTokenList.Consume();
                return stConst;
            }
            // <String>
            else if (InTokenList.CheckToken(ETokenType.String))
            {
                syntaxtree.STNodeConstant stConst = new syntaxtree.STNodeConstant(InTokenList.CurrentToken.Code);
                InTokenList.Consume();
                return stConst;
            }

            // paren-blocks: (EXPR)
            // c * (a + b)
            //     ^-----^
            else if (InTokenList.CheckToken(ETokenType.OpenParen))
            {
                // Consume the open-paren (
                InTokenList.Consume();

                // Handle inner expression.
                ASTParser_Expression exprParser = new ASTParser_Expression();
                var subExpr = exprParser.Parse(InTokenList);

                // Try consume the close-paren ).
                if (!InTokenList.EnsureOrConsumeTo(ETokenType.CloseParen))
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokenList.SourceCodeLine
                        , InTokenList.CurrentToken
                        , ETokenType.CloseParen.ToString()
                        );
                }
                InTokenList.Consume();

                return subExpr;
            }
            // Collection: [ EXPRs ]
            else if (InTokenList.CheckToken(ETokenType.OpenBracket))
            {
                ASTParser_BlockExpressionList exprListParser = new ASTParser_BlockExpressionList(ETokenType.OpenBracket, ETokenType.CloseBracket);
                syntaxtree.STNodeSequence exprList = exprListParser.Parse(InTokenList);

                return exprList;
            }
            // Const Dict: { STATEMENT }
            else if (InTokenList.CheckToken(ETokenType.OpenBrace))
            {
                // TODO impl
                throw new NotImplementedException();
                //InTokenList.Consume();

                //List<syntaxtree.STNodeBaseAssign> assignments = new List<syntaxtree.STNodeBaseAssign>();
                //ASTParser.ParseAssignments(InTokenList, assignments, ETokenType.CloseBrace);

                //syntaxtree.STNodeBaseConstDict dict = new syntaxtree.STNodeBaseConstDict();
                //dict.SetAssignments(assignments);
                //if (!InTokenList.EnsureOrConsumeTo(ETokenType.CloseBrace))
                //{
                //}
                //return dict;
            }

            throw new ParserException(
                ParserErrorType.AST_UnexpectedTermToken
                , InTokenList.SourceCodeLine
                , InTokenList.CurrentToken
                );
        }

        private syntaxtree.STNodeBase TryParseFuncCallOrCollAccess(syntaxtree.STNodeBase InPreSTNode, TokenList InTokenList)
        {
            // <ID> (EXPRs)(EXPRs)(EXPRs)
            if (InTokenList.CheckToken(ETokenType.OpenParen))
            {
                var exprListParser = new ASTParser_BlockExpressionList(ETokenType.OpenParen, ETokenType.CloseParen);
                var stnodeSeq = exprListParser.Parse(InTokenList);
                if (stnodeSeq == null)
                {
                    return null;
                }

                var call = new syntaxtree.STNodeCall(InPreSTNode, stnodeSeq.NodeList);
                return TryParseFuncCallOrCollAccess(call, InTokenList);
            }
            // <ID> [EXPRs][EXPRs][EXPRs]
            else if (InTokenList.CheckToken(ETokenType.OpenBracket))
            {
                var exprListParser = new ASTParser_BlockExpressionList(ETokenType.OpenBracket, ETokenType.CloseBracket);
                var stnodeSeq = exprListParser.Parse(InTokenList);
                if (stnodeSeq == null)
                {
                    return null;
                }

                var accessColl = new syntaxtree.STNodeCollectionAccess(InPreSTNode, stnodeSeq.NodeList);
                return TryParseFuncCallOrCollAccess(accessColl, InTokenList);
            }

            return InPreSTNode;
        }

    }


}