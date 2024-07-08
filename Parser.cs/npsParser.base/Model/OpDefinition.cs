using System;
using System.Collections.Generic;

namespace nf.protoscript
{
    /// <summary>
    /// Usage of a operator
    /// </summary>
    public enum EOpUsage
    {
        /// <summary>
        /// Invalid usage.
        /// </summary>
        Unknown,

        /// <summary>
        /// The op is a Comparer to compare two operating-values.
        /// </summary>
        Comparer,

        /// <summary>
        /// The op is a Operator to modify the left-hand object by the right-hand object.
        /// </summary>
        LOperator,

        /// <summary>
        /// The op is a Operator to modify the right-hand object by the left-hand object.
        /// </summary>
        ROperator,

        /// <summary>
        /// The op is a Boolean operator to perform boolean operations (AND, OR) on the left-hand and right-hand objects.
        /// </summary>
        BooleanOperator,

        /// <summary>
        /// The op is a special Bitwise operator to perform bitwise operations.
        /// </summary>
        BitwiseOperator,

        /// <summary>
        /// The op is an unary boolean checker.
        /// </summary>
        UnaryBooleanOperator,

        /// <summary>
        /// The op is an unary operator to operate only one object.
        /// </summary>
        UnaryOperator,

    }

    /// <summary>
    /// Function of a operator
    /// </summary>
    public enum EOpFunction
    {
        Unknown,

        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        NotEqual,

        Add,
        Substract,
        Multiply,
        Divide,
        Mod,
        Exp,
        Log,
        Sqrt,
        Lg,
        Ln,

        And,
        Or,
        Not,

        BitwiseAnd,
        BitwiseOr,
        BitwiseNot,
        ShiftLeft,
        ShiftRight,

        Positive,
        Negative,
        Absolute,
        Increment,
        Decrement,

        Custom,
    }

    /// <summary>
    /// A definition object to describe a operator.
    /// </summary>
    public class OpDefinition
    {
        public OpDefinition(EOpFunction InFunction)
        {
            Function = InFunction;
        }

        /// <summary>
        /// Get usages from function
        /// </summary>
        /// <param name="InFunction"></param>
        /// <returns></returns>
        public static EOpUsage GetDefaultFunctionUsage(EOpFunction InFunction)
        {
            switch (InFunction)
            {
                case EOpFunction.LessThan:
                case EOpFunction.LessThanOrEqual:
                case EOpFunction.GreaterThan:
                case EOpFunction.GreaterThanOrEqual:
                case EOpFunction.Equal:
                case EOpFunction.NotEqual:
                    return EOpUsage.Comparer;
                case EOpFunction.Add:
                case EOpFunction.Substract:
                case EOpFunction.Multiply:
                case EOpFunction.Divide:
                case EOpFunction.Mod:
                case EOpFunction.Exp:
                case EOpFunction.Log:
                    return EOpUsage.LOperator;
                case EOpFunction.Sqrt:
                case EOpFunction.Lg:
                case EOpFunction.Ln:
                    return EOpUsage.UnaryOperator;
                case EOpFunction.And:
                case EOpFunction.Or:
                    return EOpUsage.BooleanOperator;
                case EOpFunction.Not:
                    return EOpUsage.UnaryBooleanOperator;
                case EOpFunction.BitwiseAnd:
                case EOpFunction.BitwiseOr:
                case EOpFunction.ShiftLeft:
                case EOpFunction.ShiftRight:
                    return EOpUsage.BitwiseOperator;
                case EOpFunction.BitwiseNot:
                    return EOpUsage.UnaryBooleanOperator;
                case EOpFunction.Positive:
                case EOpFunction.Negative:
                case EOpFunction.Absolute:
                case EOpFunction.Increment:
                case EOpFunction.Decrement:
                    return EOpUsage.UnaryOperator;
            }
            return EOpUsage.Unknown;
        }

        /// <summary>
        /// Name(Purpose) of the OpDefinition
        /// </summary>
        public EOpFunction Function { get; }

        /// <summary>
        /// Default code of the operation, nullable.
        /// </summary>
        public string DefaultOpCode
        {
            get
            {
                switch (Function)
                {
                    case EOpFunction.LessThan: return "<";
                    case EOpFunction.LessThanOrEqual:return "<=";
                    case EOpFunction.GreaterThan: return ">";
                    case EOpFunction.GreaterThanOrEqual: return ">=";
                    case EOpFunction.Equal: return "==";
                    case EOpFunction.NotEqual: return "!=";
                    case EOpFunction.Add: return "+";
                    case EOpFunction.Substract: return "-";
                    case EOpFunction.Multiply: return "*";
                    case EOpFunction.Divide: return "/";
                    case EOpFunction.Mod: return "%";
                    case EOpFunction.Exp: return "^";
                    case EOpFunction.And: return "&&";
                    case EOpFunction.Or: return "||";
                    case EOpFunction.BitwiseAnd: return "&";
                    case EOpFunction.BitwiseOr: return "|";
                    case EOpFunction.BitwiseNot: return "~";
                    case EOpFunction.Not: return "!";
                    case EOpFunction.ShiftLeft: return "<<";
                    case EOpFunction.ShiftRight: return ">>";
                    case EOpFunction.Positive: return "+";
                    case EOpFunction.Negative: return "-";
                    case EOpFunction.Increment: return "++";
                    case EOpFunction.Decrement: return "--";
                }
                return null;
            }
        }

        /// <summary>
        /// Usage of the bin-op
        /// </summary>
        public EOpUsage Usage
        {
            get
            {
                if (Function == EOpFunction.Custom)
                {
                    return CustomUsage;
                }
                return GetDefaultFunctionUsage(Function);
            }
        }

