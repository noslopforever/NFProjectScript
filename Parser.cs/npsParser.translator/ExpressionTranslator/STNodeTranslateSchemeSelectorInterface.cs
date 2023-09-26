namespace nf.protoscript.translator.expression
{
    /// <summary>
    /// Scheme selector to select the best InScheme for the target context.
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

    }


    /// <summary>
    /// MemberAccess selector interface, to select scheme when handling the member-access operation.
    /// </summary>
    public interface IMemberAccessSchemeSelector : ISTNodeTranslateSchemeSelector
    {

        bool IsMatch(EExprVarAccessType InAccessType, TypeInfo InHostType, string InMemberName, ElementInfo InMemberElementInfo);

    }


}
