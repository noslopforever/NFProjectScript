using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{


    /// <summary>
    /// Unary operators like +1 -1
    /// </summary>
    public class STNodeUnaryOp
        : STNodeBase
    {
        public STNodeUnaryOp(string InUnaryOpStr, ISyntaxTreeNode InRhs)
        {
            RHS = InRhs;
        }

        public class Def
        {
            public const string Positive = "+";
            public const string Negative = "-";
        }

        /// <summary>
        /// Operator character
        /// </summary>
        [Serialization.SerializableInfo]
        public string OpCode { get; private set; } = "";

        /// <summary>
        /// Right-hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }


    }

}