        /// <summary>
        /// Custom function code if the op is a custom op.
        /// </summary>
        public string CustomFunction { get; }

        /// <summary>
        /// Custom Usage if the op is a custom op.
        /// </summary>
        public EOpUsage CustomUsage { get; }

        public string OpCode
        {
            get
            {
                if (Function == EOpFunction.Unknown)
                {
                    return "<InvalidOperator>";
                }
                if (Function == EOpFunction.Custom)
                {
                    return CustomFunction;
                }
                return DefaultOpCode;
            }
        }

        public override string ToString()
        {
            return OpCode;
        }

    }

    /// <summary>
    /// OpDefinition manager to manage BinOps used by the script.
    /// </summary>
    public class OpDefManager
    {
        private OpDefManager()
        {
            AddOpDefine(EOpFunction.LessThan);
            AddOpDefine(EOpFunction.LessThanOrEqual);
            AddOpDefine(EOpFunction.GreaterThan);
            AddOpDefine(EOpFunction.GreaterThanOrEqual);
            AddOpDefine(EOpFunction.Equal);
            AddOpDefine(EOpFunction.NotEqual);

            AddOpDefine(EOpFunction.Add);
            AddOpDefine(EOpFunction.Substract);
            AddOpDefine(EOpFunction.Multiply);
            AddOpDefine(EOpFunction.Divide);
            AddOpDefine(EOpFunction.Mod);
            AddOpDefine(EOpFunction.Exp);

            AddOpDefine(EOpFunction.And);
            AddOpDefine(EOpFunction.Or);
            AddOpDefine(EOpFunction.Not);

            AddOpDefine(EOpFunction.BitwiseAnd);
            AddOpDefine(EOpFunction.BitwiseOr);
            AddOpDefine(EOpFunction.BitwiseNot);
            AddOpDefine(EOpFunction.ShiftLeft);
            AddOpDefine(EOpFunction.ShiftRight);

            AddOpDefine(EOpFunction.Positive);
            AddOpDefine(EOpFunction.Negative);
            AddOpDefine(EOpFunction.Absolute);
            AddOpDefine(EOpFunction.Increment);
            AddOpDefine(EOpFunction.Decrement);
        }

        /// <summary>
        /// The Singleton instance of OpDefManager
        /// </summary>
        public static OpDefManager Instance { get; } = new OpDefManager();

        /// <summary>
        /// Query definition
        /// </summary>
        /// <param name="InFunction"></param>
        /// <param name="InCustomName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public OpDefinition Get(EOpFunction InFunction, string InCustomName = "")
        {
            if (InFunction == EOpFunction.Unknown)
            {
            }
            else if (InFunction == EOpFunction.Custom)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (_defaultDefs.TryGetValue(InFunction, out var existDef))
                {
                    return existDef;
                }
            }
            return null;
        }

        /// <summary>
        /// Register an OpDefine to the manager.
        /// </summary>
        /// <param name="InUsage"></param>
        /// <param name="InOpCodes"></param>
        private void AddOpDefine(EOpFunction InFunction, string InCustomName = "")
        {
            if (InFunction == EOpFunction.Unknown)
            {
                // TODO log error
                throw new ArgumentException();
            }
            else if (InFunction == EOpFunction.Custom)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (_defaultDefs.TryGetValue(InFunction, out var existDef))
                {
                    // TODO log error
                    throw new ArgumentException();
                }
                _defaultDefs.Add(InFunction, new OpDefinition(InFunction));
            }
        }

        ///// <summary>
        ///// Remove an OpDefine
        ///// </summary>
        //public void ClearBinOp(EOpFunction InFunction, string InCustomName = "")
        //{
        //    if (InFunction == EOpFunction.Unknown)
        //    {
        //        // TODO log error
        //        throw new ArgumentException();
        //    }
        //    else if (InFunction == EOpFunction.Custom)
        //    {
        //        throw new NotImplementedException();
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        // non-custom definitions
        Dictionary<EOpFunction, OpDefinition> _defaultDefs = new Dictionary<EOpFunction, OpDefinition>();

    }


    /// <summary>
    /// A code-def pair.
    /// </summary>
    public struct OpCodeWithDef
    {
        public OpCodeWithDef(string InOpCode, EOpFunction InFunc, string InCustom = "")
        {
            OpCode = InOpCode;
            OpFunc = InFunc;
            CustomFuncName = InCustom;
        }

        /// <summary>
        /// Code of this pair
        /// </summary>
        public string OpCode;

        /// <summary>
        /// Function pair with the OpCode
        /// </summary>
        public EOpFunction OpFunc;

        /// <summary>
        /// Custom Func name if the Function is a custom function.
        /// </summary>
        public string CustomFuncName;

        /// <summary>
        /// Find Def by pass a code from a pair-array.
        /// </summary>
        /// <param name="InArray"></param>
        /// <param name="InCode"></param>
        /// <returns></returns>
        public static OpDefinition FindDefByCode(IEnumerable<OpCodeWithDef> InArray, string InCode)
        {
            foreach (var owd in InArray)
            {
                if (owd.OpCode == InCode)
                {
                    return OpDefManager.Instance.Get(owd.OpFunc, owd.CustomFuncName);
                }
            }
            return null;
        }

        /// <summary>
        /// Check if a def-array contains a element with the target op code.
        /// </summary>
        /// <param name="InArray"></param>
        /// <param name="InCode"></param>
        /// <returns></returns>
        public static bool Contains(IEnumerable<OpCodeWithDef> InArray, string InCode)
        {
            foreach (var owd in InArray)
            {
                if (owd.OpCode == InCode)
                {
                    return true;
                }
            }
            return false;
        }


    }


}
