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
        public ASTParser_ExprOperator(OpCodeWithDef[] InOps, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            Ops = InOps;
        }
        public ASTParser_ExprOperator(OpCodeWithDef InOp, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            Ops = new OpCodeWithDef[] { InOp };
        }
        public ASTParser_ExprOperator(ETokenType InTokenType, OpCodeWithDef InOp, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            TokenType = InTokenType;
            Ops = new OpCodeWithDef[] { InOp };
        }
        public ASTParser_ExprOperator(ETokenType InTokenType, OpCodeWithDef[] InOps, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            TokenType = InTokenType;
            Ops = InOps;
        }

        /// <summary>
        /// Operators
        /// </summary>
        public OpCodeWithDef[] Ops { get; private set; }

        /// <summary>
        /// TokenType
        /// </summary>
        public ETokenType TokenType { get; private set; } = ETokenType.Operator;

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // Try parse lhs
            var lhs = NextParser.Parse(InTokenList);

            // Parse and consume 'op'
            OpDefinition opDef = null;
            while (InTokenList.CheckToken(TokenType)
                 && null != (opDef = OpCodeWithDef.FindDefByCode(Ops, InTokenList.CurrentToken.Code))
               )
            {
                // Save, consume, then step next.
                var opToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                // All 'Ops' must have the rhs.
                var rhs = NextParser.Parse(InTokenList);

                lhs = new syntaxtree.STNodeBinaryOp(opDef, lhs, rhs);
            }
            return lhs;
        }

    }


}