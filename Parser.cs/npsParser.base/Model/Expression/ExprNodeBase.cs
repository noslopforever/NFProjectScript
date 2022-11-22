using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.expression
{

    /// <summary>
    /// Expression node base.
    /// </summary>
    public class ExprNodeBase
        : IExpressionNode
    {
        public ExprNodeBase(string InSignature)
        {
            Signature = InSignature;
        }

        /// <summary>
        /// Signature (+,-,*,/) of the node. 
        /// </summary>
        public string Signature { get; protected set; }

        // Internal sub expr node, call DefSubExprNode to add entries to it.
        private Dictionary<string, ExprNodeBase> mSubExprNodes = new Dictionary<string, ExprNodeBase>();

        /// <summary>
        /// Define a sub expr-node.
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InNode"></param>
        protected void DefSubExprNode(string InName, ExprNodeBase InNode)
        {
            mSubExprNodes.Add(InName, InNode);
        }

        /// <summary>
        /// Sub-expression node organized by collection.
        /// </summary>
        public IReadOnlyCollection<ExprNodeBase> SubExprNodesByCollection { get { return mSubExprNodes.Values; } }

        /// <summary>
        /// Sub-expression nodes, organized by dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, ExprNodeBase> SubExprNodes { get { return mSubExprNodes; } }

    }


}
