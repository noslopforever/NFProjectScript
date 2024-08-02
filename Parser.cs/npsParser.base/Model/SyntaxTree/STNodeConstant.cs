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
            Value = null;
            ValueType = CommonTypeInfos.Unknown;
        }
        public STNodeConstant(string InValueString)
        {
            Value = InValueString;
            ValueType = CommonTypeInfos.String;
        }
        public STNodeConstant(int InValue)
        {
            Value = InValue;
            ValueType = CommonTypeInfos.Integer;
        }
        public STNodeConstant(float InValue)
        {
            Value = (double)InValue;
            ValueType = CommonTypeInfos.Float;
        }
        public STNodeConstant(double InValue)
        {
            Value = InValue;
            ValueType = CommonTypeInfos.Float;
        }
        public STNodeConstant(TypeInfo InTypeInfo)
        {
            Value = InTypeInfo;
            ValueType = CommonTypeInfos.TypeRef;
        }

        public override void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return ValueType;
        }

        /// <summary>
        /// Value to save
        /// </summary>
        // TODO [stringfy Infos/ExprNodes] remove Value, use Value string instead.
        [Serialization.SerializableInfo]
        public object Value { get; private set; } = null;

        /// <summary>
        /// Type of the value
        /// </summary>
        [Serialization.SerializableInfo]
        public TypeInfo ValueType { get; private set; } = null;

        /// <summary>
        /// Value string.
        /// </summary>
        public string ValueString
        {
            get
            {
                if (Value == null)
                {
                    return "null";
                }
                return Value.ToString();
            }
        }

        // Begin object interfaces
        public override string ToString()
        {
            return $"Constant {{ Type = {ValueType}, Value = {ValueString} }}";
        }
        // ~ End object interfaces

    }

}
