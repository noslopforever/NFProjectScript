using System;
using System.Collections.Generic;
using System.Linq;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Parser for operator expressions. Handles binary operators and constructs the corresponding syntax tree node.
    /// </summary>
    class ASTParser_ExprOperator
        : ASTParser_ExprBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a list of operators.
        /// For more details on the lower priority parser, see the <see cref="ASTParser_ExprBase.LowerPriorityParser"/> property.
        /// </summary>
        /// <param name="InOps">The list of operators to handle.</param>
        /// <param name="InLowerPriorityParser">The lower priority parser to be used for parsing sub-expressions.</param>
        public ASTParser_ExprOperator(OpCodeWithDef[] InOps, ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
            Ops = InOps;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a single operator.
        /// For more details on the lower priority parser, see the <see cref="ASTParser_ExprBase.LowerPriorityParser"/> property.
        /// </summary>
        /// <param name="InOp">The operator to handle.</param>
        /// <param name="InLowerPriorityParser">The lower priority parser to be used for parsing sub-expressions.</param>
        public ASTParser_ExprOperator(OpCodeWithDef InOp, ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
            Ops = new OpCodeWithDef[] { InOp };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a single operator and a custom token type.
        /// For more details on the lower priority parser, see the <see cref="ASTParser_ExprBase.LowerPriorityParser"/> property.
        /// </summary>
        /// <param name="InTokenType">The custom token type to use.</param>
        /// <param name="InOp">The operator to handle.</param>
        /// <param name="InLowerPriorityParser">The lower priority parser to be used for parsing sub-expressions.</param>
        public ASTParser_ExprOperator(string InTokenType, OpCodeWithDef InOp, ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
            TokenType = InTokenType;
            Ops = new OpCodeWithDef[] { InOp };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a list of operators and a custom token type.
        /// For more details on the lower priority parser, see the <see cref="ASTParser_ExprBase.LowerPriorityParser"/> property.
        /// </summary>
        /// <param name="InTokenType">The custom token type to use.</param>
        /// <param name="InOps">The list of operators to handle.</param>
        /// <param name="InLowerPriorityParser">The lower priority parser to be used for parsing sub-expressions.</param>
        public ASTParser_ExprOperator(string InTokenType, OpCodeWithDef[] InOps, ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
            TokenType = InTokenType;
            Ops = InOps;
        }

        /// <summary>
        /// Gets the operators that this parser can handle.
        /// </summary>
        public OpCodeWithDef[] Ops { get; private set; }

        /// <summary>
        /// Gets the token type that this parser is designed to handle.
        /// </summary>
        public string TokenType { get; private set; } = CommonTokenTypes.Operator;

        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try to parse the LHS of the expression using the lower priority parser.
            var lhs = LowerPriorityParser.Parse(InTokens, ref RefStartIndex);

            // Parse and consume the operator tokens
            OpDefinition opDef = null;
            while (InTokens[RefStartIndex].Check(TokenType)
                 && null != (opDef = OpCodeWithDef.FindDefByCode(Ops, InTokens[RefStartIndex].Code))
               )
            {
                // Save and consume the operator token, then step to the next.
                var opToken = InTokens[RefStartIndex];
                RefStartIndex++;

                // All 'Ops' must have the RHS.
                var rhs = LowerPriorityParser.Parse(InTokens, ref RefStartIndex);

                // Create a new binary operation node and update the LHS.
                lhs = new syntaxtree.STNodeBinaryOp(opDef, lhs, rhs);
            }
            return lhs;
        }

    }


}