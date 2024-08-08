using nf.protoscript.syntaxtree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace nf.protoscript.translator.DefaultScheme.Elements.Internal
{

    /// <summary>
    /// An executor for evaluating expressions in a translation scheme.
    /// </summary>
    internal class ExprExecutor
    {
        /// <summary>
        /// Delegate for getting the value of a variable or member.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller.</param>
        /// <param name="InSTNode">The syntax tree node to evaluate.</param>
        /// <param name="InHolderObject">The object holding the variable or member. Null if global.</param>
        /// <param name="InVarName">The name of the variable or member.</param>
        /// <returns>The value of the variable or member.</returns>
        public delegate object DelegateType_GetValue(object InExternalObject, ISyntaxTreeNode InSTNode, object InHolderObject, string InVarName);

        /// <summary>
        /// Delegate for invoking a method or function.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller.</param>
        /// <param name="InSTNode">The syntax tree node to evaluate.</param>
        /// <param name="InHolderObject">The object on which to invoke the method. Null if global.</param>
        /// <param name="InFuncName">The name of the method or function.</param>
        /// <param name="InParams">Parameters to pass to the method or function.</param>
        /// <returns>The result of the invocation.</returns>
        public delegate object DelegateType_Invoke(object InExternalObject, ISyntaxTreeNode InSTNode, object InHolderObject, string InFuncName, object[] InParams);

        /// <summary>
        /// Initializes a new instance of the <see cref="ExprExecutor"/> class.
        /// </summary>
        /// <param name="InGetValueCallback">The callback to retrieve variable or member values.</param>
        /// <param name="InInvokeCallback">The callback to invoke methods or functions.</param>
        public ExprExecutor(DelegateType_GetValue InGetValueCallback, DelegateType_Invoke InInvokeCallback)
        {
            _delGetValue = InGetValueCallback;
            _delInvoke = InInvokeCallback;
        }

        /// <summary>
        /// Evaluates the result of the target expression.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InExprNode">The expression node to evaluate.</param>
        /// <returns>The evaluation result.</returns>
        public object EvalValue(object InExternalObject, ISyntaxTreeNode InExprNode)
        {
            var stConst = InExprNode as STNodeConstant;
            var stCall = InExprNode as STNodeCall;
            var stVar = InExprNode as STNodeVar;
            var stMemberAccess = InExprNode as STNodeMemberAccess;
            var stUnaryOp = InExprNode as STNodeUnaryOp;
            var stBinOp = InExprNode as STNodeBinaryOp;

            if (stConst != null)
            {
                return _EvalValue(InExternalObject, stConst);
            }
            else if (stVar != null)
            {
                return _EvalValue(InExternalObject, stVar);
            }
            else if (stCall != null)
            {
                return _EvalValue(InExternalObject, stCall);
            }
            else if (stMemberAccess != null)
            {
                return _EvalValue(InExternalObject, stMemberAccess);
            }
            else if (stUnaryOp != null)
            {
                return _EvalValue(InExternalObject, stUnaryOp);
            }
            else if (stBinOp != null)
            {
                return _EvalValue(InExternalObject, stBinOp);
            }

            // Log an error message that the expression type is not supported.
            Debug.WriteLine("Unsupported expression type.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the result of the constant expression.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InConst">The constant expression node.</param>
        /// <returns>The value of the constant.</returns>
        private object _EvalValue(object InExternalObject, STNodeConstant InConst)
        {
            return InConst.Value;
        }

        /// <summary>
        /// Evaluates the result of the variable expression.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InVar">The variable expression node.</param>
        /// <returns>The value of the variable.</returns>
        private object _EvalValue(object InExternalObject, STNodeVar InVar)
        {
            object outValue = _delGetValue(InExternalObject, InVar, null, InVar.IDName);
            return outValue;
        }

        /// <summary>
        /// Evaluates the result of the call expression.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InCall">The call expression node.</param>
        /// <returns>The result of the function call.</returns>
        private object _EvalValue(object InExternalObject, STNodeCall InCall)
        {
            // Retrieve parameter values.
            List<object> paramValues = new List<object>();
            if (InCall.Params != null)
            {
                paramValues = new List<object>(InCall.Params.Length);
                foreach (var param in InCall.Params)
                {
                    var paramValue = EvalValue(InExternalObject, param);
                    paramValues.Add(paramValue);
                }
            }

            // Invoke and return
            var result = EvalFunction(InExternalObject, InCall.FuncExpr, paramValues.ToArray());
            return result;
        }

        /// <summary>
        /// Evaluates the result of the member-access expression.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InMemberAccess">The member-access expression node.</param>
        /// <returns>The value accessed from the member.</returns>
        private object _EvalValue(object InExternalObject, STNodeMemberAccess InMemberAccess)
        {
            var lhsValue = EvalValue(InExternalObject, InMemberAccess.LHS);

            object outValue = _delGetValue(InExternalObject, InMemberAccess, lhsValue, InMemberAccess.IDName);
            return outValue;
        }

        /// <summary>
        /// Evaluates the result of the unary operator expression.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InUnaryOp">The unary operator expression node.</param>
        /// <returns>The result of the unary operation.</returns>
        private object _EvalValue(object InExternalObject, STNodeUnaryOp InUnaryOp)
        {
            var rhsValue = EvalValue(InExternalObject, InUnaryOp.RHS);
            if (InUnaryOp.OpDef.Function == EOpFunction.Not)
            {
                bool rhsBool = Convert.ToBoolean(rhsValue);
                return !rhsBool;
            }
            // Log an error message that the unary operation is not supported.
            Debug.WriteLine("Unsupported unary operation.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the result of the binary operator expression.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InBinOp">The binary operator expression node.</param>
        /// <returns>The result of the binary operation.</returns>
        private object _EvalValue(object InExternalObject, STNodeBinaryOp InBinOp)
        {
            // Evaluate the right-hand side and left-hand side of the binary operation.
            var rhsValue = EvalValue(InExternalObject, InBinOp.RHS);
            var lhsValue = EvalValue(InExternalObject, InBinOp.LHS);

            // Handle comparison operations.
            if (InBinOp.OpDef.Function == EOpFunction.LessThan
                || InBinOp.OpDef.Function == EOpFunction.LessThanOrEqual
                || InBinOp.OpDef.Function == EOpFunction.GreaterThan
                || InBinOp.OpDef.Function == EOpFunction.GreaterThanOrEqual
            )
            {
                // Convert both operands to doubles for numerical comparison.
                // TODO catch exceptions.
                double lhsDouble = Convert.ToDouble(lhsValue);
                double rhsDouble = Convert.ToDouble(rhsValue);

                // Perform the specified comparison.
                switch (InBinOp.OpDef.Function)
                {
                    case EOpFunction.LessThan:
                        return lhsDouble < rhsDouble;
                    case EOpFunction.LessThanOrEqual:
                        return lhsDouble <= rhsDouble;
                    case EOpFunction.GreaterThan:
                        return lhsDouble > rhsDouble;
                    case EOpFunction.GreaterThanOrEqual:
                        return lhsDouble >= rhsDouble;
                }
            }
            // Handle equality and inequality operations.
            else if (InBinOp.OpDef.Function == EOpFunction.Equal
                || InBinOp.OpDef.Function == EOpFunction.NotEqual
            )
            {
                // Perform the specified equality check.
                switch (InBinOp.OpDef.Function)
                {
                    case EOpFunction.Equal:
                        return lhsValue.Equals(rhsValue);
                    case EOpFunction.NotEqual:
                        return !lhsValue.Equals(rhsValue);
                }
            }
            // Handle logical AND and OR operations.
            else if (InBinOp.OpDef.Function == EOpFunction.And
                || InBinOp.OpDef.Function == EOpFunction.Or
            )
            {
                // Convert both operands to booleans for logical operations.
                bool lhsBool = Convert.ToBoolean(lhsValue);
                bool rhsBool = Convert.ToBoolean(rhsValue);

                // Perform the specified logical operation.
                switch (InBinOp.OpDef.Function)
                {
                    case EOpFunction.And:
                        return lhsBool && rhsBool;
                    case EOpFunction.Or:
                        return lhsBool || rhsBool;
                }
            }
            else if (InBinOp.OpDef.Function == EOpFunction.Add
                || InBinOp.OpDef.Function == EOpFunction.Decrement
                || InBinOp.OpDef.Function == EOpFunction.Multiply
                || InBinOp.OpDef.Function == EOpFunction.Divide
                || InBinOp.OpDef.Function == EOpFunction.Mod
            )
            {
                // Handle string concatenation if the left-hand side is a string.
                if (lhsValue is string)
                {
                    if (InBinOp.OpDef.Function == EOpFunction.Add)
                    {
                        // Concatenate strings if the operation is addition.
                        return lhsValue.ToString() + rhsValue.ToString();
                    }

                    // For other operations, throw an exception because they are not implemented for strings.
                    throw new NotImplementedException("String operations other than concatenation are not supported.");
                }

                // Check if either operand is a boolean.
                if (lhsValue is bool || rhsValue is bool)
                {
                    // Boolean +-*/% are not implemented.
                    throw new NotImplementedException("Boolean arithmetic operations are not supported.");
                }

                // Convert both operands to numbers and perform the arithmetic operation.
                double lhsDouble = Convert.ToDouble(lhsValue);
                double rhsDouble = Convert.ToDouble(rhsValue);
                switch (InBinOp.OpDef.Function)
                {
                    case EOpFunction.Add:
                        return lhsDouble + rhsDouble;
                    case EOpFunction.Decrement:
                        return lhsDouble - rhsDouble;
                    case EOpFunction.Multiply:
                        return lhsDouble * rhsDouble;
                    case EOpFunction.Divide:
                        return lhsDouble / rhsDouble;
                    case EOpFunction.Mod:
                        return lhsDouble % rhsDouble;
                }
            }
            // Log an error message that the binary operation is not supported.
            Debug.WriteLine("Unsupported binary operation.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the target expression as a function.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InFuncExpr">The function expression node.</param>
        /// <param name="InParams">The parameters to pass to the function.</param>
        /// <returns>The result of the function call.</returns>
        private object EvalFunction(object InExternalObject, ISyntaxTreeNode InFuncExpr, object[] InParams)
        {
            var funcExprVar = InFuncExpr as STNodeVar;
            var funcExprMemberAccess = InFuncExpr as STNodeMemberAccess;

            if (funcExprVar != null)
            {
                return _EvalFunction(InExternalObject, funcExprVar, InParams);
            }
            if (funcExprMemberAccess != null)
            {
                return _EvalFunction(InExternalObject, funcExprMemberAccess, InParams);
            }

            // Log an error message that the function expression type is not supported.
            Debug.WriteLine("Unsupported function expression type.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates the function call { func() }.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InVar">The variable expression node representing the function.</param>
        /// <param name="InParams">The parameters to pass to the function.</param>
        /// <returns>The result of the function call.</returns>
        private object _EvalFunction(object InExternalObject, STNodeVar InVar, object[] InParams)
        {
            object outValue = _delInvoke(InExternalObject, InVar, null, InVar.IDName, InParams);
            return outValue;
        }

        /// <summary>
        /// Evaluates the method call { obj.func() }.
        /// </summary>
        /// <param name="InExternalObject">An external object passed by the caller, used in callbacks.</param>
        /// <param name="InMemberAccess">The member-access expression node representing the function.</param>
        /// <param name="InParams">The parameters to pass to the function.</param>
        /// <returns>The result of the function call.</returns>
        private object _EvalFunction(object InExternalObject, STNodeMemberAccess InMemberAccess, object[] InParams)
        {
            // Retrieve the LHS in 'LHS.Call()'
            var lhsValue = EvalValue(InExternalObject, InMemberAccess.LHS);
            if (lhsValue == null)
            {
                return null;
            }

            object outValue = _delInvoke(InExternalObject, InMemberAccess, lhsValue, InMemberAccess.IDName, InParams);
            return outValue;
        }

        // GetValue callback
        private readonly DelegateType_GetValue _delGetValue;
        // Invoke callback
        private readonly DelegateType_Invoke _delInvoke;
    }


}