using nf.protoscript.syntaxtree;
using System;

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

        public void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
        }

        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new InvalidProgramException();
        }
    }

}
