using nf.protoscript.syntaxtree;
using nf.protoscript.translator.expression.DefaultSnippetElements;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace nf.protoscript.translator.expression
{

    /// <summary>
    /// Default ISTNodeTranslateScheme implementation.
    /// </summary>
    public partial class STNodeTranslateSchemeDefault
        : ISTNodeTranslateScheme
    {
        public STNodeTranslateSchemeDefault() { }

        public STNodeTranslateSchemeDefault(STNodeTranslateSnippet InPresent)
        {
            Present = InPresent;
        }

        public STNodeTranslateSchemeDefault(Dictionary<string, STNodeTranslateSnippet> InSnippetTable)
        {
            _snippetTable = InSnippetTable;
        }

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

        // Begin ISTNodeTranslateScheme interfaces

        public ISTNodeTranslateSchemeInstance CreateInstance(ExprTranslatorAbstract InTranslator, ExprTranslatorAbstract.ITranslatingContext InContext)
        {
            return new Instance(this, InTranslator, InContext);
        }

        public ISTNodeTranslateSchemeInstance CreateProxyInstance(ISTNodeTranslateSchemeInstance InSchemeInstanceToBeProxied)
        {
            return Instance.CreateProxyInstance(this, InSchemeInstanceToBeProxied);
        }

        // ~ End ISTNodeTranslateScheme interfaces

        // Snippet table.
        Dictionary<string, STNodeTranslateSnippet> _snippetTable = new Dictionary<string, STNodeTranslateSnippet>();

    }


}
