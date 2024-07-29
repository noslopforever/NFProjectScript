using nf.protoscript.syntaxtree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace nf.protoscript.translator.DefaultSnippetElements.Internal
{

    /// <summary>
    /// An executor for evaluating expressions in a translation scheme.
    /// </summary>
    internal class ExprExecutor
    {


        /// <summary>
        /// Evaluates the result of the target expression.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InExprNode">The expression node to evaluate.</param>
        /// <returns>The evaluation result.</returns>
        public object EvalValue(IInfoTranslateSchemeInstance InHolderSchemeInstance, ISyntaxTreeNode InExprNode)
        {
            var stConst = InExprNode as STNodeConstant;
            var stCall = InExprNode as STNodeCall;
            var stVar = InExprNode as STNodeVar;
            var stMemberAccess = InExprNode as STNodeMemberAccess;

            if (stConst != null)
            {
                return _EvalValue(InHolderSchemeInstance, stConst);
            }
            else if (stVar != null)
            {
                return _EvalValue(InHolderSchemeInstance, stVar);
            }
            else if (stCall != null)
            {
                return _EvalValue(InHolderSchemeInstance, stCall);
            }
            else if (stMemberAccess != null)
            {
                return _EvalValue(InHolderSchemeInstance, stMemberAccess);
            }

            // TODO log error
            throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Evaluates the result of the constant expression.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InConst">The constant expression node.</param>
        /// <returns>The value of the constant.</returns>
        object _EvalValue(IInfoTranslateSchemeInstance InHolderSchemeInstance, STNodeConstant InConst)
        {
            return InConst.Value;
        }

        /// <summary>
        /// Evaluates the result of the variable expression.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InVar">The variable expression node.</param>
        /// <returns>The value of the variable.</returns>
        object _EvalValue(IInfoTranslateSchemeInstance InHolderSchemeInstance, STNodeVar InVar)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            // eval value from parameters.
            object outValue = null;
            if (InHolderSchemeInstance.TryGetParamValue(InVar.IDName, out outValue))
            {
                return outValue;
            }

            // eval value from context.
            if (context.TryGetContextValue(InVar.IDName, out outValue))
            {
                return outValue;
            }

            // TODO system var handling (with special name).

            // TEMP Handle Newline/Tab
            // TODO Replace it by system functions.
            if (0 == string.Compare(InVar.IDName, "$NL", true))
            {
                return Environment.NewLine;
            }
            else if (0 == string.Compare(InVar.IDName, "$TAB", true))
            {
                return "\t";
            }
            // ~TEMP


            // TODO log error
            throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Evaluates the result of the call expression.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InCall">The call expression node.</param>
        /// <returns>The result of the function call.</returns>
        object _EvalValue(IInfoTranslateSchemeInstance InHolderSchemeInstance, STNodeCall InCall)
        {
            // Retrieve parameter values.
            List<object> paramValues = new List<object>();
            if (InCall.Params != null)
            {
                paramValues = new List<object>(InCall.Params.Length);
                foreach (var param in InCall.Params)
                {
                    var paramValue = EvalValue(InHolderSchemeInstance, param);
                    paramValues.Add(paramValue);
                }
            }

            // Invoke and return
            var result = EvalFunction(InHolderSchemeInstance, InCall.FuncExpr, paramValues.ToArray());
            return result;
        }

        /// <summary>
        /// Evaluates the result of the member-access expression.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InMemberAccess">The member-access expression node.</param>
        /// <returns>The value accessed from the member.</returns>
        object _EvalValue(IInfoTranslateSchemeInstance InHolderSchemeInstance, STNodeMemberAccess InMemberAccess)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            var lhsValue = EvalValue(InHolderSchemeInstance, InMemberAccess.LHS);

            // Create the context of LHS
            var lhsCtx = translator.CreateContext(context, lhsValue);
            if (lhsCtx == null)
            {
                // TODO log error
                throw new NotImplementedException();
            }

            // Get result from the context of LHS.
            object outValue = null;
            if (!lhsCtx.TryGetContextValue(InMemberAccess.IDName, out outValue))
            {
                // TODO log error
                throw new NotImplementedException();
            }

            return outValue;
        }

        /// <summary>
        /// Evaluates the target expression as a function.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="funcExpr">The function expression node.</param>
        /// <param name="InParams">The parameters to pass to the function.</param>
        /// <returns>The result of the function call.</returns>
        object EvalFunction(IInfoTranslateSchemeInstance InHolderSchemeInstance, ISyntaxTreeNode funcExpr, object[] InParams)
        {
            var funcExprVar = funcExpr as STNodeVar;
            var funcExprMemberAccess = funcExpr as STNodeMemberAccess;

            if (funcExprVar != null)
            {
                return _EvalFunction(InHolderSchemeInstance, funcExprVar, InParams);
            }
            if (funcExprMemberAccess != null)
            {
                return _EvalFunction(InHolderSchemeInstance, funcExprMemberAccess, InParams);
            }

            // TODO log error
            throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Evaluates the function call { func() }.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InVar">The variable expression node representing the function.</param>
        /// <param name="InParams">The parameters to pass to the function.</param>
        /// <returns>The result of the function call.</returns>
        object _EvalFunction(IInfoTranslateSchemeInstance InHolderSchemeInstance, STNodeVar InVar, object[] InParams)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            // TODO system function handling (with special name).

            // TEMP Handle For function
            // TODO Replace it by system functions.
            if (0 == string.Compare(InVar.IDName, "For", true))
            {
                return _TEMP_HandleForCall(translator, context, InParams);
            }
            // ~TEMP

            // Call schemes
            return translator.TranslateInfo(context, InVar.IDName, InParams);
        }

        /// <summary>
        /// Evaluates the method call { obj.func() }.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InMemberAccess">The member-access expression node representing the function.</param>
        /// <param name="InParams">The parameters to pass to the function.</param>
        /// <returns>The result of the function call.</returns>
        object _EvalFunction(IInfoTranslateSchemeInstance InHolderSchemeInstance, STNodeMemberAccess InMemberAccess, object[] InParams)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            // Retrieve the LHS in 'LHS.Call()'
            var lhsValue = EvalValue(InHolderSchemeInstance, InMemberAccess.LHS);
            if (lhsValue == null)
            {
                // TODO log error
                throw new NotImplementedException();
            }

            // Create the context of LHS
            var lhsCtx = translator.CreateContext(context, lhsValue);
            if (lhsCtx == null)
            {
                // TODO log error
                throw new NotImplementedException();
            }

            // TODO system function handling (with special name).

            // TEMP Handle For function
            // TODO Replace it by system functions.
            if (InMemberAccess.IDName == "For")
            {
                return _TEMP_HandleForCall(translator, lhsCtx, InParams);
            }
            // ~TEMP

            // Try find the best functor
            return translator.TranslateInfo(lhsCtx, InMemberAccess.IDName, InParams);
        }

        // TEMP Handle For function
        private object _TEMP_HandleForCall(InfoTranslatorAbstract InTranslator, ITranslatingContext InContext, object[] InParams)
        {
            Debug.Assert(InParams.Length == 3);

            string filter = InParams[0] as string;
            string seperator = InParams[1] as string;
            string function = InParams[2] as string;

            Debug.Assert(filter != null && seperator != null && function != null);

            List<string> result = new List<string>();
            result.Add("");
            if (InContext is ITranslatingInfoContext)
            {
                (InContext as ITranslatingInfoContext)?.TranslatingInfo.ForeachSubInfo<Info>(
                    info =>
                    {
                        TranslatingInfoContext infoCtx = new TranslatingInfoContext(InContext, info);
                        var subResult = InTranslator.TranslateInfo(infoCtx, function);
                        if (subResult.Count > 0)
                        {
                            result[^1] += subResult[0];
                        }
                        for (int i = 1; i < subResult.Count; i++)
                        {
                            result.Add(subResult[i]);
                        }
                        result[^1] += seperator;
                    }
                    , info => info.Header.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
                    );
                // Remove the last seperator
                result[^1] = result[^1].Remove(result[^1].Length - seperator.Length);
            }
            else if (InContext is ITranslatingExprContext)
            {
                (InContext as ITranslatingExprContext).TranslatingExprNode.ForeachSubNodes(
                    (key, stNode) =>
                    {
                        if (key.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                        {
                            TranslatingExprContext exprCtx = new TranslatingExprContext(InContext, stNode);
                            var subResult = InTranslator.TranslateInfo(exprCtx, function);
                            if (subResult.Count > 0)
                            {
                                result[^1] += subResult[0];
                            }
                            for (int i = 1; i < subResult.Count; i++)
                            {
                                result.Add(subResult[i]);
                            }
                            result[^1] += seperator;
                        }
                        return true;
                    }
                    );
                // Remove the last seperator
                result[^1] = result[^1].Remove(result[^1].Length - seperator.Length);
            }
            return result;
        }
        // ~TEMP


    }


}