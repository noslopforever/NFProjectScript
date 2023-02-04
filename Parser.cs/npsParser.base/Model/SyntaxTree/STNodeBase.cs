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
        public STNodeBase()
        {
        }

        // TODO support runtime iterate-call

        //public STNodeBase(KeyValuePair<string, ISyntaxTreeNode>[] InSubNodes)
        //{
        //    SubNodes = new Dictionary<string, ISyntaxTreeNode>(InSubNodes);
        //}

        
        //Dictionary<string, ISyntaxTreeNode> SubNodes { get; }

        ///// <summary>
        ///// Sub syntax-tree node organized by collection.
        ///// </summary>
        //public IReadOnlyCollection<ISyntaxTreeNode> SubExprNodesByCollection
        //{
        //    get 
        //    {
        //        return SubNodes.Values;
        //    }
        //}

        ///// <summary>
        ///// Sub syntax-tree nodes, organized by dictionary.
        ///// </summary>
        //public IReadOnlyDictionary<string, ISyntaxTreeNode> SubExprNodes
        //{
        //    get
        //    { 
        //        return SubNodes;
        //    } 
        //}

    }


}
