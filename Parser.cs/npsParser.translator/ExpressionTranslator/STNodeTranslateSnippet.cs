using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{

    /// <summary>
    /// TODO
    /// </summary>
    public class STNodeTranslateSnippet
    {
        public STNodeTranslateSnippet(params IElement[] InElements)
        {
            SnippetElements = InElements;
        }


        /// <summary>
        /// Snippet elements.
        /// </summary>
        public interface IElement
        {
            public string Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance);
        }

        /// <summary>
        /// Elements which construct the snippet.
        /// </summary>
        IElement[] SnippetElements { get; }

        /// <summary>
        /// GetResult the context and return translated codes.
        /// </summary>
        /// <param name="InContext"></param>
        /// <returns></returns>
        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
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

                codeLines[writingLineIndex] += snippetElem.Apply(InHolderSchemeInstance);
            }

            return codeLines;
        }


    }

}
