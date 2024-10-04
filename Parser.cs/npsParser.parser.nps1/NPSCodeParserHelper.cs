using System;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// A helper class that provides utility methods for parsing NPS code.
    /// This class contains static methods and properties to handle common tasks such as trimming, replacing tabs, and checking start codes.
    /// </summary>
    internal class NPSCodeParserHelper
    {
        /// <summary>
        /// Gets the characters that are considered space characters.
        /// </summary>
        public static char[] GSpaceCharacters
        {
            get { return new char[] { ' ', '\t' }; }
        }

        /// <summary>
        /// Gets the characters that are considered end-of-line (EOL) characters.
        /// </summary>
        public static char[] GEOLCharacters
        {
            get { return new char[] { '\r', '\n' }; }
        }

        /// <summary>
        /// Gets the number of spaces that a tab character represents.
        /// </summary>
        public static int GTabSpaceNum
        {
            get { return 4; }
        }

        /// <summary>
        /// Gets the number of spaces that a tab character represents, clamped between 2 and 16.
        /// </summary>
        /// <returns>The number of spaces per tab.</returns>
        public static int GetTabSpaceNum()
        {
            return Math.Min(Math.Max(GTabSpaceNum, 2), 16);
        }

        /// <summary>
        /// Determines whether the specified character is a space character.
        /// </summary>
        /// <param name="InChar">The character to check.</param>
        /// <returns>True if the character is a space character; otherwise, false.</returns>
        public static bool IsSpace(char InChar)
        {
            return -1 != Array.IndexOf(GSpaceCharacters, InChar);
        }

        /// <summary>
        /// Replaces all leading tab characters in the code with the appropriate number of spaces.
        /// </summary>
        /// <param name="InCode">The input code to process.</param>
        /// <param name="OutStartSpaceNum">The number of leading spaces after replacement.</param>
        /// <returns>The modified code with leading tabs replaced by spaces.</returns>
        public static string ReplaceStartTabs(string InCode, out int OutStartSpaceNum)
        {
            string code = InCode;
            int tabSpaceNum = GetTabSpaceNum();
            int codeIndex = 0;
            for (; codeIndex < code.Length; ++codeIndex)
            {
                if (code[codeIndex] == ' ')
                {
                    continue;
                }
                if (code[codeIndex] == '\t')
                {
                    // Ensure the next character's column-index is a mutiple of tabSpaceNum (default: 4).
                    // |\t A B C D| -> |x + + + A B C D|    -> Remove \t, then insert 4 new spaces.
                    // |A \t B C D| -> |A x + + B C D|      -> Remove \t, then insert 3 new spaces.
                    // |A B \t C D| -> |A B x + C D|        -> Remove \t, then insert 2 new space.
                    // |A B C \t D| -> |A B C x D|          -> Remove \t, then insert 1 new space.
                    int insertSpaceNum = tabSpaceNum - (codeIndex % tabSpaceNum);
                    string insertSpaceString = new string(' ', insertSpaceNum);
                    code = code.Remove(codeIndex, 1).Insert(codeIndex, insertSpaceString);
                }
                else
                {
                    // Break when we meet the first NON SPACE character.
                    break;
                }

            } // ~ End for

            OutStartSpaceNum = codeIndex;
            return code;
        }

        /// <summary>
        /// Removes end-of-line (EOL) characters from the code.
        /// </summary>
        /// <param name="InCode">The input code to process.</param>
        /// <param name="OutCode">The output code without EOL characters.</param>
        public static void RemoveEOL(string InCode, out string OutCode)
        {
            OutCode = InCode.TrimEnd(GEOLCharacters);
        }

        /// <summary>
        /// Trims leading and trailing whitespace from the code and outputs the number of leading spaces.
        /// </summary>
        /// <param name="InCode">The input code to process.</param>
        /// <param name="OutStartSpaceNum">The number of leading spaces before trimming.</param>
        /// <returns>The trimmed code.</returns>
        public static string Trim(string InCode, out int OutStartSpaceNum)
        {
            string trimStart = InCode.TrimStart();
            OutStartSpaceNum = InCode.Length - trimStart.Length;
            return trimStart.TrimEnd();
        }

        /// <summary>
        /// Parses the indent (leading spaces) of the code and returns the remaining code.
        /// </summary>
        /// <param name="InCode">The input code to process.</param>
        /// <param name="OutRemainCode">The remaining code after removing the indent.</param>
        /// <returns>The number of leading spaces (indent).</returns>
        public static int ParseIndent(string InCode, out string OutRemainCode)
        {
            int indent = 0;
            ReplaceStartTabs(InCode, out indent);
            OutRemainCode = InCode.Substring(indent);
            return indent;
        }

        /// <summary>
        /// Trims the code, removes leading and trailing spaces, and outputs the indent (number of leading spaces).
        /// </summary>
        /// <param name="InCode">The input code to process.</param>
        /// <param name="OutIndent">The number of leading spaces (indent).</param>
        /// <returns>The trimmed code.</returns>
        public static string TrimCodes(string InCode, out int OutIndent)
        {
            int indent = 0;
            ReplaceStartTabs(InCode, out indent);

            OutIndent = indent;
            return InCode.Trim();
        }

        /// <summary>
        /// Checks if the code starts with the specified start code. If it does, removes the start code and returns the remaining code.
        /// </summary>
        /// <param name="InCode">The input code to process.</param>
        /// <param name="InStartCode">The start code to check for.</param>
        /// <param name="OutOtherCode">The remaining code after removing the start code.</param>
        /// <returns>True if the code starts with the specified start code; otherwise, false.</returns>
        public static bool CheckAndRemoveStartCode(string InCode, string InStartCode, out string OutOtherCode)
        {
            if (InCode.StartsWith(InStartCode))
            {
                OutOtherCode = InCode.Substring(InStartCode.Length);
                return true;
            }
            OutOtherCode = InCode;
            return false;
        }

        /// <summary>
        /// Checks if the code starts with any of the specified start codes. If it does, removes the start code and returns the remaining code.
        /// </summary>
        /// <param name="InCode">The input code to process.</param>
        /// <param name="OutOtherCode">The remaining code after removing the start code.</param>
        /// <param name="InStartCodes">The start codes to check for.</param>
        /// <returns>True if the code starts with any of the specified start codes; otherwise, false.</returns>
        public static bool CheckAndRemoveStartCodes(string InCode, out string OutOtherCode, params string[] InStartCodes)
        {
            foreach (string sc in InStartCodes)
            {
                if (CheckAndRemoveStartCode(InCode, sc, out OutOtherCode))
                {
                    return true;
                }
            }
            OutOtherCode = InCode;
            return false;
        }

        ///// <summary>
        ///// Try parse line end blocks, always be line-end attributes and comments.
        ///// </summary>
        ///// <param name="InTokenList"></param>
        ///// <param name="OutAttrs"></param>
        ///// <param name="OutComment"></param>
        //internal static void TryParseLineEndBlocks(TokenList InTokenList, out STNode_AttributeDefs OutAttrs)
        //{
        //    // Try parse line-end attributes.
        //    ASTParser_BlockLineEndAttributes leAttrsParser = new ASTParser_BlockLineEndAttributes();
        //    OutAttrs = leAttrsParser.Parse(InTokenList);
        //}

        ///// <summary>
        ///// Try parse line end blocks and call lambda.
        ///// </summary>
        ///// <param name="InTokenList"></param>
        ///// <param name="InAct"></param>
        //internal static void TryParseLineEndBlocks(TokenList InTokenList, Action<STNode_AttributeDefs> InAct)
        //{
        //    STNode_AttributeDefs attrs = null;
        //    TryParseLineEndBlocks(InTokenList, out attrs);
        //    InAct(attrs);
        //}

        ///// <summary>
        ///// Try parse line end blocks and call lambda (return version).
        ///// </summary>
        ///// <param name="InTokenList"></param>
        ///// <param name="InAct"></param>
        //internal static T TryParseLineEndBlocks<T>(TokenList InTokenList, Func<STNode_AttributeDefs, T> InFunc)
        //{
        //    STNode_AttributeDefs attrs = null;
        //    TryParseLineEndBlocks(InTokenList, out attrs);
        //    return InFunc(attrs);
        //}

        ///// <summary>
        ///// Check if the InTokenList has been finished. If not, throw ParserException.
        ///// </summary>
        ///// <param name="InTokenList"></param>
        ///// <param name="InCodeLine"></param>
        //public static void CheckFinishedAndThrow(TokenList InTokenList, CodeLine InCodeLine)
        //{
        //    if (!InTokenList.IsEnd)
        //    {
        //        throw new ParserException(ParserErrorType.Factory_UnexpectedToken
        //            , InTokenList.CurrentToken
        //            );
        //    }
        //}

    }

}