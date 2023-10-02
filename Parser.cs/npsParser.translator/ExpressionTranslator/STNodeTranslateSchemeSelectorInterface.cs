using nf.protoscript.syntaxtree;

namespace nf.protoscript.translator.expression
{
    /// <summary>
    /// Scheme selector to select the best InScheme for the target translating-context.
    /// </summary>
    public interface ISTNodeTranslateSchemeSelector
    {
        /// <summary>
        /// Name of the selector
        /// </summary>
        string SelectorName { get; }

        /// <summary>
        /// Priority of the selector, bigger runs earlier, default is 0.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Scheme returned by the selector
        /// </summary>
        ISTNodeTranslateScheme Scheme { get; }

        /// <summary>
        /// Is Conditions matched.
        /// </summary>
        /// <param name="InContext"></param>
        /// <returns></returns>
        bool IsMatch(ITranslatingContext InContext);

    }


}
