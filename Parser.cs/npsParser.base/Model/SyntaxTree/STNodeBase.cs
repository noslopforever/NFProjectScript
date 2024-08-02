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

        /// <summary>
        /// Get typename of this syntax-tree node.
        /// Always be the ClassName - STNode
        /// e.g. STNodeConstant -> "constant"
        /// </summary>
        /// <TODO> STNode(Info) -> STNodeType(TypeInfo).Name </TODO>
        public string Type_Name
        {
            get
            {
                Type type = GetType();
                return GetType().Name.Replace("STNode", "");
            }
        }

        public abstract void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc);

        public abstract TypeInfo GetPredictType(ElementInfo InHostElemInfo);

    }


}
