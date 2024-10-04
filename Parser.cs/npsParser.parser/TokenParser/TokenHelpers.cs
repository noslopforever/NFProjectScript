using System;
using System.Collections.Generic;
using System.IO;

namespace nf.protoscript.parser
{
    /// <summary>
    /// Helper functions for IToken.
    /// </summary>
    public static class TokenHelpers
    {

        /// <summary>
        /// Dumps the list of tokens to the specified text writer in a formatted manner.
        /// </summary>
        /// <param name="InTextWriter">The text writer to which the tokens will be written.</param>
        /// <param name="InTokens">The list of tokens to dump.</param>
        public static void DumpTokens(TextWriter InTextWriter, IReadOnlyList<IToken> InTokens)
        {
            string tokenTypes = "";
            string tokenCodes = "";
            foreach (var token in InTokens)
            {
                string tokenType = token.TokenType;
                string tokenCode = token.Code;

                int maxLen = Math.Max(tokenType.Length, tokenCode.Length);
                int secLen = maxLen + 2;

                string filtedCode = _PadStringToLength(tokenCode, ' ', secLen);
                string filtedType = _PadStringToLength(tokenType, '^', secLen);

                tokenTypes += filtedType + "|";
                tokenCodes += filtedCode + "|";
            }

            InTextWriter.WriteLine(tokenCodes);
            InTextWriter.WriteLine(tokenTypes);
        }

        /// <summary>
        /// Pads the original string to the target length by adding padding characters.
        /// </summary>
        /// <param name="InOrigin">The original string to pad.</param>
        /// <param name="InPaddingChar">The character to use for padding.</param>
        /// <param name="InTargetLength">The target length of the padded string.</param>
        /// <returns>The padded string.</returns>
        static string _PadStringToLength(string InOrigin, char InPaddingChar, int InTargetLength)
        {
            int spaceLen = InTargetLength - InOrigin.Length;
            if (spaceLen <= 0)
            {
                return InOrigin;
            }

            int halfSpaceLen = spaceLen / 2;
            string result = _CreatePaddingString(InPaddingChar, halfSpaceLen);
            result += InOrigin;
            int lineEndSpaceLen = InTargetLength - result.Length;
            result += _CreatePaddingString(InPaddingChar, lineEndSpaceLen);
            return result;
        }

        /// <summary>
        /// Creates a string with the specified number of repeated characters.
        /// </summary>
        /// <param name="InCharToRepeat">The character to repeat.</param>
        /// <param name="InRepeatCount">The number of times to repeat the character.</param>
        /// <returns>A string containing the repeated character.</returns>
        static string _CreatePaddingString(char InCharToRepeat, int InRepeatCount)
        {
            string result = "";
            for (int i = 0; i < InRepeatCount; i++)
            {
                result += InCharToRepeat;
            }
            return result;
        }

    }


}