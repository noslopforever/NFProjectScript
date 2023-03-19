using System;
using System.Linq;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Unary operation handler.
    /// </summary>
    class ASTParser_ExprUnary : ASTParser_ExprBase
    {
        public ASTParser_ExprUnary(string[] InUnaryOps, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            UnaryOps = InUnaryOps;
        }
        public string[] UnaryOps { get; private set; }

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // Try parse and consume Unary 'op'
            if (InTokenList.CheckToken(ETokenType.Operator)
                && UnaryOps.Contains(InTokenList.CurrentToken.Code)
                )
            {
                var opToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                // All 'Op's have rhs.
                var rhs = NextParser.Parse(InTokenList);

                syntaxtree.STNodeUnaryOp op = new syntaxtree.STNodeUnaryOp(opToken.Code, rhs);
                return op;
            }

            // No unary, next.
            return NextParser.Parse(InTokenList);
        }
    }


}