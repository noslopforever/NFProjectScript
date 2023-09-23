using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace nf.protoscript.translator.expression
{

    /// <summary>
    /// Default ISTNodeTranslateScheme implementation.
    /// </summary>
    public class STNodeTranslateSchemeDefault
        : ISTNodeTranslateScheme
    {

        public class Instance
            : ISTNodeTranslateSchemeInstance
        {
            public Instance(STNodeTranslateSchemeDefault InScheme
                , ExprTranslatorAbstract InTranslator
                , IExprTranslateContext InTranslateContext
                , ISyntaxTreeNode InNodeToTranslate
                )
            {
                Scheme = InScheme;
                Translator = InTranslator;
                TranslateContext = InTranslateContext;
                NodeToTranslate = InNodeToTranslate;
            }


            // Begin ISTNodeTranslateSchemeInstance interfaces

            public ISTNodeTranslateScheme Scheme { get; }
            public ExprTranslatorAbstract Translator { get; }
            public IExprTranslateContext TranslateContext { get; }
            public ISyntaxTreeNode NodeToTranslate { get; }

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
                return null;
            }

            public void SetEnvVariable(string InVariableName, object InEnvVarValue)
            {
                _envVars[InVariableName] = InEnvVarValue;
            }

            public object FindEnvVariable(string InVariableName)
            {
                if (_envVars.TryGetValue(InVariableName, out var varValue))
                {
                    return varValue;
                }
                return null;
            }

            public string GetVarValue(string InKey, string InStageName)
            {
                // Find env-variables
                var envVar = this.FindEnvVariable(InKey);
                if (envVar != null)
                {
                    return envVar.ToString();
                }

                // Find prerequisite
                var schemeInst = this.FindPrerequisite(InKey);
                if (schemeInst != null)
                {
                    var result = schemeInst.GetResult(InStageName);
                    if (result.Count > 1)
                    {
                        // TODO log error
                        throw new InvalidOperationException();
                    }
                    return result[0];
                }

                // Find node's properties
                ISyntaxTreeNode nodeToTranslate = this.NodeToTranslate;
                try
                {
                    var propVal = nodeToTranslate.GetType().GetProperty(InKey).GetValue(nodeToTranslate).ToString();
                    return propVal;
                }
                catch (Exception ex)
                {
                    // TODO log error
                }

                return "<<INVALID_SUB_VALUE>>";
            }

            // ~ End ISTNodeTranslateSchemeInstance interfaces


            // Env-var table
            Dictionary<string, object> _envVars = new Dictionary<string, object>();

            // Prerequisite scheme table and list.
            Dictionary<string, ISTNodeTranslateSchemeInstance> _prerequisitesTable = new Dictionary<string, ISTNodeTranslateSchemeInstance>();
            List<ISTNodeTranslateSchemeInstance> _prerequisitesList = new List<ISTNodeTranslateSchemeInstance>();

            // Result caches
            public Dictionary<string, IReadOnlyList<string>> _stageResultCaches = new Dictionary<string, IReadOnlyList<string>>();

        }

        public STNodeTranslateSchemeDefault() { }


        /// <summary>
        /// Snippet for the 'Present' stage.
        /// </summary>
        public STNodeTranslateSnippet Present
        {
            get
            {
                return _snippetTable["Present"];
            }
            set
            {
                _snippetTable["Present"] = value;
            }
        }


        /// <summary>
        /// Get a snippet of a target stage.
        /// </summary>
        /// <param name="InStageName"></param>
        /// <returns></returns>
        public STNodeTranslateSnippet GetTranslateSnippet(string InStageName)
        {
            if (_snippetTable.TryGetValue(InStageName, out STNodeTranslateSnippet value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Add a snippet to the scheme.
        /// </summary>
        /// <param name="InStageName"></param>
        /// <param name="InSnippet"></param>
        public void AddSnippet(string InStageName, STNodeTranslateSnippet InSnippet)
        {
            _snippetTable[InStageName] = InSnippet;
        }

        /// <summary>
        /// Create a instance of this scheme.
        /// </summary>
        /// <param name="InTranslator"></param>
        /// <param name="InExprContext"></param>
        /// <param name="InSTNode"></param>
        /// <returns></returns>
        public ISTNodeTranslateSchemeInstance CreateInstance(ExprTranslatorAbstract InTranslator, IExprTranslateContext InExprContext, ISyntaxTreeNode InSTNode)
        {
            return new Instance(this, InTranslator, InExprContext, InSTNode);
        }


        // Snippet table.
        Dictionary<string, STNodeTranslateSnippet> _snippetTable = new Dictionary<string, STNodeTranslateSnippet>();

    }


}
