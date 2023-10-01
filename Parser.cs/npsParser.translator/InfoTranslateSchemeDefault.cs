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

        public IInfoTranslateSchemeInstance CreateInstance(InfoTranslatorAbstract InTranslator, Info InContextInfo)
        {
            return new Instance(InTranslator, this, InContextInfo);
        }

        public InfoTranslateSnippet Snippet { get; }

    }

}
