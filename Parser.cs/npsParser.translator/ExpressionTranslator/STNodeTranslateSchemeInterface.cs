using nf.protoscript.syntaxtree;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Created by ISTNodeTranslateScheme and bound with Translator/ExprTranslateEnvironment/STNodes.
    /// </summary>
    public interface ISTNodeTranslateSchemeInstance
    {

        /// <summary>
        /// Scheme which create this instance.
        /// </summary>
        ISTNodeTranslateScheme Scheme { get; }

        /// <summary>
        /// Translator created this scheme instance.
        /// </summary>
        ExprTranslatorAbstract Translator { get; }

        /// <summary>
        /// Translating Context.
        /// </summary>
        ExprTranslatorAbstract.ITranslatingContext TranslatingContext { get; }

        /// <summary>
        /// GetResult this instance and get result for the target stage which has been decided by translator.
        /// </summary>
        /// <param name="InStageName"></param>
        /// <returns></returns>
        IReadOnlyList<string> GetResult(string InStageName);


        /// <summary>
        /// Prerequisite schemes of this scheme.
        /// </summary>
        IEnumerable<ISTNodeTranslateSchemeInstance> PrerequisiteSchemeInstances { get; }

        /// <summary>
        /// Add prerequisite scheme. Prerequisite schemes will be handled before this scheme.
        /// </summary>
        /// <param name="InKey"></param>
        /// <param name="rhsScheme"></param>
        void AddPrerequisiteScheme(string InKey, ISTNodeTranslateSchemeInstance InPrerequisiteSchemeInstance);

        /// <summary>
        /// Find prerequisite scheme by name.
        /// </summary>
        /// <param name="InKey"></param>
        /// <returns></returns>
        ISTNodeTranslateSchemeInstance FindPrerequisite(string InKey);

    }

    /// <summary>
    /// Translate scheme contains one or several snippets.
    /// Always be converted from setting or configuration files.
    /// </summary>
    public interface ISTNodeTranslateScheme
    {

        /// <summary>
        /// Get translate snippet for the target stage.
        /// </summary>
        /// <param name="InStageName"></param>
        /// <returns></returns>
        STNodeTranslateSnippet GetTranslateSnippet(string InStageName);

        /// <summary>
        /// Create instance to do translate for the target STNode.
        /// </summary>
        /// <returns></returns>
        ISTNodeTranslateSchemeInstance CreateInstance(ExprTranslatorAbstract InTranslator, ExprTranslatorAbstract.ITranslatingContext InContext);

        /// <summary>
        /// Create a instance and set it as the 'proxy' of another instance.
        /// </summary>
        /// <param name="InSchemeInstanceToBeProxied"></param>
        /// <returns></returns>
        ISTNodeTranslateSchemeInstance CreateProxyInstance(ISTNodeTranslateSchemeInstance InSchemeInstanceToBeProxied);


    }


}
