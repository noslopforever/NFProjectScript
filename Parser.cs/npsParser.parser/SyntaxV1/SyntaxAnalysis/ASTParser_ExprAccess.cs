using nf.protoscript.parser.token;
using System;
using System.Linq;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Access operator parser.
    /// </summary>
    /// <example>
    /// A.B, Pos.X, Role.HP
    /// </example>
    class ASTParser_ExprAccess
        : ASTParser_ExprOperator
    {
        public ASTParser_ExprAccess(ASTParser_ExprBase InNextExprParser)
            : base(".", InNextExprParser)
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

                syntaxtree.STNodeSub op = new syntaxtree.STNodeSub(lhs, rhs);
                return op;
            }
            return lhs;
        }

    }


}