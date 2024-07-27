using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// Element to write all sub-elements in a new block which has a increment indent than the current block.
    /// </summary>
    public class ElementIndentBlock
        : InfoTranslateSchemeDefault.IElement
    {
        public ElementIndentBlock(params InfoTranslateSchemeDefault.IElement[] InSubElements)
        {
            SubScheme = new InfoTranslateSchemeDefault(InSubElements);
        }

        /// <summary>
        /// Sub snippet
        /// </summary>
        public InfoTranslateSchemeDefault SubScheme { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Get sub results, insert spaces at the very first of each line, and then return.

            var subResults = SubScheme.Apply(InHolderSchemeInstance);
            List<string> results = new List<string>(subResults.Count);
            foreach (var item in subResults)
            {
                results.Add(item.Insert(0, "    "));
            }
            return results;
        }
    }

}