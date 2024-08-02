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
    /// The Lambda Selector, which selects the scheme by lambda expressions.
    /// </summary>
    public class TranslateSchemeSelector_Expr
        : IInfoTranslateSchemeSelector
    {
        public TranslateSchemeSelector_Expr(
            int InPriority
            , string InConditionCode
            , IInfoTranslateScheme InScheme
            )
        {
            Priority = InPriority;
            Scheme = InScheme;

            _conditionCode = InConditionCode;
            if (!string.IsNullOrWhiteSpace(_conditionCode))
            {
                _conditionExpr = ElementExprParser.ParseCode(InConditionCode);
            }
        }
        public TranslateSchemeSelector_Expr(
            int InPriority
            , ISyntaxTreeNode InExpr
            , IInfoTranslateScheme InScheme
            )
        {
            Priority = InPriority;
            Scheme = InScheme;

            _conditionCode = InExpr.ToString();
            if (!string.IsNullOrWhiteSpace(_conditionCode))
            {
                _conditionExpr = InExpr;
            }
        }


        // Begin IInfoTranslateSchemeSelector
        /// <inheritdoc />
        public int Priority { get; }

        /// <inheritdoc />
        public IInfoTranslateScheme Scheme { get; }

        /// <inheritdoc />
        public bool IsMatch(ITranslatingContext InContext)
        {
            if (_conditionExpr == null)
            {
                return true;
            }
            ExprExecutor executor = new ExprExecutor(
                (contextObj, stNode, holder, varName) =>
                {
                    var context = contextObj as ITranslatingContext;
                    if (holder != null)
                    {
                        return _HandleMemberVar(context, holder, varName);
                    }
                    return _HandleGlobalVar(context, varName);
                }
                , (contextObj, stNode, holder, varName, paramList) =>
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

        private object _HandleGlobalVar(ITranslatingContext InContext, string InVarName)
        {
            object outValue = null;
            if (InContext.TryGetContextValue(InVarName, out outValue))
            {
                return outValue;
            }

            throw new ArgumentException($"Cannot find property {InVarName} in Context {InContext}.");
        }

        private object _HandleMemberVar(ITranslatingContext InContext, object InHolder, string InVarName)
        {
            throw new NotImplementedException();
        }

        private object _HandleGlobalFunction(ITranslatingContext InContext, string InVarName, object[] InParams)
        {
            throw new NotImplementedException();
        }

        private object _HandleMemberMethod(ITranslatingContext InContext, object InHolder, string InVarName, object[] InParams)
        {
            throw new NotImplementedException();
        }

        // ~ End IInfoTranslateSchemeSelector

        /// <summary>
        /// Condition code
        /// </summary>
        string _conditionCode = "";

        /// <summary>
        /// Condition expression
        /// </summary>
        ISyntaxTreeNode _conditionExpr = null;

    }

}
