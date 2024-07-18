using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// Change current context and call sub-snippets with the new Context.
    /// TODO replace with scope change.
    /// </summary>
    public class ElementChangeContext
        : ElementWithSubSnippets
        , InfoTranslateSnippet.IElement
    {
        public ElementChangeContext(string InNewContextName, params InfoTranslateSnippet.IElement[] InSubElements)
            : base(InSubElements)
        {
            NewContextName = InNewContextName;
        }

        /// <summary>
        /// The new context 
        /// </summary>
        public string NewContextName { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;

            IEnumerable<ITranslatingContext> targetContexts = null;
            if (NewContextName != null && NewContextName != "")
            {
                // TODO Unify constructions of all contexts into a unique factory.

                // Get the caller object by Name.
                if (InHolderSchemeInstance.Context.TryGetContextValue(NewContextName, out var val))
                {
                    // construct the context for the caller-object
                    if (val is Info)
                    {
                        var newCtx = new TranslatingInfoContext(InHolderSchemeInstance.Context, val as Info);
                        targetContexts = new ITranslatingContext[] { newCtx };
                    }
                    else if (val is ISyntaxTreeNode)
                    {
                        var newCtx = new TranslatingExprContext(InHolderSchemeInstance.Context, val as ISyntaxTreeNode);
                        targetContexts = new ITranslatingContext[] { newCtx };
                    }
                }
            }

            if (targetContexts != null)
            {
                List<string> results = new List<string>();
                foreach (var targetContext in targetContexts)
                {
                    var si = SubScheme.CreateInstance(translator, targetContext);
                    results.AddRange(si.GetResult());
                }
                return results;
            }

            // TODO log error.
            throw new InvalidCastException();
            return new string[0];
        }

    }

}