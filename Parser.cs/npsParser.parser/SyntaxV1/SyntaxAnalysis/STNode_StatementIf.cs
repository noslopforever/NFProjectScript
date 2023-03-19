﻿namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Statement: If.
    /// </summary>
    class STNode_StatementIf
        : syntaxtree.ISyntaxTreeNode
    {

        /// <summary>
        /// Expression of the condition.
        /// </summary>
        public syntaxtree.STNodeBase ConditionExpr { get; private set; }

    }

}
