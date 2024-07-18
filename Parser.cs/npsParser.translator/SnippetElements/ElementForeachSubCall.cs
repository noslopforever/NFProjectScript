using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// For each sub info and call another scheme for it.
    /// </summary>
    public class ElementForeachSubCall
        : InfoTranslateSnippet.IElement
    {
        public ElementForeachSubCall(string InSchemeName, string InHeaderFilter, string InSeperator = "")
        {
            SchemeName = InSchemeName;
            HeaderFilter = InHeaderFilter;
            Seperator = InSeperator;
        }

        /// <summary>
        /// The scheme to call for each sub Info
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// Which infos should be selected
        /// </summary>
        public string HeaderFilter { get; }

        /// <summary>
        /// Seperator between sub items.
        /// </summary>
        public string Seperator { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;
            List<string> result = new List<string>();
            result.Add("");
            if (context is ITranslatingInfoContext)
            {
                (context as ITranslatingInfoContext)?.TranslatingInfo.ForeachSubInfo<Info>(
                    info =>
                    {
                        TranslatingInfoContext infoCtx = new TranslatingInfoContext(context, info);
                        var scheme = translator.FindBestScheme(infoCtx, SchemeName);
                        var si = scheme.CreateInstance(translator, infoCtx);
                        var subResult = si.GetResult();
                        if (subResult.Count > 0)
                        {
                            result[^1] += subResult[0];
                        }
                        for (int i = 1; i < subResult.Count; i++)
                        {
                            result.Add(subResult[i]);
                        }
                        result[^1] += Seperator;
                    }
                    , info => info.Header.Contains(HeaderFilter)
                    );
                // Remove the last seperator
                result[^1] = result[^1].Remove(result[^1].Length - Seperator.Length);
            }
            else if (context is ITranslatingExprContext)
            {
                (context as ITranslatingExprContext).TranslatingExprNode.ForeachSubNodes(
                    (key, stNode) =>
                    {
                        if (key.StartsWith(HeaderFilter))
                        {
                            TranslatingExprContext exprCtx = new TranslatingExprContext(context, stNode);
                            var scheme = translator.FindBestScheme(exprCtx, SchemeName);
                            var si = scheme.CreateInstance(translator, exprCtx);
                            var subResult = si.GetResult();
                            if (subResult.Count > 0)
                            {
                                result[^1] += subResult[0];
                            }
                            for (int i = 1; i < subResult.Count; i++)
                            {
                                result.Add(subResult[i]);
                            }
                            result[^1] += Seperator;
                        }
                        return true;
                    }
                    );
                // Remove the last seperator
                result[^1] = result[^1].Remove(result[^1].Length - Seperator.Length);
            }
            return result;
        }

    }

}