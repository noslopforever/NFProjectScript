using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Represent a comment definition statement.
    /// </summary>
    [VirtualSTNode]
    class STNode_CommentDef
        : ISyntaxTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_CommentDef"/> class with no initial comment.
        /// </summary>
        internal STNode_CommentDef()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_CommentDef"/> class with the specified comment.
        /// </summary>
        /// <param name="InComment">The comment text.</param>
        public STNode_CommentDef(string InComment)
        {
            Comment = InComment;
        }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        public string Comment { get; set; }

        /// <inheritdoc />
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            // Comments do not have sub-nodes, so this method is empty.
        }

        /// <inheritdoc />
        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new InvalidProgramException();
        }

    }

}