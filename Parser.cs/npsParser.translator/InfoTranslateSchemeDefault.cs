using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    public partial class InfoTranslateSchemeDefault
        : IInfoTranslateScheme
    {
        public InfoTranslateSchemeDefault(InfoTranslateSnippet InSnippet)
        {
            Snippet = InSnippet;
        }

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
