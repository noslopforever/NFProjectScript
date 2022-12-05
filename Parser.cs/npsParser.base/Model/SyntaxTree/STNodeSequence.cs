﻿using System.Collections.Generic;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// STNode sequence.
    /// 
    /// local a, b, c = 0;
    /// a = b + c;
    /// return c;
    /// 
    /// </summary>
    public class STNodeSequence : STNodeBase
    {
        public STNodeSequence()
            : base("sequence")
        {
        }

        public STNodeSequence(params ISyntaxTreeNode[] InSTNodes)
            : base("sequence")
        {
            NodeList = InSTNodes;
        }

        /// <summary>
        /// Sub STNode list.
        /// </summary>
        public IReadOnlyList<ISyntaxTreeNode> NodeList { get; }

    }

}