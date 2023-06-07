using System;
using System.Collections.Generic;
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
            List<string> unaryTokens = new List<string>();
            while (InTokenList.CheckToken(ETokenType.Operator)
                && UnaryOps.Contains(InTokenList.CurrentToken.Code)
                )
            {
                var opToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                unaryTokens.Add(opToken.Code);
            }

            // rhs value
            var rhs = NextParser.Parse(InTokenList);

            // parse unary operators from end to start.
            var lastNode = rhs;
            for (int i = unaryTokens.Count - 1; i >= 0; i--)
            {
                lastNode = new syntaxtree.STNodeUnaryOp(unaryTokens[i], lastNode);
            }
            return lastNode;
        }
    }


}