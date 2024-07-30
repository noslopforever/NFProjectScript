using nf.protoscript.translator.DefaultScheme.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace nf.protoscript.translator.DefaultScheme
{

    /// <summary>
    /// Parser to parse InfoTranslateSchemeDefault.IElements.
    /// </summary>
    public static class ElementParser
    {

        /// <summary>
        /// Regex pattern to recognize '${...}'
        /// </summary>
        private const string Pattern = @"\$\{[^{}]*\}";

        /// <summary>
        /// Parses elements from the given code.
        /// </summary>
        /// <param name="InCode">The input code containing elements to parse.</param>
        /// <returns>An enumerable collection of parsed elements.</returns>
        /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
        public static InfoTranslateSchemeDefault.IElement[] ParseElements(string InCode)
        {
            var resultElements = new List<InfoTranslateSchemeDefault.IElement>();

            // Define the pattern to find all ${ ... } parts in the input code.
            // Assuming Pattern is a valid regular expression pattern.
            var matches = Regex.Matches(InCode, Pattern);

            int lastIndex = 0;
            foreach (Match match in matches)
            {
                // Handle the text outside the ${ ... } blocks.
                if (lastIndex < match.Index)
                {
                    string outsideCode = InCode.Substring(lastIndex, match.Index - lastIndex);
                    resultElements.Add(new ElementConstString(outsideCode));
                }

                // Handle the code inside the ${ ... } block.
                string insideCode = match.Value;
                var codeWithoutContainer = insideCode.Substring(2, insideCode.Length - 3); // Remove the ${ and } characters.
                resultElements.Add(new ElementExpr(codeWithoutContainer));

                // Update the lastIndex to the end of the current match.
                lastIndex = match.Index + match.Length;
            }

            // Handle any trailing text after the last ${ ... } block.
            if (lastIndex < InCode.Length)
            {
                string remainingText = InCode.Substring(lastIndex);
                resultElements.Add(new ElementConstString(remainingText));
            }

            return resultElements.ToArray();
        }


    }

}