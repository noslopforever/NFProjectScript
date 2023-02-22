using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Expr-node: constant
    /// </summary>
    public class STNodeConstant
        : STNodeBase
    {
        internal STNodeConstant()
        {
        }

        public STNodeConstant(string InValueString)
        {
            Value = InValueString;
        }
        public STNodeConstant(int InValue)
        {
            Value = InValue;
        }
        public STNodeConstant(float InValue)
        {
            Value = InValue;
        }
        public STNodeConstant(TypeInfo InTypeInfo)
        {
            Value = InTypeInfo;
        }

        /// <summary>
        /// Value to save
        /// </summary>
        [Serialization.SerializableInfo]
        public object Value { get; set; } = null;


    }

}
