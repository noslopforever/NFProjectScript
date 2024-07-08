using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Statement: While.
    /// </summary>
    class STNode_StatementWhile
        : syntaxtree.ISyntaxTreeNode
    {
        /// <summary>
        /// If "do...while (condition)", the property returns true.
        /// If "while (condition) do ...", the property returns false.
        /// </summary>
        public bool DoWhileOrWhile { get; } = false;

        /// <summary>
        /// Expression of the condition.
        /// </summary>
        public syntaxtree.STNodeBase ConditionExpr { get; private set; }

        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc("ConditionExpr", ConditionExpr)) { return; }
        }

        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return null;
        }

    }

}
