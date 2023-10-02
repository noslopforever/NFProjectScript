using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{
    public abstract class InfoTranslatorAbstract
    {

        public virtual IReadOnlyList<string> TranslateInfo(ITranslatingContext InTranslatingContext, string InSchemeName)
        {
            var scheme = FindBestScheme(InTranslatingContext, InSchemeName);
            var si = scheme.CreateInstance(this, InTranslatingContext);
            return si.GetResult();
        }

        /// <summary>
        /// Find the best scheme fit the translating Info. 
        /// </summary>
        /// <param name="InContext"></param>
        /// <param name="InSchemeName"></param>
        /// <returns></returns>
        public abstract IInfoTranslateScheme FindBestScheme(ITranslatingContext InTranslatingContext, string InSchemeName);

    }

}
