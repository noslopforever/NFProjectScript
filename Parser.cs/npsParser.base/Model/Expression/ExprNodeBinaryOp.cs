using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.expression
{

    /// <summary>
    /// Binary operators like + - * / %
    /// </summary>
    public class ExprNodeBinaryOp : ExprNodeBase
    {
        public ExprNodeBinaryOp(string InOpCode)
            : base(InOpCode)
        {
            OpCode = InOpCode;
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

    }

}
