using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace nf.protoscript.syntaxtree
{


    /// <summary>
    /// Unary operators like +1 -1
    /// </summary>
    public class STNodeUnaryOp
        : STNodeBase
    {
        public STNodeUnaryOp(OpDefinition InOpDef, ISyntaxTreeNode InRhs)
        {
            Debug.Assert(InOpDef.Usage == EOpUsage.UnaryBooleanOperator || InOpDef.Usage == EOpUsage.UnaryOperator);
            OpDef = InOpDef;
            RHS = InRhs;
        }

        public override void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc("RHS", RHS)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            switch (OpDef.Usage)
            {
                case EOpUsage.UnaryBooleanOperator:
                    return CommonTypeInfos.Boolean;
                case EOpUsage.UnaryOperator:
                    return RHS.GetPredictType(InHostElemInfo);
            }
            return null;
        }

        /// <summary>
        /// Operator character
        /// </summary>
        [Serialization.SerializableInfo]
        public OpDefinition OpDef { get; private set; }

        /// <summary>
        /// Right-hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

        /// <summary>
        /// String code of the operator.
        /// </summary>
        public string OpCode { get { return OpDef.OpCode; } }

        // Begin object interfaces
        public override string ToString()
        {
            return $"UnaryOp {{ Op = {OpCode}, RHS = {RHS} }}";
        }
        // ~ End object interfaces


    }

}
