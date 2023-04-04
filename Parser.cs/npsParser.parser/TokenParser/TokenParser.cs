using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace nf.protoscript.parser.token
{
    using TokenTypeWithRegex = KeyValuePair<ETokenType, string>;

    /// <summary>
    /// Token factory to create tokens.
    /// </summary>
    internal class TokenParser
    {
        const string RegexAll = ".";

        /// <summary>
        /// Construct with tokens which should be checked by this parser and regexes which are used to check these tokens.
        /// </summary>
        /// <param name="InTypeByPriority"></param>
        internal TokenParser(params (ETokenType, string)[] InTypeByPriority)
        {
            // Push the 'fallback' regex.
            var typeWithRegexList = new List<(ETokenType, string)>();
            typeWithRegexList.AddRange(InTypeByPriority);
            typeWithRegexList.Add((ETokenType.Unknown, RegexAll));

            List<TokenTypeWithRegex> tokenTypes = new List<TokenTypeWithRegex>();

            bool addVerticalBar = false;
            for (int i = 0; i < typeWithRegexList.Count; i++)
            {
                var tokenTypeWithRegex = typeWithRegexList[i];
                ETokenType tokenType = tokenTypeWithRegex.Item1;
                string regex = tokenTypeWithRegex.Item2;

                // Add vertical bar except the first element.
                // E0|E1|E2|...
                if (addVerticalBar)
                { _TokenRegexPatterns += "|"; }
                addVerticalBar = true;

                // register group name: (?<r0>{regex})|(?<r1>{regex})|...
                string regexGroupName = _GenRegexGroupNameForIndex(i);
                _TokenRegexPatterns += string.Format("(?<{0}>{1})"
                    , regexGroupName
                    , regex
                    );
                tokenTypes.Add(new TokenTypeWithRegex(tokenType, regex));
            }
            _TokenTypes_SortByPriority = tokenTypes.ToArray();
        }

        private static string _GenRegexGroupNameForIndex(int i)
        {
            return $"r{i}";
        }

        /// <summary>
        /// TokenType sorted by priority.
        /// </summary>
        TokenTypeWithRegex[] _TokenTypes_SortByPriority = new TokenTypeWithRegex[] { };

        /// <summary>
        /// Merged regex pattern.
        /// </summary>
        string _TokenRegexPatterns = "";

        /// <summary>
        /// Parse a line.
        /// </summary>
        public void ParseLine(string InLineCodes, ref List<Token> RefTokens)
        {
            string Codes = InLineCodes;
            while (true)
            {
                Token t = ParseToken(Codes, out Codes);
                if (t == null)
                { break; }

                // Skip 'skip' tokens, do not save it.
                if (true)
                {
                    if (t.TokenType == ETokenType.Skip)
                    { continue; }
                }

                RefTokens.Add(t);
            }
        }


        /// <summary>
        /// Try parse strings into tokens.
        /// </summary>
        /// <param name="InLineIndex"></param>
        /// <param name="InString"></param>
        /// <param name="OutOtherString"></param>
        /// <returns></returns>
        public Token ParseToken(string InString, out string OutOtherString)
        {
            // do match token patterns.
            var regMatch = Regex.Match(InString, _TokenRegexPatterns);

            // check which token has been matched.
            ETokenType result = ETokenType.Unknown;
            Group resultGroup = null;
            if (_CheckRegexMatch(
                    regMatch
                    , out result
                    , out resultGroup
                    )
                )
            {
                ETokenType tokenType = result;
                string code = resultGroup.Value;
                Token token = new Token(tokenType, code, InString.Length);

                // Get strings after this token.
                int consumeCodeLen = resultGroup.Index + resultGroup.Value.Length;
                OutOtherString = InString.Substring(consumeCodeLen);

                return token;
            }

            OutOtherString = "";
            return null;
        }

        /// <summary>
        /// Do check regexes by priority
        /// </summary>
        bool _CheckRegexMatch(Match InRegMatch
            , out ETokenType OutMatchToken
            , out Group OutMatchGroup
            )
        {
            // Find and return the first match object.
            for (int i = 0; i < _TokenTypes_SortByPriority.Length; ++i)
            {
                var checkingObj = _TokenTypes_SortByPriority[i];
                string checkingGroupName = _GenRegexGroupNameForIndex(i);

                Group checkingMatchGroup = InRegMatch.Groups[checkingGroupName];
                if (checkingGroupName == null)
                    continue;

                if (!checkingMatchGroup.Success)
                    continue;

                OutMatchToken = checkingObj.Key;
                OutMatchGroup = checkingMatchGroup;
                return true;
            }
            OutMatchGroup = null;
            OutMatchToken = ETokenType.Unknown;
            return false;
        }


    }


}