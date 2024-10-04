using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.parser.nps1
{
    
    /// <summary>
    /// Represents a 'foreach' statement.
    /// </summary>
    [VirtualSTNode]
    class STNode_StatementForeach
        : ISyntaxTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_StatementForeach"/> class with the specified collection expression.
        /// </summary>
        /// <param name="InCollectionExpr">The expression representing the collection to iterate over.</param>
        public STNode_StatementForeach(ISyntaxTreeNode InCollectionExpr)
        {
            CollectionExpr = InCollectionExpr;
        }

        /// <summary>
        /// Gets the expression representing the collection to iterate over.
        /// </summary>
        public ISyntaxTreeNode CollectionExpr { get; private set; }

        /// <inheritdoc />
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc("CollectionExpr", CollectionExpr)) { return; }
        }

        /// <inheritdoc />
        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return null;
        }
    }

}
