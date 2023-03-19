namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Comment, to add descriptions to another syntax-node.
    /// </summary>
    class STNode_Comment
        : syntaxtree.ISyntaxTreeNode
    {
        
        /// <summary>
        /// Comment text.
        /// </summary>
        public string CommentText { get; private set; }

    }

}
