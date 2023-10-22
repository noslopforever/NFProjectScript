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
            ForeachSubNodes(node => { STNode_Return.GatherReturns(returns, node); return true; });
            if (returns.Count == 0)
            {
                return null;
            }

            // TODO return the signature, not the return value's type.
            //throw new NotImplementedException();

            var retTypes = STNode_Return.GatherReturnTypes(InHostElemInfo, returns);
            TypeInfo commonType = TypeInfo.PredictCommonBaseTypeFromTypes(retTypes);
            return commonType;
        }

        /// <summary>
        /// Sub STNode list.
        /// </summary>
        [Serialization.SerializableInfo]
        public IReadOnlyList<ISyntaxTreeNode> NodeList { get; private set; }

    }

}
