﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        public STNodeCollectionAccess(STNodeBase InLhs, ISyntaxTreeNode InParam0)
        {
            CollExpr = InLhs;
            Params = new ISyntaxTreeNode[1] { InParam0 };
        }

        public STNodeCollectionAccess(STNodeBase InLhs, ISyntaxTreeNode[] InParams)
        {
            CollExpr = InLhs;
            Params = InParams;
        }

        public STNodeCollectionAccess(STNodeBase InLhs, IEnumerable<ISyntaxTreeNode> InParams)
        {
            CollExpr = InLhs;
            Params = InParams.ToArray();
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            foreach (var param in Params)
            {
                if (!InActionFunc(param)) { return; }
            }
            if (!InActionFunc(CollExpr)) { return; }
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


    }

}
