using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Expr node : assign.
    /// </summary>
    public class STNodeAssign
        : STNodeBase
    {
        public STNodeAssign()
            : base()
        {
        }

        public STNodeAssign(ISyntaxTreeNode InLHS, ISyntaxTreeNode InRHS)
            : base()
        {
            LHS = InLHS;
            RHS = InRHS;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc(LHS)) { return; }
            if (!InActionFunc(RHS)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return LHS.GetPredictType(InHostElemInfo);
        }

        /// <summary>
        /// Left hand value
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode LHS { get; private set; }

        /// <summary>
        /// Right hand value
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

        // Begin object interfaces
        public override string ToString()
        {
            return $"Assign {{ LHS = {LHS}, RHS = {RHS} }}";
        }
        // ~ End object interfaces

    }

}
