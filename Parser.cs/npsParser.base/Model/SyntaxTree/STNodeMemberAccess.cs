
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