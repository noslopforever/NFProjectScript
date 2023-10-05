using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{
    public abstract class InfoTranslatorAbstract
    {
        /// <summary>
        /// Try find the best scheme to fit the target InTranslatingContext, and get results from it.
        /// </summary>
        /// <param name="InTranslatingContext"></param>
        /// <param name="InSchemeName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create an ExprTranslator to translate expressions.
        /// </summary>
        /// <param name="InTranslatorType"></param>
        /// <returns></returns>
        public abstract expression.ExprTranslatorAbstract LoadExprTranslator(string InTranslatorType);

    }

}
