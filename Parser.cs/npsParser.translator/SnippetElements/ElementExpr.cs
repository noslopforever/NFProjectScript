using nf.protoscript.syntaxtree;
using nf.protoscript.translator.DefaultScheme.Elements.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace nf.protoscript.translator.DefaultScheme.Elements
{

    /// <summary>
    /// Represents an element that is defined by eaxpressions.
    /// </summary>
    public class ElementExpr
        : InfoTranslateSchemeDefault.IElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementExpr"/> class with the specified expression.
        /// </summary>
        /// <param name="InExpr">The original expression.</param>
        public ElementExpr(ISyntaxTreeNode InExpr)
        {
            Expression = InExpr;
        }

        /// <summary>
        /// Gets the expression parsed from the ExprCode.
        /// </summary>
        public ISyntaxTreeNode Expression { get; private set; } = null;

        // Begin IElement interfaces

        /// <inheritdoc />
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            // Execute the translator expressions
            var exprResult = Execute(InHolderSchemeInstance, Expression);
            if (exprResult == null)
            {
                return new string[] { "" };
            }

            if (exprResult is IReadOnlyList<string>)
            {
                return exprResult as IReadOnlyList<string>;
            }
            return new string[] { exprResult.ToString() };
        }

        // ~ End IElement interfaces

        /// <summary>
        /// Executes an expression statement within the given translation scheme instance.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InStatement">The expression to execute.</param>
        /// <returns>The result of the expression execution.</returns>
        private static object Execute(IInfoTranslateSchemeInstance InHolderSchemeInstance, ISyntaxTreeNode InStatement)
        {
            ExprExecutor executor = new ExprExecutor(
                (schemeInstObj, stNode, holder, varName) =>
                {
                    var schemeInst = schemeInstObj as IInfoTranslateSchemeInstance;
                    if (holder != null)
                    {
                        return _HandleMemberVar(schemeInst, holder, varName);
                    }
                    return _HandleGlobalVar(schemeInst, varName);
                }
                ,
                (schemeInstObj, stNode, holder, varName, paramList) =>
                {
                    var schemeInst = schemeInstObj as IInfoTranslateSchemeInstance;
                    if (holder != null)
                    {
                        // Try handling a member function.
                        return _HandleMemberMethod(schemeInst, holder, varName, paramList);
                    }
                    // Try handling a global function.
                    return _HandleGlobalFunction(schemeInst, varName, paramList);
                }
                );

            object val = executor.EvalValue(InHolderSchemeInstance, InStatement);
            return val;
        }

        // Begin object interfaces
        /// <inheritdoc />
        public override string ToString()
        {
            return Expression != null? Expression.ToString(): "<null>";
        }
        // ~ End object interfaces

        /// <summary>
        /// Handles retrieval of a global variable.
        /// </summary>
        /// <param name="InSchemeInstance">The translation scheme instance.</param>
        /// <param name="InVarName">The name of the variable.</param>
        /// <returns>The value of the variable or throws an exception if not found.</returns>
        private static object _HandleGlobalVar(IInfoTranslateSchemeInstance InSchemeInstance, string InVarName)
        {
            var translator = InSchemeInstance.HostTranslator;
            var context = InSchemeInstance.Context;

            // eval value from parameters.
            object outValue = null;
            if (InSchemeInstance.TryGetParamValue(InVarName, out outValue))
            {
                return outValue;
            }

            // eval value from context.
            if (context.TryGetContextValue(InVarName, out outValue))
            {
                return outValue;
            }

            // TODO system var handling (with special name).

            // TEMP Handle Newline/Tab
            // TODO Replace it by system functions.
            if (0 == string.Compare(InVarName, "$NL", true))
            {
                return Environment.NewLine;
            }
            else if (0 == string.Compare(InVarName, "$TAB", true))
            {
                return "\t";
            }
            // ~TEMP

            // TODO log error
            throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Handles retrieval of a member variable.
        /// </summary>
        /// <param name="InSchemeInstance">The translation scheme instance.</param>
        /// <param name="InHolder">The object holding the variable.</param>
        /// <param name="InVarName">The name of the variable.</param>
        /// <returns>The value of the variable or throws an exception if not found.</returns>
        private static object _HandleMemberVar(IInfoTranslateSchemeInstance InSchemeInstance, object InHolder, string InVarName)
        {
            var translator = InSchemeInstance.HostTranslator;
            var context = InSchemeInstance.Context;

            // Create the context of Holder
            var lhsCtx = translator.CreateContext(context, InHolder);
            if (lhsCtx == null)
            {
                // TODO log error
                throw new NotImplementedException("Could not create context for holder.");
            }

            // Get result from the holder context.
            object outValue = null;
            if (!lhsCtx.TryGetContextValue(InVarName, out outValue))
            {
                // TODO log error
                throw new NotImplementedException();
            }

            return outValue;
        }

        /// <summary>
        /// Handles invocation of a global function.
        /// </summary>
        /// <param name="InSchemeInstance">The translation scheme instance.</param>
        /// <param name="InVarName">The name of the function.</param>
        /// <param name="InParams">The parameters passed to the function.</param>
        /// <returns>The result of the function call.</returns>
        private static object _HandleGlobalFunction(IInfoTranslateSchemeInstance InSchemeInstance, string InVarName, object[] InParams)
        {
            var translator = InSchemeInstance.HostTranslator;
            var context = InSchemeInstance.Context;

            // TODO system function handling (with special name).

            // TEMP Handle For function
            // TODO Replace it by system functions.
            if (0 == string.Compare(InVarName, "For", true))
            {
                return _TEMP_HandleForCall(translator, context, InParams);
            }
            // ~TEMP

            // Call schemes
            return translator.TranslateInfo(context, InVarName, InParams);
        }

        /// <summary>
        /// Handles invocation of a member method.
        /// </summary>
        /// <param name="InSchemeInstance">The translation scheme instance.</param>
        /// <param name="InHolder">The object holding the method.</param>
        /// <param name="InVarName">The name of the method.</param>
        /// <param name="InParams">The parameters passed to the method.</param>
        /// <returns>The result of the method call.</returns>
        private static object _HandleMemberMethod(IInfoTranslateSchemeInstance InSchemeInstance, object InHolder, string InVarName, object[] InParams)
        {
            var translator = InSchemeInstance.HostTranslator;
            var context = InSchemeInstance.Context;

            // Create the context of holder
            var lhsCtx = translator.CreateContext(context, InHolder);
            if (lhsCtx == null)
            {
                // TODO log error
                throw new NotImplementedException("Could not create context for holder.");
            }

            // TODO system function handling (with special name).

            // TEMP Handle For function
            // TODO Replace it by system functions.
            if (0 == string.Compare(InVarName, "For", true))
            {
                return _TEMP_HandleForCall(translator, lhsCtx, InParams);
            }
            // ~TEMP

            // Try find the best functor
            return translator.TranslateInfo(lhsCtx, InVarName, InParams);
        }


        // TEMP Handle For function
        private static object _TEMP_HandleForCall(InfoTranslatorAbstract InTranslator, ITranslatingContext InContext, object[] InParams)
        {
            Debug.Assert(InParams.Length >= 3);

            string filter = InParams[0] as string;
            string separator = InParams[1] as string;
            string function = InParams[2] as string;

            Debug.Assert(filter != null && separator != null && function != null);

            // Gather other parameters.
            object[] otherParams = new object[InParams.Length - 3];
            for (int i = 3; i < InParams.Length; i++)
            {
                otherParams[i - 3] = InParams[i];
            }

            List<string> result = new List<string>();
            result.Add("");
            if (InContext is ITranslatingInfoContext)
            {
                (InContext as ITranslatingInfoContext)?.TranslatingInfo.ForeachSubInfo<Info>(
                    info =>
                    {
                        TranslatingInfoContext infoCtx = new TranslatingInfoContext(InContext, info);
                        var subResult = InTranslator.TranslateInfo(infoCtx, function, otherParams);
                        if (subResult.Count > 0)
                        {
                            result[^1] += subResult[0];
                        }
                        for (int i = 1; i < subResult.Count; i++)
                        {
                            result.Add(subResult[i]);
                        }
                        result[^1] += separator;
                    }
                    , info => info.Header.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
                    );
                // Remove the last Separator
                if (result[^1].Length > 0)
                {
                    result[^1] = result[^1].Remove(result[^1].Length - separator.Length);
                }
            }
            else if (InContext is ITranslatingExprContext)
            {
                (InContext as ITranslatingExprContext).TranslatingExprNode.ForeachSubNodes(
                    (key, stNode) =>
                    {
                        if (key.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                        {
                            TranslatingExprContext exprCtx = new TranslatingExprContext(InContext, stNode);
                            var subResult = InTranslator.TranslateInfo(exprCtx, function, otherParams);
                            if (subResult.Count > 0)
                            {
                                result[^1] += subResult[0];
                            }
                            for (int i = 1; i < subResult.Count; i++)
                            {
                                result.Add(subResult[i]);
                            }
                            result[^1] += separator;
                        }
                        return true;
                    }
                    );
                // Remove the last Separator
                if (result[^1].Length > 0)
                {
                    result[^1] = result[^1].Remove(result[^1].Length - separator.Length);
                }
            }
            return result;
        }
        // ~TEMP



    }

}