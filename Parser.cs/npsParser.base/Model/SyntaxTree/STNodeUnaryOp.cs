using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace nf.protoscript.syntaxtree
{


    /// <summary>
    /// Unary operators like +1 -1
    /// </summary>
    public class STNodeUnaryOp
        : STNodeBase
    {
        public STNodeUnaryOp(OpDefinition InOpDef, ISyntaxTreeNode InRhs)
        {
            Debug.Assert(InOpDef.Usage == EOpUsage.UnaryBooleanOperator || InOpDef.Usage == EOpUsage.UnaryOperator);
            OpDef = InOpDef;
            RHS = InRhs;
        }

        /// <summary>
        /// Operator character
        /// </summary>
        [Serialization.SerializableInfo]
        public OpDefinition OpDef { get; private set; }

        /// <summary>
        /// Right-hand expression
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

        
    }

}
