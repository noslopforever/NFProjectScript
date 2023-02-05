using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Binary operators like + - * / %
    /// </summary>
    public class STNodeBinaryOp : STNodeBase
    {
        internal STNodeBinaryOp()
        {
        }

        public STNodeBinaryOp(string InOpCode, ISyntaxTreeNode InLhs, ISyntaxTreeNode InRhs)
        {
            OpCode = InOpCode;
            LHS = InLhs;
            RHS = InRhs;
        }

        /// <summary>
        /// Suggested op definitions.
        /// </summary>
        public class Def
        {
            public const string Add = "add";
            public const string Sub = "sub";
            public const string Mul = "mul";
            public const string Div = "div";
            public const string Mod = "mod";
        }

        [Serialization.SerializableInfo]
        public string OpCode { get; private set; } = "";

        /// <summary>
        /// Left hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode LHS { get; private set; }

        /// <summary>
        /// Right hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

    }

}
