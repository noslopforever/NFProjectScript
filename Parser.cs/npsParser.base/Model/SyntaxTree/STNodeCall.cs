using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// An expr-node to call some method.
    /// </summary>
    public class STNodeCall
        : STNodeBase
    {
        internal STNodeCall()
        {
        }

        public STNodeCall(STNodeBase InLhs)
        {
            FuncExpr = InLhs;
        }

        public STNodeCall(STNodeBase InLhs, ISyntaxTreeNode InParam0)
        {
            FuncExpr = InLhs;
            Params = new ISyntaxTreeNode[1] { InParam0 };
        }

        public STNodeCall(STNodeBase InLhs, ISyntaxTreeNode[] InParams)
        {
            FuncExpr = InLhs;
            Params = InParams;
        }

        public STNodeCall(STNodeBase InLhs, IEnumerable<ISyntaxTreeNode> InParams)
        {
            FuncExpr = InLhs;
            Params = InParams.ToArray();
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            foreach (var param in Params)
            {
                if (!InActionFunc(param)) { return; }
            }
            if (!InActionFunc(FuncExpr)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            var type = FuncExpr.GetPredictType(InHostElemInfo);
            var delType = type as DelegateTypeInfo;
            if (delType != null)
            {
                return delType.ReturnType;
            }
            return null;
        }

        /// <summary>
        /// Function name
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode FuncExpr { get; private set; }

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

            return $"Call {{ FuncExpr = {FuncExpr}, Params = {paramStr} }}";
        }
        // ~ End object interfaces

    }

}
