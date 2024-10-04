using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser for unary operation expressions. Handles unary operators and constructs the corresponding syntax tree nodes.
    /// </summary>
    class ASTParser_ExprUnary : ASTParser_ExprBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprUnary"/> class with a list of unary operators.
        /// For more details on the lower priority parser, see the <see cref="ASTParser_ExprBase.LowerPriorityParser"/> property.
        /// </summary>
        /// <param name="InUnaryOps">The list of unary operators to handle.</param>
        /// <param name="InLowerPriorityParser">The lower priority parser to be used for parsing sub-expressions.</param>
        public ASTParser_ExprUnary(OpCodeWithDef[] InUnaryOps, ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
            UnaryOps = InUnaryOps;
        }

        /// <summary>
        /// Gets the unary operators that this parser can handle.
        /// </summary>
        public OpCodeWithDef[] UnaryOps { get; private set; }

        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try to parse and consume unary 'op'
            List<OpDefinition> unaryTokens = new List<OpDefinition>();
            OpDefinition opDef = null;
            while (InTokens[RefStartIndex].Check(CommonTokenTypes.Operator)
                && null != (opDef = OpCodeWithDef.FindDefByCode(UnaryOps, InTokens[RefStartIndex].Code))
                )
            {
                var opToken = InTokens[RefStartIndex];
                RefStartIndex++;

                // Add the parsed unary operator to the list.
                unaryTokens.Add(opDef);
            }

            // Parse the right-hand side (RHS) value using the lower priority parser.
            var rhs = LowerPriorityParser.Parse(InTokens, ref RefStartIndex);

            // Construct the unary operation nodes from the end of the list to the start.
            var lastNode = rhs;
            for (int i = unaryTokens.Count - 1; i >= 0; i--)
            {
                lastNode = new STNodeUnaryOp(unaryTokens[i], lastNode);
            }
            return lastNode;
        }
    }


}