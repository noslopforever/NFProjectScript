using nf.protoscript.translator.expression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    /// <summary>
    /// Snippets established by code Elements (like constant-string, variable-value).
    /// Elements may be constructed from snippet-scripts.
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
        /// Elements which construct the snippet.
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

                // The first line of the snippet will be applied to the current writing-line.
                // Other lines will be pushed to new lines.
                var elemCodeLns = snippetElem.Apply(InHolderSchemeInstance).ToArray();
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