using System;
using System.Collections.Generic;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// STNode sequence.
    /// 
    /// local a, b, c = 0;
    /// a = b + c;
    /// return c;
    /// 
    /// </summary>
    public class STNodeSequence : STNodeBase
    {
        internal STNodeSequence()
        {
        }

        public STNodeSequence(params ISyntaxTreeNode[] InSTNodes)
        {
            NodeList = InSTNodes;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            foreach (var node in NodeList)
            {
                if (!InActionFunc(node)) { return; }
            }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            // Predict method type by returns from nodes.

            List<STNode_Return> returns = new List<STNode_Return>();
            ForeachSubNodes(node => { GatherReturns(returns, node); return true; });
            var retTypes = GatherReturnTypes(InHostElemInfo, returns);
            TypeInfo commonType = PredictCommonBaseTypeFromTypes(retTypes);
            return commonType;
        }

        /// <summary>
        /// Sub STNode list.
        /// </summary>
        [Serialization.SerializableInfo]
        public IReadOnlyList<ISyntaxTreeNode> NodeList { get; private set; }


        /// <summary>
        /// Predict the common base type from InTypes.
        /// </summary>
        /// <param name="InTypes"></param>
        /// <returns>May be null, which means there is no valid common base type of all InTypes/</returns>
        static TypeInfo PredictCommonBaseTypeFromTypes(IEnumerable<TypeInfo> InTypes)
        {
            bool onlyOnce = false;
            TypeInfo checkingBaseType = null;
            foreach (var type in InTypes)
            {
                // Do once
                if (!onlyOnce)
                {
                    checkingBaseType = type;
                    onlyOnce = true;
                    continue;
                }

                // If the checking type is not a common-base of another type, try select a new common-base from upper levels.
                // e.g. B0 <- B1, B1 <- B10, B1 <- B11, checking: B10,
                // Because the B11 is not derived from B10, so we must select B1 (B10's base) as the new checking type.
                while (checkingBaseType != null)
                {
                    if (!checkingBaseType.IsSameOrDerivedOf(type))
                    {
                        checkingBaseType = checkingBaseType.BaseType;
                    }
                }
            }
            return checkingBaseType;
        }
        /// <summary>
        /// Gather all return types from all return nodes.
        /// </summary>
        /// <param name="InHostElementInfo"></param>
        /// <param name="InReturns"></param>
        /// <returns></returns>
        static List<TypeInfo> GatherReturnTypes(ElementInfo InHostElementInfo, IEnumerable<STNode_Return> InReturns)
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
        /// Gather all return nodes.
        /// </summary>
        /// <param name="InSTNodes"></param>
        /// <returns></returns>
        static List<STNode_Return> GatherReturns(IEnumerable<ISyntaxTreeNode> InSTNodes)
        {
            List<STNode_Return> results = new List<STNode_Return>();
            GatherReturns(results, InSTNodes);
            return results;
        }
        /// <summary>
        /// Gather all return nodes and save them in OutRetNodes.
        /// </summary>
        /// <param name="OutRetNodes"></param>
        /// <param name="InSTNodes"></param>
        static void GatherReturns(List<STNode_Return> OutRetNodes, IEnumerable<ISyntaxTreeNode> InSTNodes)
        {
            foreach (var node in InSTNodes)
            {
                GatherReturns(OutRetNodes, node);
            }
        }
        /// <summary>
        /// Gather all return nodes and save them in OutRetNodes.
        /// </summary>
        /// <param name="OutRetNodes"></param>
        /// <param name="InSTNodes"></param>
        static void GatherReturns(List<STNode_Return> OutRetNodes, ISyntaxTreeNode InSTNode)
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

    }

}
