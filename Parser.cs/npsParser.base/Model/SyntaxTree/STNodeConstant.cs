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

        public STNodeConstant(string InValueTypeStr, string InValueString)
        {
            ValueTypeString = InValueTypeStr;
            ValueString = InValueString;
        }

        /// <summary>
        /// Common type definitions
        /// </summary>
        public const string Any = "any";
        public const string Byte = "byte";
        public const string Integer = "integer";
        public const string Float = "float";
        public const string String = "string";
        public const string DateTime = "datetime";

        /// <summary>
        /// Type of the constant.
        /// </summary>
        [Serialization.SerializableInfo]
        public string ValueTypeString { get; set; } = "";

        /// <summary>
        /// String to save the constant.
        /// </summary>
        [Serialization.SerializableInfo]
        public string ValueString { get; set; } = "";

    }

}
