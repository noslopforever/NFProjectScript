using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Expr node : assign.
    /// </summary>
    public class STNodeAssign
        : STNodeBase
    {
        public STNodeAssign()
            : base("assign")
        {
        }

        public STNodeAssign(ISyntaxTreeNode InLHS, ISyntaxTreeNode InRHS)
            : base("assign")
        {
            LHS = InLHS;
            RHS = InRHS;
        }

        /// <summary>
        /// Left hand value
        /// </summary>
        public ISyntaxTreeNode LHS { get; }

        /// <summary>
        /// Right hand value
        /// </summary>
        public ISyntaxTreeNode RHS { get; }

    }

}
