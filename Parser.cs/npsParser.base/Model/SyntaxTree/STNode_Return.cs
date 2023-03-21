namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// Return statement, return an expression as the result of a method.
    /// </summary>
    public class STNode_Return
        : STNodeBase
    {
        public STNode_Return(ISyntaxTreeNode InReturnExpr)
        {
            ReturnExpr = InReturnExpr;
        }

        /// <summary>
        /// The expression pending to be returned.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode ReturnExpr { get; private set; }

    }

}
