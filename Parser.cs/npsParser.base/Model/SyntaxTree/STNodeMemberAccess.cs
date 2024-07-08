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

        public STNodeMemberAccess(ISyntaxTreeNode InLhs, string InIDName)
        {
            LHS = InLhs;
            IDName = InIDName;
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
            var info = lhsPredType.FindTheFirstSubInfoWithName<ElementInfo>(IDName);
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
        public string IDName { get; private set; }

        // Begin object interfaces
        public override string ToString()
        {
            return $"MemberAccess {{ IDName = {IDName}, LHS = {LHS} }}";
        }
        // ~ End object interfaces

    }
}