using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Represents a 'while' or 'do...while' statement.
    /// </summary>
    [VirtualSTNode]
    class STNode_StatementWhile
        : ISyntaxTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_StatementWhile"/> class with the specified condition expression and loop type.
        /// </summary>
        /// <param name="InConditionExpr">The expression representing the condition for the loop.</param>
        /// <param name="InDoWhileOrWhile">Indicates whether the loop is a 'do...while' (true) or a 'while' (false).</param>
        public STNode_StatementWhile(ISyntaxTreeNode InConditionExpr, bool InDoWhileOrWhile)
        {
            ConditionExpr = InConditionExpr;
            DoWhileOrWhile = InDoWhileOrWhile;
        }

        /// <summary>
        /// Gets a value indicating whether the loop is a 'do...while' (true) or a 'while' (false).
        /// </summary>
        public bool DoWhileOrWhile { get; } = false;

        /// <summary>
        /// Gets the expression representing the condition for the loop.
        /// </summary>
        public ISyntaxTreeNode ConditionExpr { get; private set; }

        /// <inheritdoc />
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc("ConditionExpr", ConditionExpr)) { return; }
        }

        /// <inheritdoc />
        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return null;
        }

    }

}
