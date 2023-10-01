using nf.protoscript.translator.expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    /// <summary>
    /// Translate Scheme contains one or more snippets which always be constructed by loading configuration files.
    /// When translating, replace variables in the snippets by the translating Info to get the final results.
    /// </summary>
    public interface IInfoTranslateScheme
    {

        /// <summary>
        /// Create instance to 
        /// </summary>
        /// <param name="InTranslator"></param>
        /// <returns></returns>
        IInfoTranslateSchemeInstance CreateInstance(InfoTranslatorAbstract InTranslator, Info InContextInfo);

    }

    /// <summary>
    /// Scheme instance created by a scheme and bind context variables.
    /// A scheme should create many instances for different translating Info nodes.
    /// </summary>
    public interface IInfoTranslateSchemeInstance
    {
        /// <summary>
        /// HostTranslator translating this scheme instance.
        /// </summary>
        InfoTranslatorAbstract HostTranslator { get; }

        /// <summary>
        /// Info bound with this scheme instance.
        /// </summary>
        Info ContextInfo { get; }

        /// <summary>
        /// Apply the scheme and get translated codes
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<string> GetResult();

        /// <summary>
        /// Get the value of the Context variable bound with this scheme instance.
        /// </summary>
        /// <param name="InContextVarName"></param>
        /// <returns></returns>
        string GetContextVarValueString(string InContextVarName);

    }


    /// <summary>
    /// Scheme selector to select the best Scheme for the target translating-context.
    /// </summary>
    public interface IInfoTranslateSchemeSelector
    {
        /// <summary>
        /// Name of the selector
        /// </summary>
        string SelectorName { get; }

        /// <summary>
        /// Priority of the selector, bigger runs earlier, default is 0.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Scheme returned by the selector
        /// </summary>
        IInfoTranslateScheme Scheme { get; }

        /// <summary>
        /// Is Conditions matched.
        /// </summary>
        /// <param name="InContext"></param>
        /// <returns></returns>
        bool IsMatch(Info InContext);

    }


}
