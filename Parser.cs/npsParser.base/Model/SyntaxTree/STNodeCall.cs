using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// An expr-node to call some method.
    /// </summary>
    public class STNodeCall
        : STNodeBase
    {
        public STNodeCall()
            : base("call")
        {
        }

        public STNodeCall(string InFuncName)
            : base("call")
        {
            FuncName = InFuncName;
        }

        public STNodeCall(string InFuncName, ISyntaxTreeNode InParam0)
            : base("call")
        {
            FuncName = InFuncName;
            Params = new ISyntaxTreeNode[1] { InParam0 };
        }

        /// <summary>
        /// Function name
        /// </summary>
        public string FuncName { get; }

        /// <summary>
        /// Parameters
        /// </summary>
        public ISyntaxTreeNode[] Params { get; }

    }

}
