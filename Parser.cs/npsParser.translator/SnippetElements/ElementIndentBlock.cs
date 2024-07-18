using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// Element to write all sub-elements in a new block which has a increment indent than the current block.
    /// </summary>
    public class ElementIndentBlock
        : InfoTranslateSnippet.IElement
    {
        public ElementIndentBlock(params InfoTranslateSnippet.IElement[] InSubElements)
        {
            SubSnippet = new InfoTranslateSnippet(InSubElements);
        }

        /// <summary>
        /// Sub snippet
        /// </summary>
        public InfoTranslateSnippet SubSnippet { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Get sub results, insert spaces at the very first of each line, and then return.

            var subResults = SubSnippet.Apply(InHolderSchemeInstance);
            List<string> results = new List<string>(subResults.Count);
            foreach (var item in subResults)
            {
                results.Add(item.Insert(0, "    "));
            }
            return results;
        }
    }

}