using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    public class STNodeUnaryOp
        : STNodeBase
    {
        public STNodeUnaryOp(string InUnaryOpStr)
        {
        }

        public class Def
        {
            public const string Positive = "pos";
            public const string Negative = "neg";
        }


    }

}
