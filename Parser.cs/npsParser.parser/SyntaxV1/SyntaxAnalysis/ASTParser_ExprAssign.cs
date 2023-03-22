using nf.protoscript.parser.token;
using System;
using System.Linq;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Assign operator parser. A = B.
    /// </summary>
    class ASTParser_ExprAssign
        : ASTParser_ExprOperator
    {
        public ASTParser_ExprAssign(ASTParser_ExprBase InNextExprParser)
            : base(ETokenType.Assign, "=", InNextExprParser)
        {
        }

        public ASTParser_ExprAssign(string[] InAssignCodes, ASTParser_ExprBase InNextExprParser)
            : base(ETokenType.Assign, InAssignCodes, InNextExprParser)
        {
        }

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            // Try parse lhs
            var lhs = NextParser.Parse(InTokenList);

            // Parse and consume 'op'
            var opToken = InTokenList.CurrentToken;
            if (InTokenList.CheckToken(TokenType)
                && Ops.Contains(opToken.Code)
                )
            {
                InTokenList.Consume();

                // All 'Ops' have rhs.
                var rhs = NextParser.Parse(InTokenList);

                // Create Assign nodes instead of op nodes.
                if (opToken.Code == "=")
                {
                    syntaxtree.STNodeAssign op = new syntaxtree.STNodeAssign(lhs, rhs);
                    return op;
                }
                else
                {
                    syntaxtree.STNodeCompoundAssign op = new syntaxtree.STNodeCompoundAssign(opToken.Code, lhs, rhs);
                    return op;
                }
            }
            return lhs;
        }

    }


}