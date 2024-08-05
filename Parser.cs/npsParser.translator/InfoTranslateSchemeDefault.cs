using nf.protoscript.translator.DefaultScheme.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace nf.protoscript.translator.DefaultScheme
{
    /// <summary>
    /// Defines a default translation scheme containing one or more snippets. 
    /// These snippets are typically loaded from configuration files.
    /// During translation, variables within the snippets are replaced with data from the translating Info to produce the final results.
    /// </summary>
    public partial class InfoTranslateSchemeDefault
        : IInfoTranslateScheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoTranslateSchemeDefault"/> class with a set of elements.
        /// </summary>
        /// <param name="InElementArray">An array of elements that define the snippet.</param>
        public InfoTranslateSchemeDefault(params IElement[] InElementArray)
        {
            SnippetElements = InElementArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoTranslateSchemeDefault"/> class with parameter names and a set of elements.
        /// </summary>
        /// <param name="InParams">An array of parameter names.</param>
        /// <param name="InElementArray">An array of elements that define the snippet.</param>
        public InfoTranslateSchemeDefault(string[] InParams, params IElement[] InElementArray)
        {
            _params = InParams;
            SnippetElements = InElementArray;
        }

        //public InfoTranslateSchemeDefault(string[] InParams, params string[] InSchemeCodes)
        //{
        //    _params = InParams;
        //    var resultElems = new List<IElement>();
        //    foreach(var code in InSchemeCodes)
        //    {
        //        var elements = ElementParser.ParseElements(code);
        //        resultElems.AddRange(elements);
        //    }
        //    SnippetElements = resultElems.ToArray();
        //}

        //public InfoTranslateSchemeDefault(string InSchemeCode)
        //{
        //    var resultElems = new List<IElement>();
        //    var elements = ElementParser.ParseElements(InSchemeCode);
        //    resultElems.AddRange(elements);
        //    SnippetElements = resultElems.ToArray();
        //}

        // Begin IInfoTranslateScheme interfaces
        /// <inheritdoc />
        public int ParamNum => _params.Length;
        /// <inheritdoc />
        public string[] ParamNames => _params;
        /// <inheritdoc />
        public int GetParamIndex(string InName)
        {
            for (int i = 0; i < _params.Length; ++i)
            {
                if (string.Equals(_params[i], InName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <inheritdoc />
        public IInfoTranslateSchemeInstance CreateInstance(InfoTranslatorAbstract InTranslator, ITranslatingContext InContext, params object[] InParams)
        {
            return new Instance(this, InTranslator, InContext, InParams);
        }
        // ~ End IInfoTranslateScheme interfaces


        /// <summary>
        /// Represents an element that can be applied to the translation scheme.
        /// A scheme is composed of one or more elements, each implementing this interface,
        /// which collectively define the behavior of the translation process.
        /// </summary>
        public interface IElement
        {
            /// <summary>
            /// Applies the element to the translation scheme instance.
            /// </summary>
            /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
            /// <returns>A read-only list of strings representing the result of the element.</returns>
            public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance);
        }

        /// <summary>
        /// All elements composing this scheme.
        /// </summary>
        IElement[] SnippetElements { get; }

        /// <summary>
        /// Applies all elements to the given translation scheme instance and returns the translated codes.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme to apply the elements to.</param>
        /// <returns>A read-only list of strings representing the translated codes.</returns>
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            List<string> codeLines = new List<string>();
            codeLines.Add("");
            int writingLineIndex = 0;

            for (int i = 0; i < SnippetElements.Length; i++)
            {
                var snippetElem = SnippetElements[i];

                // Get results applied by the element.
                var elemCodeLns = snippetElem.Apply(InHolderSchemeInstance).ToArray();
                if (elemCodeLns.Length == 0)
                {
                    continue;
                }

                // The first line of the snippet will be applied to the current writing-line.
                // Other lines will be pushed to new lines.
                codeLines[writingLineIndex] += elemCodeLns[0];
                for (int elemLnIndex = 1; elemLnIndex < elemCodeLns.Length; elemLnIndex++)
                {
                    codeLines.Add($"{elemCodeLns[elemLnIndex]}");
                    writingLineIndex++;
                }
            }

            return codeLines;
        }

        /// <summary>
        /// Stores the parameter names for this scheme.
        /// </summary>
        private string[] _params = new string[0];

    }

}