
namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// Access sub properties of composite objects.
    /// </summary>
    public class STNodeSub : STNodeBase
    {
        internal STNodeSub()
        {
        }

        public STNodeSub(ISyntaxTreeNode InLhs, ISyntaxTreeNode InRhs)
        {
            LHS = InLhs;
            RHS = InRhs;
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
        public ISyntaxTreeNode RHS { get; private set; }

    }
}