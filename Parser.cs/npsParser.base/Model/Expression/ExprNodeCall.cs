using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.expression
{

    /// <summary>
    /// An expr-node to call some method.
    /// </summary>
    public class ExprNodeCall
        : ExprNodeBase
    {
        public ExprNodeCall()
            : base("call")
        {
        }

    }

}
