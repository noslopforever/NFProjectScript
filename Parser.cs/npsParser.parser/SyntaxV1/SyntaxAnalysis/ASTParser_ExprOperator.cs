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
            TokenType = InTokenType;
            Ops = new string[] { InOp };
        }
        public ASTParser_ExprOperator(ETokenType InTokenType, string[] InOps, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            TokenType = InTokenType;
            Ops = InOps;
        }

        /// <summary>
        /// Operators
        /// </summary>
        public string[] Ops { get; private set; }

        /// <summary>
        /// TokenType
        /// </summary>
        public ETokenType TokenType { get; private set; } = ETokenType.Operator;

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // Try parse lhs
            var lhs = NextParser.Parse(InTokenList);

            // Parse and consume 'op'
            while (InTokenList.CheckToken(TokenType)
                && Ops.Contains(InTokenList.CurrentToken.Code)
                )
            {
                // Save, consume, then step next.
                var opToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                // All 'Ops' must have the rhs.
                var rhs = NextParser.Parse(InTokenList);

                lhs = new syntaxtree.STNodeBinaryOp(opToken.Code, lhs, rhs);
            }
            return lhs;
        }

    }


}