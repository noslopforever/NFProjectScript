using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{

    /// <summary>
    /// A special 'NewLine' element.
    /// </summary>
    public sealed class ElementNewLine
        : InfoTranslateSnippet.IElement
    {
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// A snippet element which contains only constant-string.
    /// </summary>
    public class ElementConstString
        : InfoTranslateSnippet.IElement
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
            var ctxValStr = InHolderSchemeInstance.GetContextVarValueString(Key);
            return new string[] { ctxValStr };
        }
    }


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

    public class ElementForEachSubCall
        : InfoTranslateSnippet.IElement
    {
        public ElementForEachSubCall(string InSchemeName, string InHeaderFilter)
        {
            SchemeName = InSchemeName;
            HeaderFilter = InHeaderFilter;
        }

        /// <summary>
        /// The scheme to call for each sub Info
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// Which infos should be selected
        /// </summary>
        public string HeaderFilter { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;

            List<string> result = new List<string>();
            InHolderSchemeInstance.ContextInfo.ForeachSubInfo<Info>(
                i => {
                    var scheme = translator.FindBestScheme(i, SchemeName);
                    var si = scheme.CreateInstance(translator, i);
                    result.AddRange(si.GetResult());
                }
                , i => i.Header.Contains(HeaderFilter)
                );
            return result;
        }

    }



}