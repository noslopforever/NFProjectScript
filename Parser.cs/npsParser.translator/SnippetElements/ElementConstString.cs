using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultScheme.Elements
{
    /// <summary>
    /// Represents an element containing a constant string value.
    /// </summary>
    public class ElementConstString : InfoTranslateSchemeDefault.IElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementConstString"/> class.
        /// </summary>
        /// <param name="InValue">The constant string value to store.</param>
        public ElementConstString(string InValue)
        {
            Value = InValue;
        }

        /// <summary>
        /// Gets the constant string value contained in this element.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the string representation of the element.
        /// </summary>
        /// <returns>The constant string value.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Applies the element by returning its value as a single-item list.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The holder scheme instance.</param>
        /// <returns>A read-only list containing the constant string value.</returns>
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            return new string[] { Value };
        }
    }
}