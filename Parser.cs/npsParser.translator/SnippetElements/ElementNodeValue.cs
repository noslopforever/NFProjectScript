using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// A snippet element to get value strings from the translating Context.
    /// </summary>
    public class ElementNodeValue
        : InfoTranslateSnippet.IElement
    {
        public ElementNodeValue(string InKey)
        {
            Key = InKey;
        }

        public string Key { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Find data value from SI's context
            var ctxValStr = InHolderSchemeInstance.Context.GetContextValueString(Key);
            return new string[] { ctxValStr };
        }
    }

}