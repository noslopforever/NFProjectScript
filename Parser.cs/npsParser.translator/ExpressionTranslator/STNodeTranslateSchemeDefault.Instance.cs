using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{

    public partial class STNodeTranslateSchemeDefault
    {

        /// <summary>
        /// The Instance which implements ISTNodeTranslateSchemeInstance, be created by STNodeTranslateSchemeDefault.
        /// </summary>
        public class Instance
            : ISTNodeTranslateSchemeInstance
        {
            public Instance(STNodeTranslateSchemeDefault InScheme
                , ExprTranslatorAbstract InTranslator
                , ExprTranslatorAbstract.ITranslatingContext InContext
                )
            {
                Scheme = InScheme;
                Translator = InTranslator;
                TranslatingContext = InContext;
            }

            public static ISTNodeTranslateSchemeInstance CreateProxyInstance(STNodeTranslateSchemeDefault InScheme, ISTNodeTranslateSchemeInstance InProxySI)
            {
                Instance inst = new Instance(InScheme, InProxySI.Translator, InProxySI.TranslatingContext)
                {
                    ProxyInstance = InProxySI
                };
                return inst;
            }

            /// <summary>
            /// The Proxy SI if this instance is a proxy instance.
            /// </summary>
            public ISTNodeTranslateSchemeInstance ProxyInstance { get; private set; }

            // Begin ISTNodeTranslateSchemeInstance interfaces

            public ISTNodeTranslateScheme Scheme { get; }
            public ExprTranslatorAbstract Translator { get; }
            public ExprTranslatorAbstract.ITranslatingContext TranslatingContext { get; }

            public IReadOnlyList<string> GetResult(string InStageName)
            {
                // Find cache first
                IReadOnlyList<string> result = null;
                if (_stageResultCaches.TryGetValue(InStageName, out result))
                {
                    return result;
                }

                // Not found in cache, calculate the result.

                // Try get snippet of this stage, then apply it.
                IReadOnlyList<string> schemeCodes = new string[0];
                var targetSnippet = Scheme.GetTranslateSnippet(InStageName);
                if (targetSnippet != null)
                {
                    schemeCodes = targetSnippet.Apply(this);
                }

                // Cache and return.
                _stageResultCaches.Add(InStageName, schemeCodes);
                return schemeCodes;
            }



            public IEnumerable<ISTNodeTranslateSchemeInstance> PrerequisiteSchemeInstances
            {
                get { return _prerequisitesList; }
            }

            public void AddPrerequisiteScheme(string InKey, ISTNodeTranslateSchemeInstance InPrerequisiteSchemeInstance)
            {
                _prerequisitesTable[InKey] = InPrerequisiteSchemeInstance;
                _prerequisitesList.Add(InPrerequisiteSchemeInstance);
            }

            public ISTNodeTranslateSchemeInstance FindPrerequisite(string InKey)
            {
                if (_prerequisitesTable.TryGetValue(InKey, out var result))
                {
                    return result;
                }

                // If proxy, find in proxy's prerequisites
                if (ProxyInstance != null)
                {
                    return ProxyInstance.FindPrerequisite(InKey);
                }
                return null;
            }

            // ~ End ISTNodeTranslateSchemeInstance interfaces


            // Prerequisite scheme table and list.
            Dictionary<string, ISTNodeTranslateSchemeInstance> _prerequisitesTable = new Dictionary<string, ISTNodeTranslateSchemeInstance>();
            List<ISTNodeTranslateSchemeInstance> _prerequisitesList = new List<ISTNodeTranslateSchemeInstance>();

            // Result caches
            Dictionary<string, IReadOnlyList<string>> _stageResultCaches = new Dictionary<string, IReadOnlyList<string>>();

        }

    }


}
