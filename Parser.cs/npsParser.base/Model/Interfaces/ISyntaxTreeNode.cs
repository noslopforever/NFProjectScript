using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Syntax-tree node interface.
    /// 
    /// A syntax-tree will always be constructed after analysing the semantic of an expression.
    /// 
    /// </summary>
    public interface ISyntaxTreeNode
    {

        /// <summary>
        /// Sub SyntaxTreeNodes belongs to this node.
        /// </summary>
        /// <param name="InActionFunc">
        /// Return false to break the foreach loop.
        /// </param>
        void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc);

        /// <summary>
        /// Type predicted from this node.
        /// </summary>
        /// <param name="InHostElemInfo">
        /// Host Element of the expression.
        /// Member if the expression is an init-expression,
        /// or Method if the expression is an method expression.
        /// </param>
        TypeInfo GetPredictType(ElementInfo InHostElemInfo);

    }

}
