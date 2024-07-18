namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// Base class of all elements with sub-snippets
    /// </summary>
    public abstract class ElementWithSubSnippets
    {
        public ElementWithSubSnippets(params InfoTranslateSnippet.IElement[] InSubElements)
        {
            SubSnippet = new InfoTranslateSnippet(InSubElements);
            SubScheme = new InfoTranslateSchemeDefault(SubSnippet);
        }

        public InfoTranslateSnippet SubSnippet { get; }

        public IInfoTranslateScheme SubScheme { get; }

    }

}