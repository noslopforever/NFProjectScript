using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.expression
{

    public class ExprNodeUnaryOp
        : ExprNodeBase
    {
        public ExprNodeUnaryOp(string InUnaryOpStr)
            : base(InUnaryOpStr)
        {
        }

        public class Def
        {
            public const string Positive = "pos";
            public const string Negative = "neg";
        }


    }

}
