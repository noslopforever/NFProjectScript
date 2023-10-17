using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Syntax-tree node base.
    /// </summary>
    public abstract class STNodeBase
        : ISyntaxTreeNode
    {
        public STNodeBase()
        {
        }

        public abstract void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc);

        public abstract TypeInfo GetPredictType(ElementInfo InHostElemInfo);

    }


}
