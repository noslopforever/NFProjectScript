using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// A snippet element to get value strings from the parameter.
    /// </summary>
    public class ElementParamValue
        : InfoTranslateSnippet.IElement
    {
        public ElementParamValue(string InKey)
        {
            Key = InKey;
        }

        public string Key { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Find data value from SI's context
            var ctxValStr = InHolderSchemeInstance.GetParamValue(Key);
            return ctxValStr;
        }
    }

}