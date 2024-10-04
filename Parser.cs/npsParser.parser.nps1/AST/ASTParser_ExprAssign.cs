using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse an assign operator like 'A = B'.
    /// </summary>
    class ASTParser_ExprAssign
        : ASTParser_ExprBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprAssign"/> class with the default assignment operator.
        /// For more details on the lower priority parser, see the <see cref="LowerPriorityParser"/> property.
        /// </summary>
        /// <param name="InLowerPriorityParser">The lower priority parser to be used for parsing sub-expressions.</param>
        public ASTParser_ExprAssign(ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
            Ops = new string[] { "=" };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprAssign"/> class with custom assignment operators.
        /// For more details on the lower priority parser, see the <see cref="ASTParser_ExprBase.LowerPriorityParser"/> property.
        /// </summary>
        /// <param name="InAssignCodes">The custom assignment operators to use.</param>
        /// <param name="InLowerPriorityParser">The lower priority parser to be used for parsing sub-expressions.</param>
        public ASTParser_ExprAssign(string[] InAssignCodes, ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
            Ops = InAssignCodes;
        }

        /// <summary>
        /// Get the operators that this parser can handle.
        /// </summary>
        public string[] Ops { get; private set; }

        /// <summary>
        /// Gets the token type that this parser is designed to handle.
        /// </summary>
        public string TokenType { get; private set; } = CommonTokenTypes.Operator;

        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try to parse the LHS of the expression using the lower priority parser.
            var lhs = LowerPriorityParser.Parse(InTokens, ref RefStartIndex);

            // List to store the operator and corresponding RHS nodes.
            List<(string, ISyntaxTreeNode)> opAndRhsList = new List<(string, ISyntaxTreeNode)>();

            // This while loop processes multiple assignment statements in a chain, such as "a = b = c = d".
            // It continues to parse and add RHS expressions until all assignment operators are processed.
            var opToken = InTokens[RefStartIndex];
            while (InTokens[RefStartIndex].Check(TokenType)
                && Ops.Contains(opToken.Code)
                )
            {
                RefStartIndex++;

                // Parse the RHS of the expression using the lower priority parser.
                var rhs = LowerPriorityParser.Parse(InTokens, ref RefStartIndex);

                // Add the operator and RHS to the list.
                opAndRhsList.Add((opToken.Code, rhs));
            }

            // If no assignment operator is found, return the LHS immediately.
            if (opAndRhsList.Count == 0)
            {
                return lhs;
            }

            // Insert the LHS as the first entry in the list.
            opAndRhsList.Insert(0, ("ERROR", lhs));

            // Create the assignment or compound assignment nodes from right to left.
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