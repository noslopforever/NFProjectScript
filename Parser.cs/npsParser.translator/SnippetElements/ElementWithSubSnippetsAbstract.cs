namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// Base class of all elements with sub-snippets
    /// </summary>
    public abstract class ElementWithSubSnippets
    {
        public ElementWithSubSnippets(params InfoTranslateSnippet.IElement[] InSubElements)
        {
            SubScheme = new InfoTranslateSchemeDefault(InSubElements);
        }

        /// <summary>
        /// Scheme applied to each sub-info.
        /// </summary>
        public IInfoTranslateScheme SubScheme { get; }

    }

}