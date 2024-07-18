using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    /// <summary>
    /// Default translate scheme class. Use snippets to translate the target context.
    /// </summary>
    public partial class InfoTranslateSchemeDefault
        : IInfoTranslateScheme
    {
        public InfoTranslateSchemeDefault(InfoTranslateSnippet InSnippet)
        {
            Snippet = InSnippet;
        }

        public InfoTranslateSchemeDefault(params InfoTranslateSnippet.IElement[] InElementArray)
        {
            Snippet = new InfoTranslateSnippet(InElementArray);
        }

        /// <summary>
        /// Snippets constructs this scheme.
        /// </summary>
        public InfoTranslateSnippet Snippet { get; }

        public IInfoTranslateSchemeInstance CreateInstance(InfoTranslatorAbstract InTranslator, ITranslatingContext InContext)
        {
            return new Instance(this, InTranslator, InContext);
        }

        public IInfoTranslateSchemeInstance CreateProxyInstance(IInfoTranslateSchemeInstance InSchemeInstanceToBeProxied)
        {
            return Instance.CreateProxyInstance(this, InSchemeInstanceToBeProxied);

        }

    }

}
