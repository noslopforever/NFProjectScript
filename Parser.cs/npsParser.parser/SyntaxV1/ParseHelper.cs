using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace nf.protoscript.parser.syntax1
{
    public class ParseHelper
    {
        public static char[] GSpaceCharacters
        {
            get { return new char[] { ' ', '\t' }; }
        }
        public static char[] GEOLCharacters
        {
            get { return new char[] { '\r', '\n' }; }
        }
        public static int GTabSpaceNum
        {
            get { return 4; }
        }

        public static int GetTabSpaceNum()
        {
            return Math.Min(Math.Max(GTabSpaceNum, 2), 16);
        }

        public static bool IsSpace(char InChar)
        {
            return -1 != Array.IndexOf(GSpaceCharacters, InChar);
        }

        public static string ReplaceStartTabs(string InCode, out int OutStartSpaceNum)
        {
            string coe = InCode;
            int tabSpaceNum = GetTabSpaceNum();
            int codeIndex = 0;
            for (; codeIndex < coe.Length; ++codeIndex)
            {
                if (coe[codeIndex] == ' ') { continue; }
                if (coe[codeIndex] == '\t')
                {
                    // Ensure the next character's column-index is a mutiple of tabSpaceNum (default: 4).
                    // |\t A B C D| -> |x + + + A B C D|    -> Remove \t, then insert 4 new spaces.
                    // |A \t B C D| -> |A x + + B C D|      -> Remove \t, then insert 3 new spaces.
                    // |A B \t C D| -> |A B x + C D|        -> Remove \t, then insert 2 new space.
                    // |A B C \t D| -> |A B C x D|          -> Remove \t, then insert 1 new space.
                    int insertSpaceNum = tabSpaceNum - (codeIndex % tabSpaceNum);
                    string insertSpaceString = "";
                    for (int i = 0; i < insertSpaceNum; ++i)
                    {
                        insertSpaceString += ' ';
                    }
                    coe = coe.Remove(codeIndex, 1).Insert(codeIndex, insertSpaceString);
                }
                // till the first NON-SPACE character.
                else
                { break; }

            } // ~ while next Tab

            OutStartSpaceNum = codeIndex;
            return coe;
        }

        /// <summary>
        /// Remove EOL from the code.
        /// </summary>
        /// <param name="InCode"></param>
        /// <param name="OutCode"></param>
        public static void RemoveEOL(string InCode, out string OutCode)
        {
            OutCode = InCode.TrimEnd(GEOLCharacters);
        }

        /// <summary>
        /// Trim
        /// </summary>
        /// <param name="InCode"></param>
        /// <param name="OutStartSpaceNum"></param>
        /// <returns></returns>
        public static string Trim(string InCode, out int OutStartSpaceNum)
        {
            string trimStart = InCode.TrimStart();
            OutStartSpaceNum = InCode.Length - trimStart.Length;
            return trimStart.TrimEnd();
        }

        /// <summary>
        /// Parse indent (start spaces) of a string.
        /// </summary>
        public static int ParseIndent(string InCode, out string OutRemainCode)
        {
            int indent = 0;
            ReplaceStartTabs(InCode, out indent);
            OutRemainCode = InCode.Substring(indent);
            return indent;
        }

        /// <summary>
        /// Trim codes, remove start/end spaces, and out StartSpaceNum
        /// </summary>
        public static string TrimCodes(string InCode, out int OutIndent)
        {
            int indent = 0;
            ReplaceStartTabs(InCode, out indent);

            OutIndent = indent;
            return InCode.Trim();
        }

        /// <summary>
        /// Check if InCode starts with InStartCodes. If yes, remove the StartCode and return the other codes by OutOtherCode.
        /// </summary>
        /// <param name="InCode"></param>
        /// <param name="InStartCodes"></param>
        /// <param name="OutOtherCode"></param>
        /// <returns></returns>
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
        /// Check if InCode starts with InStartCodes. If yes, remove the StartCode and return the other codes by OutOtherCode.
        /// </summary>
        /// <param name="InCode"></param>
        /// <param name="InStartCodes"></param>
        /// <param name="OutOtherCode"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Try parse line end blocks, always be line-end attributes and comments.
        /// </summary>
        /// <param name="InTokenList"></param>
        /// <param name="OutAttrs"></param>
        /// <param name="OutComment"></param>
        internal static void TryParseLineEndBlocks(TokenList InTokenList, out STNode_AttributeDefs OutAttrs, out STNode_Comment OutComment)
        {
            // Try parse line-end attributes.
            ASTParser_BlockLineEndAttributes leAttrsParser = new ASTParser_BlockLineEndAttributes();
            OutAttrs = leAttrsParser.Parse(InTokenList);

            // Try parse line-end comments.
            ASTParser_BlockLineEndComments leCommentParser = new ASTParser_BlockLineEndComments();
            OutComment = leCommentParser.Parse(InTokenList);
        }

        /// <summary>
        /// Try parse line end blocks and call lambda.
        /// </summary>
        /// <param name="InTokenList"></param>
        /// <param name="InAct"></param>
        internal static void TryParseLineEndBlocks(TokenList InTokenList, Action<STNode_AttributeDefs, STNode_Comment> InAct)
        {
            STNode_AttributeDefs attrs = null;
            STNode_Comment comment = null;
            TryParseLineEndBlocks(InTokenList, out attrs, out comment);
            InAct(attrs, comment);
        }

        /// <summary>
        /// Try parse line end blocks and call lambda (return version).
        /// </summary>
        /// <param name="InTokenList"></param>
        /// <param name="InAct"></param>
        internal static T TryParseLineEndBlocks<T>(TokenList InTokenList, Func<STNode_AttributeDefs, STNode_Comment, T> InFunc)
        {
            STNode_AttributeDefs attrs = null;
            STNode_Comment comment = null;
            TryParseLineEndBlocks(InTokenList, out attrs, out comment);
            return InFunc(attrs, comment);
        }

        /// <summary>
        /// Check if the InTokenList has been finished. If not, throw ParserException.
        /// </summary>
        /// <param name="InTokenList"></param>
        /// <param name="InCodeLine"></param>
        public static void CheckFinishedAndThrow(TokenList InTokenList, CodeLine InCodeLine)
        {
            if (!InTokenList.IsEnd)
            {
                throw new ParserException(ParserErrorType.Factory_UnexpectedToken
                    , InCodeLine
                    , InTokenList.CurrentToken
                    );
            }
        }

    }


}
