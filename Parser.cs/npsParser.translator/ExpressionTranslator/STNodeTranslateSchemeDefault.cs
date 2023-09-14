﻿using nf.protoscript.syntaxtree;
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
            public Instance(ISTNodeTranslateScheme InScheme
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
                // Get and cache prerequisites.
                foreach (var prerequisite in _prerequisitesList)
                {
                    prerequisite.GetResult(InStageName);
                }

                // Get snippet of this stage, then apply it.
                var presentSnippet = Scheme.GetTranslateSnippet(InStageName);
                IReadOnlyList<string> schemeCodes = presentSnippet.Apply(this);

                // Cache and return.
                _stageResultCaches.Add(InStageName, schemeCodes);
                return schemeCodes;
            }



            public IEnumerable<ISTNodeTranslateSchemeInstance> PrerequisiteSchemeInstance
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
                return _prerequisitesTable[InKey];
            }

            // Prerequisite scheme table and list.
            Dictionary<string, ISTNodeTranslateSchemeInstance> _prerequisitesTable = new Dictionary<string, ISTNodeTranslateSchemeInstance>();
            List<ISTNodeTranslateSchemeInstance> _prerequisitesList = new List<ISTNodeTranslateSchemeInstance>();
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
            return _snippetTable[InStageName];
        }

        /// <summary>
        /// Add a snippet to the scheme.
        /// </summary>
        /// <param name="InStageName"></param>
        /// <param name="InSnippet"></param>
        public void AddSnippet(string InStageName,  STNodeTranslateSnippet InSnippet)
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
