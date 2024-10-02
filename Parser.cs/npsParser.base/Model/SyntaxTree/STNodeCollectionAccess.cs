using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// An expr-node to access sub elements in a collection.
    /// 
    /// The collection may have multiple keys, like 2D-array or 3D-array.
    /// 
    /// </summary>
    public class STNodeCollectionAccess
        : STNodeBase
    {
        internal STNodeCollectionAccess()
        {
        }

        public STNodeCollectionAccess(ISyntaxTreeNode InLhs, ISyntaxTreeNode InParam0)
        {
            CollExpr = InLhs;
            Params = new ISyntaxTreeNode[1] { InParam0 };
        }

        public STNodeCollectionAccess(ISyntaxTreeNode InLhs, ISyntaxTreeNode[] InParams)
        {
            CollExpr = InLhs;
            Params = InParams;
        }

        public STNodeCollectionAccess(ISyntaxTreeNode InLhs, IEnumerable<ISyntaxTreeNode> InParams)
        {
            CollExpr = InLhs;
            Params = InParams.ToArray();
        }

        public override void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            for (int i = 0; i < Params.Length; i++)
            {
                var param = Params[i];
                var key = $"Param{i}";
                if (!InActionFunc(key, param)) { return; }
            }
            if (!InActionFunc("CollExpr", CollExpr)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            var colType = CollExpr.GetPredictType(InHostElemInfo);

            TypeInfo[] paramTypes = new TypeInfo[Params.Length];
            for (int i = 0; i < Params.Length; i++)
            {
                paramTypes[i] = Params[i].GetPredictType(InHostElemInfo);
            }
            return colType.EvalCollectionElementType(paramTypes);
        }

        /// <summary>
        /// Expression to locate a collection.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode CollExpr { get; }

        /// <summary>
        /// Parameters
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode[] Params { get; private set; }

        // Begin object interfaces
        public override string ToString()
        {
            string paramStr = "";
            for (int i = 0; i < Params.Length; ++i)
            {
                if (i != 0)
                {
                    paramStr += ", ";
                }

                var param = Params[i];
                paramStr += param.ToString();
            }

            return $"CollectionAccess {{ CollExpr = {CollExpr}, Params = {paramStr} }}";
        }
        // ~ End object interfaces



    }

}
