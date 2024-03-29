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
        internal STNodeConstant()
        {
            Value = null;
            Type = CommonTypeInfos.Unknown;
        }
        public STNodeConstant(string InValueString)
        {
            Value = InValueString;
            Type = CommonTypeInfos.String;
        }
        public STNodeConstant(int InValue)
        {
            Value = InValue;
            Type = CommonTypeInfos.Integer;
        }
        public STNodeConstant(float InValue)
        {
            Value = (double)InValue;
            Type = CommonTypeInfos.Float;
        }
        public STNodeConstant(double InValue)
        {
            Value = InValue;
            Type = CommonTypeInfos.Float;
        }
        public STNodeConstant(TypeInfo InTypeInfo)
        {
            Value = InTypeInfo;
            Type = CommonTypeInfos.TypeRef;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return Type;
        }

        /// <summary>
        /// Value to save
        /// </summary>
        [Serialization.SerializableInfo]
        public object Value { get; private set; } = null;

        [Serialization.SerializableInfo]
        public TypeInfo Type { get; private set; } = null;

    }

}
