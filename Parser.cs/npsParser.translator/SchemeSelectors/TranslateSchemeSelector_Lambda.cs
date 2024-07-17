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
    public class TranslateSchemeSelector_Lambda
        : IInfoTranslateSchemeSelector
    {
        public TranslateSchemeSelector_Lambda(
            int InPriority
            , Func<ITranslatingContext, bool> InConditionChecker
            , IInfoTranslateScheme InScheme
            )
        {
            Priority = InPriority;
            _conditionChecker = InConditionChecker;
            Scheme = InScheme;
        }

        // Begin IInfoTranslateSchemeSelector
        public int Priority { get; }
        public IInfoTranslateScheme Scheme { get; }

        public bool IsMatch(ITranslatingContext InContext)
        {
            return _conditionChecker(InContext);
        }
        // ~ End IInfoTranslateSchemeSelector

        Func<ITranslatingContext, bool> _conditionChecker;

    }

}
