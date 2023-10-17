using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Binary operators like + - * / %
    /// </summary>
    public class STNodeBinaryOp
        : STNodeBase
    {
        internal STNodeBinaryOp()
        {
        }

        public STNodeBinaryOp(OpDefinition InOpDef, ISyntaxTreeNode InLhs, ISyntaxTreeNode InRhs)
        {
            Debug.Assert(InOpDef.Usage != EOpUsage.UnaryBooleanOperator && InOpDef.Usage != EOpUsage.UnaryOperator);
            OpDef = InOpDef;
            LHS = InLhs;
            RHS = InRhs;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc(LHS)) { return; }
            if (!InActionFunc(RHS)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            switch (OpDef.Usage)
            {
                case EOpUsage.Comparer:
                case EOpUsage.BooleanOperator:
                    return CommonTypeInfos.Boolean;
                case EOpUsage.LOperator:
                    return LHS.GetPredictType(InHostElemInfo);
                case EOpUsage.ROperator:
                    return RHS.GetPredictType(InHostElemInfo);
                case EOpUsage.BitwiseOperator:
                    return LHS.GetPredictType(InHostElemInfo);
            }
            return null;
        }

        /// <summary>
        /// The Operator Definition.
        /// </summary>
        [Serialization.SerializableInfo]
        public OpDefinition OpDef { get; private set; }

        /// <summary>
        /// Left hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode LHS { get; private set; }

        /// <summary>
        /// Right hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

    }

}
