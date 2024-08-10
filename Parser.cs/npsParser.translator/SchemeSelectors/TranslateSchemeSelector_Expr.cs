using nf.protoscript.syntaxtree;
using nf.protoscript.translator.DefaultScheme.Elements;
using nf.protoscript.translator.DefaultScheme.Elements.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator.SchemeSelectors
{
    /// <summary>
    /// Represents a selector that chooses a translation scheme based on a common condition expression.
    /// </summary>
    public class TranslateSchemeSelector_Expr : IInfoTranslateSchemeSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateSchemeSelector_Expr"/> class with a pre-parsed condition expression.
        /// </summary>
        /// <param name="InPriority">The priority of the selector.</param>
        /// <param name="InExpr">The parsed condition expression.</param>
        /// <param name="InScheme">The translation scheme to apply when the condition is met.</param>
        public TranslateSchemeSelector_Expr(int InPriority, ISyntaxTreeNode InExpr, IInfoTranslateScheme InScheme)
        {
            Priority = InPriority;
            Scheme = InScheme;

            _conditionExpr = InExpr;
        }

        // Begin IInfoTranslateSchemeSelector implementation
        /// <inheritdoc />
        public int Priority { get; }

        /// <inheritdoc />
        public IInfoTranslateScheme Scheme { get; }

        /// <inheritdoc />
        public bool IsMatch(ITranslatingContext InContext)
        {
            if (_conditionExpr == null)
            {
                // TODO log warning.
                return false;
            }

            // Evaluate the condition expression using an expression executor.
            ExprExecutor executor = new ExprExecutor(
                (contextObj, stNode, holder, varName) =>
                {
                    var context = contextObj as ITranslatingContext;
                    if (holder != null)
                    {
                        return _HandleMemberVar(context, holder, varName);
                    }
                    return _HandleGlobalVar(context, varName);
                },
                (contextObj, stNode, holder, varName, paramList) =>
                {
                    var context = contextObj as ITranslatingContext;
                    if (holder != null)
                    {
                        return _HandleMemberMethod(context, holder, varName, paramList);
                    }
                    return _HandleGlobalFunction(context, varName, paramList);
                }
            );

            object result = executor.EvalValue(InContext, _conditionExpr);
            bool resultBool = Convert.ToBoolean(result);
            return resultBool;
        }

        /// <summary>
        /// Handles accessing global variables within the context.
        /// </summary>
        /// <param name="InContext">The translating context.</param>
        /// <param name="InVarName">The name of the variable.</param>
        /// <returns>The value of the global variable.</returns>
        private object _HandleGlobalVar(ITranslatingContext InContext, string InVarName)
        {
            object outValue = null;
            if (InContext.TryGetContextValue(InVarName, out outValue))
            {
                return outValue;
            }

            throw new ArgumentException($"Cannot find property '{InVarName}' in Context '{InContext}'.");
        }

        /// <summary>
        /// Handles accessing member variables of an object within the context.
        /// </summary>
        /// <param name="InContext">The translating context.</param>
        /// <param name="InHolder">The object holding the member variable.</param>
        /// <param name="InVarName">The name of the member variable.</param>
        /// <returns>The value of the member variable.</returns>
        private object _HandleMemberVar(ITranslatingContext InContext, object InHolder, string InVarName)
        {
            throw new NotImplementedException("Handling member variables is not yet implemented.");
        }

        /// <summary>
        /// Handles calling global functions within the context.
        /// </summary>
        /// <param name="InContext">The translating context.</param>
        /// <param name="InVarName">The name of the function.</param>
        /// <param name="InParams">The parameters to pass to the function.</param>
        /// <returns>The result of the function call.</returns>
        private object _HandleGlobalFunction(ITranslatingContext InContext, string InVarName, object[] InParams)
        {
            // TEMP
            if (0 == string.Compare(InVarName, "IsValid", true))
            {
                return InContext != null;
            }
            else if (0 == string.Compare(InVarName, "HasSub", true))
            {
                if (InParams.Length != 1)
                {
                    // TODO log warning
                    throw new ArgumentException("Need a parameter.");
                }
                string strParam = InParams[0] as string;
                if (strParam == null)
                {
                    // TODO log warning
                    throw new ArgumentException("Need a string parameter.");
                }

                var infoCtx = InContext as ITranslatingInfoContext;
                if (infoCtx == null)
                {
                    throw new ArgumentException("Need a Info context.");
                }

                if (infoCtx.TranslatingInfo == null)
                {
                    throw new ArgumentException("Need a valid Info context.");
                }

                return infoCtx.TranslatingInfo.HasSubInfoWithHeader<Info>(strParam);
            }
            // ~ TEMP

            throw new NotImplementedException("Handling global functions is not yet implemented.");
        }

        /// <summary>
        /// Handles calling member methods of an object within the context.
        /// </summary>
        /// <param name="InContext">The translating context.</param>
        /// <param name="InHolder">The object holding the member method.</param>
        /// <param name="InVarName">The name of the member method.</param>
        /// <param name="InParams">The parameters to pass to the method.</param>
        /// <returns>The result of the method call.</returns>
        private object _HandleMemberMethod(ITranslatingContext InContext, object InHolder, string InVarName, object[] InParams)
        {
            // TEMP
            if (0 == string.Compare(InVarName, "IsValid", true))
            {
                return InHolder != null;
            }
            // ~ TEMP

            throw new NotImplementedException("Handling member methods is not yet implemented.");
        }

        // ~ End IInfoTranslateSchemeSelector implementation

        /// <summary>
        /// The parsed condition expression.
        /// </summary>
        private ISyntaxTreeNode _conditionExpr = null;
    }
}