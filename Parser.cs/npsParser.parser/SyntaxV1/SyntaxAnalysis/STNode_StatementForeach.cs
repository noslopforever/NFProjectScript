using nf.protoscript.parser.token;
using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Statement: Foreach.
    /// </summary>
    class STNode_StatementForeach
        : syntaxtree.ISyntaxTreeNode
    {

        /// <summary>
        /// Expresion of the collection
        /// </summary>
        public syntaxtree.STNodeBase CollectionExpr { get; private set; }

        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc("CollectionExpr", CollectionExpr)) { return; }
        }

        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return null;
        }
    }

}
