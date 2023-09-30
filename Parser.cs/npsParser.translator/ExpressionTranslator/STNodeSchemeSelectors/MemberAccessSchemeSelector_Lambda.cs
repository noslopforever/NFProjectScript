using System;
using nf.protoscript.syntaxtree;
using nf.protoscript.translator.expression;

namespace nf.protoscript.translator.expression.schemeSelectors
{
    /// <summary>
    /// Member Access selector implemented by Lambda expressions.
    /// </summary>
    public class STNodeTranslateSchemeSelector_Lambda
        : ISTNodeTranslateSchemeSelector
    {
        public STNodeTranslateSchemeSelector_Lambda(
            string InSelectorName
            , int InPriority
            , Func<ExprTranslatorAbstract.ITranslatingContext, bool> InConditionChecker
            , ISTNodeTranslateScheme InScheme
            )
        {
            SelectorName = InSelectorName;
            Priority = InPriority;
            Scheme = InScheme;
            ConditionChecker = InConditionChecker;
        }

        // Begin ISTNodeTranslateSchemeSelector interfaces
        public string SelectorName { get; } = "";
        public int Priority { get; } = 0;
        public ISTNodeTranslateScheme Scheme { get; }

        public bool IsMatch(ExprTranslatorAbstract.ITranslatingContext InContext)
        {
            return ConditionChecker(InContext);
        }

        // ~ End ISTNodeTranslateSchemeSelector interfaces

        /// <summary>
        /// The lambda to check the codition.
        /// </summary>
        public Func<ExprTranslatorAbstract.ITranslatingContext, bool> ConditionChecker { get; }

    }


}
