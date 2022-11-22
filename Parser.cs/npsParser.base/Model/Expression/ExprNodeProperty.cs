using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.expression
{

    /// <summary>
    /// An expr-node to access a sub property of the LHS.
    /// </summary>
    public class ExprNodeProperty : ExprNodeBase
    {
        public ExprNodeProperty()
            : base("property")
        {
        }


    }

}
