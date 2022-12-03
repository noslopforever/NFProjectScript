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
        public STNodeBinaryOp(string InOpCode, ISyntaxTreeNode InLhs, ISyntaxTreeNode InRhs)
            : base(InOpCode)
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

        public string OpCode { get; } = "";

        /// <summary>
        /// Left hand expression
        /// </summary>
        public ISyntaxTreeNode LHS { get; }

        /// <summary>
        /// Right hand expression
        /// </summary>
        public ISyntaxTreeNode RHS { get; }

    }

}
