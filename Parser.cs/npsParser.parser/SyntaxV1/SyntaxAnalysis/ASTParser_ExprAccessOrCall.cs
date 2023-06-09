using nf.protoscript.parser.token;
using System;
using System.Linq;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Access/Collection-Access/Call parser.
    /// </summary>
    /// <example>
    /// A.B, Pos.X, Role.HP, Sqrt(100), Items[0, 1].
    /// A.B.foo(C.D.E, F)[0, G.H].I
    /// </example>
    class ASTParser_ExprAccessOrCall
        : ASTParser_ExprOperator
    {
        public ASTParser_ExprAccessOrCall(ASTParser_ExprBase InNextExprParser)
            : base(".", InNextExprParser)
        {
        }

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // Try parse lhs
            var lhs = NextParser.Parse(InTokenList);

            // Parse : SUB, CALL, COLL
            while (true)
            {
                // Handle SUB: <Term> . <Term>
                if (InTokenList.CheckToken(ETokenType.Operator)
                    && Ops.Contains(InTokenList.CurrentToken.Code)
                    )
                {
                    InTokenList.Consume();

                    // All 'Ops' have rhs.
                    var rhs = NextParser.Parse(InTokenList);

                    var sub = new syntaxtree.STNodeSub(lhs, rhs);
                    lhs = sub;
                }
                // Handle CALL:  <Term> (EXPRs)
                else if (InTokenList.CheckToken(ETokenType.OpenParen))
                {
                    var exprListParser = new ASTParser_BlockExpressionList(ETokenType.OpenParen, ETokenType.CloseParen);
                    var stnodeSeq = exprListParser.Parse(InTokenList);
                    if (stnodeSeq == null)
                    {
                        return null;
                    }

                    var call = new syntaxtree.STNodeCall(lhs, stnodeSeq.NodeList);
                    lhs = call;
                }
                // Handle COLL: <Term> [EXPRs]
                else if (InTokenList.CheckToken(ETokenType.OpenBracket))
                {
                    var exprListParser = new ASTParser_BlockExpressionList(ETokenType.OpenBracket, ETokenType.CloseBracket);
                    var stnodeSeq = exprListParser.Parse(InTokenList);
                    if (stnodeSeq == null)
                    {
                        return null;
                    }

                    var accessColl = new syntaxtree.STNodeCollectionAccess(lhs, stnodeSeq.NodeList);
                    lhs = accessColl;
                }
                else
                {
                    break;
                }
            }
            return lhs;
        }

    }


}