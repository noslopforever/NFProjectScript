using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    /// <summary>
    /// Snippets defined by code elements (such as constant strings, variable values).
    /// These elements can be assembled from snippet scripts.
    /// </summary>
    public class InfoTranslateSnippet
    {
        public InfoTranslateSnippet(params IElement[] InElements)
        {
            SnippetElements = InElements;
        }

        /// <summary>
        /// Snippet elements.
        /// </summary>
        public interface IElement
        {
            public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance);
        }

        /// <summary>
        /// All elements composing this snippet.
        /// </summary>
        IElement[] SnippetElements { get; }

        /// <summary>
        /// GetResult for the context and return translated codes.
        /// </summary>
        /// <param name="InHolderSchemeInstance"></param>
        /// <returns></returns>
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            List<string> codeLines = new List<string>();
            codeLines.Add("");
            int writingLineIndex = 0;

            for (int i = 0; i < SnippetElements.Length; i++)
            {
                var snippetElem = SnippetElements[i];
                if (snippetElem is DefaultSnippetElements.ElementNewLine)
                {
                    codeLines.Add("");
                    writingLineIndex++;
                    continue;
                }

                // Get results applied by the element.
                var elemCodeLns = snippetElem.Apply(InHolderSchemeInstance).ToArray();
                if (elemCodeLns.Length == 0)
                {
                    continue;
                }

                // The first line of the snippet will be applied to the current writing-line.
                // Other lines will be pushed to new lines.
                codeLines[writingLineIndex] += elemCodeLns[0];
                for (int elemLnIndex = 1; elemLnIndex < elemCodeLns.Length; elemLnIndex++)
                {
                    codeLines.Add($"{elemCodeLns[elemLnIndex]}");
                    writingLineIndex++;
                }
            }

            return codeLines;
        }

    }



}