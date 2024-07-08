using System;
using System.Collections.Generic;
using System.Reflection.Emit;
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

            List<STNodeReturn> returns = new List<STNodeReturn>();
            ForeachSubNodes(node => { STNodeReturn.GatherReturns(returns, node); return true; });
            if (returns.Count == 0)
            {
                return null;
            }

            // TODO return the signature, not the return value's type.
            //throw new NotImplementedException();

            var retTypes = STNodeReturn.GatherReturnTypes(InHostElemInfo, returns);
            TypeInfo commonType = TypeInfo.PredictCommonBaseTypeFromTypes(retTypes);
            return commonType;
        }

        /// <summary>
        /// Sub STNode list.
        /// </summary>
        [Serialization.SerializableInfo]
        public IReadOnlyList<ISyntaxTreeNode> NodeList { get; private set; }

        // Begin object interfaces
        public override string ToString()
        {
            string subStr = "";
            for (int i = 0; i < NodeList.Count; ++i)
            {
                if (i != 0)
                {
                    subStr += ", ";
                }

                var sub = NodeList[i];
                subStr += sub.ToString();
            }

            return $"Sequence {{ NodeList = {subStr} }}";
        }
        // ~ End object interfaces


    }

}
