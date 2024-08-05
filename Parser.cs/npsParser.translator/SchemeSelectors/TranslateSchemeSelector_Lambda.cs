using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator.SchemeSelectors
{

    /// <summary>
    /// Represents a scheme selector that uses a lambda expression to determine whether a scheme should be selected.
    /// </summary>
    public class TranslateSchemeSelector_Lambda : IInfoTranslateSchemeSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateSchemeSelector_Lambda"/> class.
        /// </summary>
        /// <param name="InPriority">The priority of the selector. Higher values indicate higher priority.</param>
        /// <param name="InConditionChecker">A lambda expression that checks the translating context and returns true if the scheme should be selected.</param>
        /// <param name="InScheme">The scheme to select if the condition is met.</param>
        public TranslateSchemeSelector_Lambda(
            int InPriority,
            Func<ITranslatingContext, bool> InConditionChecker,
            IInfoTranslateScheme InScheme)
        {
            Priority = InPriority;
            _conditionChecker = InConditionChecker ?? throw new ArgumentNullException(nameof(InConditionChecker));
            Scheme = InScheme ?? throw new ArgumentNullException(nameof(InScheme));
        }

        /// <summary>
        /// Gets the priority of the selector.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Gets the scheme associated with this selector.
        /// </summary>
        public IInfoTranslateScheme Scheme { get; }

        /// <summary>
        /// Determines whether the scheme should be selected based on the translating context.
        /// </summary>
        /// <param name="InContext">The translating context to check.</param>
        /// <returns>true if the scheme should be selected; otherwise, false.</returns>
        public bool IsMatch(ITranslatingContext InContext)
        {
            return _conditionChecker(InContext);
        }

        /// <summary>
        /// The lambda expression used to check the conditions for selecting the scheme.
        /// </summary>
        private readonly Func<ITranslatingContext, bool> _conditionChecker;
    }
}
