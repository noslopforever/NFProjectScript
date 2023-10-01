using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{
    public abstract class InfoTranslatorAbstract
    {

        public virtual IReadOnlyList<string> TranslateInfo(Info InTargetInfo, string InSchemeName)
        {
            var scheme = FindBestScheme(InTargetInfo, InSchemeName);
            var si = scheme.CreateInstance(this, InTargetInfo);
            return si.GetResult();
        }

        /// <summary>
        /// Find the best scheme fit the translating Info. 
        /// </summary>
        /// <param name="InContext"></param>
        /// <param name="InSchemeName"></param>
        /// <returns></returns>
        public abstract IInfoTranslateScheme FindBestScheme(Info InContext, string InSchemeName);

    }

}
