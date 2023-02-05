﻿using System;
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
            : base()
        {
        }

        public STNodeAssign(ISyntaxTreeNode InLHS, ISyntaxTreeNode InRHS)
            : base()
        {
            LHS = InLHS;
            RHS = InRHS;
        }

        /// <summary>
        /// Left hand value
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode LHS { get; private set; }

        /// <summary>
        /// Right hand value
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

    }

}
