using System;
using System.Collections.Generic;

namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// Access sub properties of composite objects.
    /// </summary>
    public class STNodeMemberAccess : STNodeBase
    {
        internal STNodeMemberAccess()
        {
        }

        public STNodeMemberAccess(ISyntaxTreeNode InLhs, string InMemberID)
        {
            LHS = InLhs;
            MemberID = InMemberID;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc(LHS)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            // Let the lhs to find the element.
            var lhsPredType = LHS.GetPredictType(InHostElemInfo);
            if (lhsPredType == null)
            {
                return null;
            }
            // Try find the element and retrieve its type.
            var info = lhsPredType.FindTheFirstSubInfoWithName<ElementInfo>(MemberID);
            if (info != null)
            {
                return info.ElementType;
            }
            return null;
        }

        /// <summary>
        /// Left hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode LHS { get; private set; }

        /// <summary>
        /// Right hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public string MemberID { get; private set; }

    }
}