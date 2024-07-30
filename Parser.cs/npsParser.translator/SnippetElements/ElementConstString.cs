using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultScheme.Elements
{
    /// <summary>
    /// A snippet element which contains only constant-string.
    /// </summary>
    public class ElementConstString
        : InfoTranslateSchemeDefault.IElement
    {
        public string Value { get; }
        public ElementConstString(string InValue)
        {
            Value = InValue;
        }
        public override string ToString()
        {
            return Value;
        }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            return new string[] { Value };
        }
    }

}