using System;
using System.Collections.Generic;

namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// Return statement, return an expression as the result of a method.
    /// </summary>
    public sealed class STNode_Return
        : STNodeBase
    {
        public STNode_Return(ISyntaxTreeNode InReturnExpr)
        {
            ReturnExpr = InReturnExpr;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc(ReturnExpr)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            if (ReturnExpr != null)
            {
                return ReturnExpr.GetPredictType(InHostElemInfo);
            }
            return null;
        }

        /// <summary>
        /// The expression pending to be returned.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode ReturnExpr { get; private set; }

    }

}
