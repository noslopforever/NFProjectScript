using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Assign operator parser. A = B.
    /// </summary>
    class ASTParser_ExprAssign
        : ASTParser_ExprBase
    {
        public ASTParser_ExprAssign(ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            TokenType = ETokenType.Assign;
            Ops = new string[] { "=" };
        }

        public ASTParser_ExprAssign(string[] InAssignCodes, ASTParser_ExprBase InNextExprParser)
            : base(InNextExprParser)
        {
            Ops = InAssignCodes;
            TokenType = ETokenType.Assign;
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
            var opToken = InTokenList.CurrentToken;

            // op and rhs list.
            List<(string, syntaxtree.STNodeBase)> opAndRhsList = new List<(string, syntaxtree.STNodeBase)>();

            while (InTokenList.CheckToken(TokenType)
                && Ops.Contains(opToken.Code)
                )
            {
                InTokenList.Consume();

                // All 'Ops' have rhs.
                var rhs = NextParser.Parse(InTokenList);

                opAndRhsList.Add((opToken.Code, rhs));
            }

            // No assign op, return lhs immediately.
            if (opAndRhsList.Count == 0)
            {
                return lhs;
            }

            // Insert lhs to the first entry
            opAndRhsList.Insert(0, ("ERROR", lhs));

            // Create STNodeAssign from right to left.
            //
            // Step0:
            // LHS, E0, E1, E2
            //          ^   ^
            //          i   lastOp/STNode
            // O2(E1, E2)
            //
            // Step1:
            // LHS, E0, E1, E2
            //      ^   ^
            //      i   lastOp/STNode
            // O1(E0, O2(E1, E2))
            //
            // Step END:
            // LHS, E0, E1, E2
            // ^    ^
            // i    lastOp/STNode
            // O0(LHS, O1(E0, E1))
            //
            var lastOpCode = opAndRhsList[opAndRhsList.Count - 1].Item1;
            var lastSTNode = opAndRhsList[opAndRhsList.Count - 1].Item2;
            for (int i = opAndRhsList.Count - 2; i >= 0; i--)
            {
                var lhsSTNode = opAndRhsList[i].Item2;

                if (opToken.Code == "=")
                {
                    lastSTNode = new syntaxtree.STNodeAssign(lhsSTNode, lastSTNode);
                }
                else
                {
                    lastSTNode = new syntaxtree.STNodeCompoundAssign(opToken.Code, lhsSTNode, lastSTNode);
                }

                lastOpCode = opAndRhsList[i].Item1;
            }

            return lastSTNode;
        }

    }


}