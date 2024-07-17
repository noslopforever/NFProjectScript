using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    /// <summary>
    /// Info translator.
    /// </summary>
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
            if (scheme == null)
            {
                // TODO log error
                throw new NotImplementedException();
                return new string[] { };
            }
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
        /// Create TranslatingContext for the target Info.
        /// </summary>
        /// <param name="InParentContext"></param>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public virtual ITranslatingInfoContext CreateContext(ITranslatingContext InParentContext, Info InInfo)
        {
            return new TranslatingInfoContext(InParentContext, InInfo);
        }

        /// <summary>
        /// Create TranslatingContext for the target expression.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="inNode"></param>
        /// <returns></returns>
        public virtual ITranslatingExprContext CreateContext(ITranslatingContext InParentContext, ISyntaxTreeNode InExprNode)
        {
            return new TranslatingExprContext(InParentContext, InExprNode);
        }

    }

}
