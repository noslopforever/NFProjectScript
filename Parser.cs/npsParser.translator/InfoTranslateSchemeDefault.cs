using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{
    /// <summary>
    /// Defines a translation scheme containing one or more snippets. These snippets are typically loaded from configuration files.
    /// During translation, variables within the snippets are replaced with data from the translating Info to produce the final results.
    /// </summary>
    public partial class InfoTranslateSchemeDefault
        : IInfoTranslateScheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoTranslateSchemeDefault"/> class with a set of elements.
        /// </summary>
        /// <param name="InElementArray">An array of elements that define the snippet.</param>
        public InfoTranslateSchemeDefault(params InfoTranslateSnippet.IElement[] InElementArray)
        {
            Snippet = new InfoTranslateSnippet(InElementArray);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoTranslateSchemeDefault"/> class with parameter names and a set of elements.
        /// </summary>
        /// <param name="InParams">An array of parameter names.</param>
        /// <param name="InElementArray">An array of elements that define the snippet.</param>
        public InfoTranslateSchemeDefault(string[] InParams, params InfoTranslateSnippet.IElement[] InElementArray)
        {
            _params = InParams;
            Snippet = new InfoTranslateSnippet(InElementArray);
        }

        // Begin IInfoTranslateScheme interfaces
        /// <see cref="IInfoTranslateScheme.ParamNum"/>
        public int ParamNum => _params.Length;
        /// <see cref="IInfoTranslateScheme.ParamNames"/>
        public string[] ParamNames => _params;
        /// <see cref="IInfoTranslateScheme.Snippet"/>
        public InfoTranslateSnippet Snippet { get; }
        /// <see cref="IInfoTranslateScheme.GetParamIndex"/>
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
        /// <see cref="IInfoTranslateScheme.CreateInstance"/>
        public IInfoTranslateSchemeInstance CreateInstance(InfoTranslatorAbstract InTranslator, ITranslatingContext InContext, params object[] InParams)
        {
            return new Instance(this, InTranslator, InContext, InParams);
        }
        // ~ End IInfoTranslateScheme interfaces

        private string[] _params = new string[0];

    }

}