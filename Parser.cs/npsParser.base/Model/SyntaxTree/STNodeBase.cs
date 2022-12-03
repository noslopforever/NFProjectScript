using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Syntax-tree node base.
    /// </summary>
    public class STNodeBase
        : ISyntaxTreeNode
    {
        public STNodeBase(string InSignature)
        {
            Signature = InSignature;
        }

        /// <summary>
        /// Signature (+,-,*,/) of the node. 
        /// </summary>
        public string Signature { get; protected set; }

        // Internal sub expr node, call DefSubExprNode to add entries to it.
        private Dictionary<string, STNodeBase> mSubExprNodes = new Dictionary<string, STNodeBase>();

        /// <summary>
        /// Define a sub expr-node.
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InNode"></param>
        protected void DefSubExprNode(string InName, STNodeBase InNode)
        {
            mSubExprNodes.Add(InName, InNode);
        }

        /// <summary>
        /// Sub syntax-tree node organized by collection.
        /// </summary>
        public IReadOnlyCollection<STNodeBase> SubExprNodesByCollection { get { return mSubExprNodes.Values; } }

        /// <summary>
        /// Sub syntax-tree nodes, organized by dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, STNodeBase> SubExprNodes { get { return mSubExprNodes; } }

    }


}
