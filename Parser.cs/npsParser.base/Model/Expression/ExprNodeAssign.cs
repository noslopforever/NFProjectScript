using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.expression
{

    /// <summary>
    /// Expr node : assign.
    /// </summary>
    public class ExprNodeAssign
        : ExprNodeBase
    {
        public ExprNodeAssign()
            : base("assign")
        {
        }

        public ExprNodeAssign(IExpressionNode InLHS, IExpressionNode InRHS)
            : base("assign")
        {
            LHS = InLHS;
            RHS = InRHS;
        }

        /// <summary>
        /// Left hand value
        /// </summary>
        public IExpressionNode LHS { get; }

        /// <summary>
        /// Right hand value
        /// </summary>
        public IExpressionNode RHS { get; }

    }

}
