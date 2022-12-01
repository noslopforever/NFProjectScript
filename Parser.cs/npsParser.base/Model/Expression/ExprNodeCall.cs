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

        public ExprNodeCall(string InFuncName)
            : base("call")
        {
            FuncName = InFuncName;
        }

        public ExprNodeCall(string InFuncName, IExpressionNode InParam0)
            : base("call")
        {
            FuncName = InFuncName;
            Params = new IExpressionNode[1] { InParam0 };
        }

        /// <summary>
        /// Function name
        /// </summary>
        public string FuncName { get; }

        /// <summary>
        /// Parameters
        /// </summary>
        public IExpressionNode[] Params { get; }

    }

}
