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
        /// Gather all return types from all return nodes.
        /// </summary>
        /// <param name="InHostElementInfo"></param>
        /// <param name="InReturns"></param>
        /// <returns></returns>
        public static List<TypeInfo> GatherReturnTypes(ElementInfo InHostElementInfo, IEnumerable<STNode_Return> InReturns)
        {
            List<TypeInfo> types = new List<TypeInfo>();
            foreach (var ret in InReturns)
            {
                var type = ret.GetPredictType(InHostElementInfo);
                types.Add(type);
            }
            return types;
        }

        /// <summary>
        /// Gather all return nodes in InSTNodes.
        /// </summary>
        /// <param name="InSTNodes"></param>
        /// <returns></returns>
        public static List<STNode_Return> GatherReturns(IEnumerable<ISyntaxTreeNode> InSTNodes)
        {
            List<STNode_Return> results = new List<STNode_Return>();
            GatherReturns(results, InSTNodes);
            return results;
        }

        /// <summary>
        /// Collect all return nodes to OutRetNodes.
        /// </summary>
        /// <param name="OutRetNodes"></param>
        /// <param name="InSTNodes"></param>
        public static void GatherReturns(List<STNode_Return> OutRetNodes, IEnumerable<ISyntaxTreeNode> InSTNodes)
        {
            foreach (var node in InSTNodes)
            {
                GatherReturns(OutRetNodes, node);
            }
        }
        /// <summary>
        /// Gather all return nodes from one syntax-tree and save results to the OutRetNodes.
        /// </summary>
        /// <param name="OutRetNodes"></param>
        /// <param name="InSTNodes"></param>
        public static void GatherReturns(List<STNode_Return> OutRetNodes, ISyntaxTreeNode InSTNode)
        {
            // save if the node is a return node.
            if (InSTNode is STNode_Return)
            {
                OutRetNodes.Add(InSTNode as STNode_Return);
            }

            // gather recursively
            InSTNode.ForeachSubNodes(node => 
            {
                GatherReturns(OutRetNodes, node);
                return true;
            }
            );
        }

        /// <summary>
        /// The expression pending to be returned.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode ReturnExpr { get; private set; }

    }

}
