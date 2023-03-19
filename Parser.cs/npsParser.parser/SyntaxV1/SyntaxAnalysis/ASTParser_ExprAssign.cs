using nf.protoscript.parser.token;
using System;
using System.Linq;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Assign operator parser. A = B.
    /// </summary>
    class ASTParser_ExprAssign : ASTParser_ExprOperator
    {
        public ASTParser_ExprAssign(ASTParser_ExprBase InNextExprParser)
            : base(ETokenType.Assign, "=", InNextExprParser)
        {
        }
        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // Try parse lhs
            var lhs = NextParser.Parse(InTokenList);

            // Parse and consume 'op'
            if (InTokenList.CheckToken(TokenType)
                && Ops.Contains(InTokenList.CurrentToken.Code)
                )
            {
                InTokenList.Consume();

                // All 'Ops' have rhs.
                var rhs = NextParser.Parse(InTokenList);

                syntaxtree.STNodeAssign op = new syntaxtree.STNodeAssign(lhs, rhs);
                return op;
            }
            return lhs;
        }

    }


}