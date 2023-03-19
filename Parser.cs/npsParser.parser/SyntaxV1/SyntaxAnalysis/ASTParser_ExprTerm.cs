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
            // < Number > | < ID > | (EXPRs) | [EXPRs] | { STATEMENT }
            // EXPR (EXPRS) | EXPR [EXPRS]

            // <ID>
            if (InTokenList.CheckToken(ETokenType.ID))
            {
                Token idToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                return ParseTermWithID(idToken, InTokenList);
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
                syntaxtree.STNodeConstant stConst = new syntaxtree.STNodeConstant(float.Parse(InTokenList.CurrentToken.Code));
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
            // Collection: (EXPRs)
            else if (InTokenList.CheckToken(ETokenType.OpenParen))
            {
                throw new NotImplementedException();
                //InTokenList.Consume();

                //List<model.Expression> exprs = new List<model.Expression>();
                //ASTParser.ParseExpressions(exprs, InTokenList, ETokenType.CloseParen);
                //syntaxtree.STNodeBaseConstColl exprList = new syntaxtree.STNodeBaseConstColl(exprs);
                //if (!InTokenList.EnsureConsumeTheToken(ETokenType.CloseParen))
                //{
                //    // TODO log error
                //    throw new NotImplementedException();
                //}
                //return exprList;
            }
            // Collection: [ EXPRs ]
            else if (InTokenList.CheckToken(ETokenType.OpenBracket))
            {
                throw new NotImplementedException();
                //InTokenList.Consume();

                //List<model.Expression> exprs = new List<model.Expression>();
                //ASTParser.ParseExpressions(exprs, InTokenList, ETokenType.CloseBracket);
                //syntaxtree.STNodeSequence exprList = new syntaxtree.STNodeSequence(exprs);
                //if (!InTokenList.EnsureConsumeTheToken(ETokenType.CloseBracket))
                //{
                //    // TODO log error
                //    throw new NotImplementedException();
                //}
                //return exprList;
            }
            // Const Dict: { STATEMENT }
            else if (InTokenList.CheckToken(ETokenType.OpenBrace))
            {
                throw new NotImplementedException();
                //InTokenList.Consume();

                //List<syntaxtree.STNodeBaseAssign> assignments = new List<syntaxtree.STNodeBaseAssign>();
                //ASTParser.ParseAssignments(InTokenList, assignments, ETokenType.CloseBrace);

                //syntaxtree.STNodeBaseConstDict dict = new syntaxtree.STNodeBaseConstDict();
                //dict.SetAssignments(assignments);
                //if (!InTokenList.EnsureConsumeTheToken(ETokenType.CloseBrace))
                //{
                //}
                //return dict;
            }

            // TODO log error: "Unexpected token "
            throw new NotImplementedException();
            return null;
        }

        private syntaxtree.STNodeBase ParseTermWithID(Token InIDToken, TokenList InTokenList)
        {
            // <ID> (EXPRs)
            if (InTokenList.CheckToken(ETokenType.OpenParen))
            {
                throw new NotImplementedException();
                //var exprListParser = new ASTParser_ExpressionList(ETokenType.OpenParen, ETokenType.CloseParen);
                //var exprList = exprListParser.Parse(InTokenList);
                //if (exprList == null)
                //{
                //    return null;
                //}

                //var exprSTNodeList = ParseHelper.ConvertExprListToSTNodeList(exprList);
                //syntaxtree.STNodeCall call = new syntaxtree.STNodeCall(InIDToken.Code, exprSTNodeList.ToArray());
                //return call;
            }
            // <ID> [EXPRs]
            else if (InTokenList.CheckToken(ETokenType.OpenBracket))
            {
                throw new NotImplementedException();
                //var exprListParser = new ASTParser_ExpressionList(ETokenType.OpenBracket, ETokenType.CloseBracket);
                //var exprList = exprListParser.Parse(InTokenList);
                //if (exprList == null)
                //{
                //    return null;
                //}

                //var exprSTNodeList = ParseHelper.ConvertExprListToSTNodeList(exprList);
                //syntaxtree.STNodeAccessCollection accessColl = new syntaxtree.STNodeAccessCollection(InIDToken.Code, exprSTNodeList.ToArray());
                //return accessColl;
            }

            return new syntaxtree.STNodeGetVar(InIDToken.Code);
        }

    }


}