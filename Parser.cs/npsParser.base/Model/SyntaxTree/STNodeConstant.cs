﻿using System;
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
        public STNodeConstant()
            : base("const")
        {
        }

        public STNodeConstant(string InValueTypeStr, string InValueString)
            : base("const")
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
        public string ValueTypeString { get; set; } = "";

        /// <summary>
        /// String to save the constant.
        /// </summary>
        public string ValueString { get; set; } = "";

    }

}