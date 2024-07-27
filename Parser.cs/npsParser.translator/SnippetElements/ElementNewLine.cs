using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// A special 'NewLine' element.
    /// </summary>
    public sealed class ElementNewLine
        : InfoTranslateSchemeDefault.IElement
    {
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            throw new InvalidOperationException();
        }
    }

}