using System;
using System.Linq;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Operator handler.
    /// </summary>
    class ASTParser_ExprOperator : ASTParser_ExprBase
    {
        public ASTParser_ExprOperator(string[] InOps, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            Ops = InOps;
        }
        public ASTParser_ExprOperator(string InOp, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            Ops = new string[] { InOp };
        }
        public ASTParser_ExprOperator(ETokenType InTokenType, string InOp, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            Ops = new string[] { InOp };
            TokenType = InTokenType;
        }

        public string[] Ops { get; private set; }

        public ETokenType TokenType { get; private set; } = ETokenType.Operator;

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // Try parse lhs
            var lhs = NextParser.Parse(InTokenList);

            // Parse and consume 'op'
            if (InTokenList.CheckToken(TokenType)
                && Ops.Contains(InTokenList.CurrentToken.Code)
                )
            {
                // Save, consume, then step next.
                var opToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                // All 'Ops' must have the rhs.
                var rhs = NextParser.Parse(InTokenList);

                syntaxtree.STNodeBinaryOp op = new syntaxtree.STNodeBinaryOp(opToken.Code, lhs, rhs);
                return op;
            }
            return lhs;
        }

    }


}