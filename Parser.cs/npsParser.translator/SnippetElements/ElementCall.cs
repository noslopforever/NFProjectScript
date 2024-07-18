using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// Call another scheme for the current context.
    /// </summary>
    public class ElementCall
        : InfoTranslateSnippet.IElement
    {
        public ElementCall(string InSchemeName, params (string, InfoTranslateSnippet.IElement)[] InParams)
        {
            SchemeName = InSchemeName;
            foreach (var item in InParams)
            {
                _params.Add(item.Item1, new InfoTranslateSnippet(item.Item2));
            }
        }

        /// <summary>
        /// The scheme to call
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// Pass in parameters.
        /// </summary>
        public IReadOnlyDictionary<string, InfoTranslateSnippet> Params { get { return _params; } }
        Dictionary<string, InfoTranslateSnippet> _params = new Dictionary<string, InfoTranslateSnippet>();

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            var scheme = translator.FindBestScheme(context, SchemeName);
            var si = scheme.CreateInstance(translator, context);

            // collect parameter scheme instances from parameter snippets.
            foreach (var item in _params)
            {
                InfoTranslateSnippet snippet = item.Value;
                var paramScheme = new InfoTranslateSchemeDefault(snippet);
                var paramSI = paramScheme.CreateInstance(translator, context);
                si.AddParam(item.Key, paramSI);
            }

            return si.GetResult();
        }

    }

}