﻿using System;
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
        internal STNodeCall()
        {
        }

        public STNodeCall(string InFuncName)
        {
            FuncName = InFuncName;
        }

        public STNodeCall(string InFuncName, ISyntaxTreeNode InParam0)
        {
            FuncName = InFuncName;
            Params = new ISyntaxTreeNode[1] { InParam0 };
        }

        /// <summary>
        /// Function name
        /// </summary>
        [Serialization.SerializableInfo]
        public string FuncName { get; private set; }

        /// <summary>
        /// Parameters
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode[] Params { get; private set; }

    }

}
