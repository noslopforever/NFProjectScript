namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// Expr node : compound assign.
    /// </summary>
    public class STNodeCompoundAssign
        : STNodeBase
    {
        internal STNodeCompoundAssign()
            : base()
        {
        }

        public STNodeCompoundAssign(string InCompoundOp, ISyntaxTreeNode InLHS, ISyntaxTreeNode InRHS)
            : base()
        {
            CompoundOp = InCompoundOp;
            LHS = InLHS;
            RHS = InRHS;
        }

        /// <summary>
        /// Operator: += -= *= /= ...
        /// </summary>
        [Serialization.SerializableInfo]
        public string CompoundOp { get; private set; }

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

    }

}