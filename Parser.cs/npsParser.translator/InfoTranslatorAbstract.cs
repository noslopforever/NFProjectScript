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
    /// Abstract base class for translators that handle the translation of Info objects.
    /// </summary>
    public abstract class InfoTranslatorAbstract
    {
        /// <summary>
        /// Attempts to find the best translation scheme that fits the provided translating context and gets the results.
        /// </summary>
        /// <param name="InTranslatingContext">The translating context.</param>
        /// <param name="InSchemeName">The name of the translation scheme to use.</param>
        /// <param name="InParams">Optional parameters for the scheme.</param>
        /// <returns>A read-only list of strings representing the translation result.</returns>
        public virtual IReadOnlyList<string> TranslateInfo(ITranslatingContext InTranslatingContext, string InSchemeName, object[] InParams = null)
        {
            var scheme = FindBestScheme(InTranslatingContext, InSchemeName);
            if (scheme == null)
            {
                // TODO: Log the error and potentially throw a more specific exception.
                throw new InvalidOperationException("No suitable translation scheme found.");
            }
            var si = scheme.CreateInstance(this, InTranslatingContext, InParams);
            return si.GetResult();
        }

        /// <summary>
        /// Finds the best translation scheme that fits the translating context.
        /// </summary>
        /// <param name="InTranslatingContext">The translating context.</param>
        /// <param name="InSchemeName">The name of the translation scheme to find.</param>
        /// <returns>The best fitting translation scheme or null if none is found.</returns>
        public abstract IInfoTranslateScheme FindBestScheme(ITranslatingContext InTranslatingContext, string InSchemeName);

        /// <summary>
        /// Creates a translating context for the given Info.
        /// </summary>
        /// <param name="InParentContext">The parent translating context.</param>
        /// <param name="InInfo">The Info to create a context for.</param>
        /// <returns>A new translating context for the Info.</returns>
        public virtual ITranslatingInfoContext CreateContext(ITranslatingContext InParentContext, Info InInfo)
        {
            return new TranslatingInfoContext(InParentContext, InInfo);
        }

        /// <summary>
        /// Creates a translating context for the given expression node.
        /// </summary>
        /// <param name="InParentContext">The parent translating context.</param>
        /// <param name="InExprNode">The expression node to create a context for.</param>
        /// <returns>A new translating context for the expression node.</returns>
        public virtual ITranslatingExprContext CreateContext(ITranslatingContext InParentContext, ISyntaxTreeNode InExprNode)
        {
            return new TranslatingExprContext(InParentContext, InExprNode);
        }

        /// <summary>
        /// Creates a translating context for a generic object.
        /// </summary>
        /// <param name="InParentContext">The parent translating context.</param>
        /// <param name="InGenericObject">The generic object to create a context for.</param>
        /// <returns>A new translating context for the generic object, or null if unsupported.</returns>
        public virtual ITranslatingContext CreateContext(ITranslatingContext InParentContext, object InGenericObject)
        {
            if (InGenericObject is Info)
            {
                return CreateContext(InParentContext, InGenericObject as Info);
            }
            else if (InGenericObject is ISyntaxTreeNode)
            {
                return CreateContext(InParentContext, InGenericObject as ISyntaxTreeNode);
            }

            return null;
        }


    }
}