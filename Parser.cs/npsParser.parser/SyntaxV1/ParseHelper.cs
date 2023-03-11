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

        public static bool CheckRegexMatch(Match InRegMatch, ICollection<object> InMatchObjects, Func<object, string> InNameGetter, out object OutMatchObject, out Group OutMatchGroup)
        {
            foreach (object checkingObj in InMatchObjects)
            {
                string checkingGroupName = InNameGetter(checkingObj);
                Group checkingMatchGroup = InRegMatch.Groups[checkingGroupName];
                if (checkingGroupName == null)
                    continue;

                if (!checkingMatchGroup.Success)
                    continue;

                OutMatchObject = checkingObj;
                OutMatchGroup = checkingMatchGroup;
                return true;
            }
            OutMatchGroup = null;
            OutMatchObject = null;
            return false;
        }


    }


}
