using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace nf.protoscript.parser.token
{
    using TokenTypeWithRegexes = KeyValuePair<ETokenType, string[]>;

    /// <summary>
    /// Token factory to create tokens.
    /// </summary>
    internal class TokenParser
    {

        /// <summary>
        /// Construct with tokens which should be checked by this parser and regexes which are used to check these tokens.
        /// </summary>
        /// <param name="InTypeByPriority"></param>
        internal TokenParser(params (ETokenType, string[])[] InTypeByPriority)
        {
            // Push the 'fallback' regex.
            var typeWithRegexes = new List<(ETokenType, string[])>();
            typeWithRegexes.AddRange(InTypeByPriority);
            typeWithRegexes.Add((ETokenType.Unknown, new string[] { CommonTokenRegexes.ALL }));

            List<TokenTypeWithRegexes> tokenTypes = new List<TokenTypeWithRegexes>();

            bool addVerticalBar = false;
            foreach (var tokenTypeWithRegexes in typeWithRegexes)
            {
                ETokenType tokenType = tokenTypeWithRegexes.Item1;
                string[] regexes = tokenTypeWithRegexes.Item2;

                for (int i = 0; i < regexes.Length; i++)
                {
                    // Add vertical bar except the first element.
                    // E0|E1|E2|...
                    if (addVerticalBar)
                    { _TokenRegexPatterns += "|"; }
                    addVerticalBar = true;

                    string regexGroupName = _GenRegexCheckName(tokenType, i);
                    string regex = regexes[i];
                    _TokenRegexPatterns += string.Format("(?<{0}>{1})"
                        , regexGroupName
                        , regex
                        );
                }
                tokenTypes.Add(new TokenTypeWithRegexes(tokenType, regexes));
            }
            _TokenTypes_SortByPriority = tokenTypes.ToArray();
        }

        /// <summary>
        /// TokenType sorted by priority.
        /// </summary>
        TokenTypeWithRegexes[] _TokenTypes_SortByPriority = new TokenTypeWithRegexes[] { };

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
            TokenTypeWithRegexes result;
            Group resultGroup = null;
            if (_CheckRegexMatch(
                    regMatch
                    , _TokenTypes_SortByPriority
                    , tokenTypeWithRegex => _GenRegexesCheckNames(tokenTypeWithRegex)
                    , out result
                    , out resultGroup
                    )
                )
            {
                ETokenType tokenType = result.Key;
                string code = resultGroup.Value;
                Token token = new Token(tokenType, code);

                int consumeCodeLen = resultGroup.Index + resultGroup.Value.Length;
                OutOtherString = InString.Substring(consumeCodeLen);

                return token;
            }

            OutOtherString = "";
            return null;
        }

        /// <summary>
        /// Generate regex-check-name which should be used as the name of MatchGroup
        /// </summary>
        string _GenRegexCheckName(ETokenType InType, int InRegexIndex)
        {
            return string.Format("N{0}_{1}", InType, InRegexIndex);
        }

        /// <summary>
        /// Generate regex-check-names which should be used as names of MatchGroups.
        /// </summary>
        string[] _GenRegexesCheckNames(TokenTypeWithRegexes InTypeWithRegexes)
        {
            ETokenType tokenType = InTypeWithRegexes.Key;
            string[] regexes = InTypeWithRegexes.Value;
            string[] result = new string[regexes.Length];
            for (int i = 0; i < regexes.Length; i++)
            {
                result[i] = _GenRegexCheckName(tokenType, i);
            }
            return result;
        }

        /// <summary>
        /// Do check regexes by priority
        /// </summary>
        bool _CheckRegexMatch<T>(Match InRegMatch, ICollection<T> InMatchObjects, Func<T, string[]> InNameGetter, out T OutMatchObject, out Group OutMatchGroup)
        {
            // Find and return the first match object.
            foreach (T checkingObj in InMatchObjects)
            {
                string[] checkingGroupNames = InNameGetter(checkingObj);
                foreach (string checkingGroupName in checkingGroupNames)
                {
                    Group checkingMatchGroup = InRegMatch.Groups[checkingGroupName];
                    if (checkingGroupName == null)
                        continue;

                    if (!checkingMatchGroup.Success)
                        continue;

                    OutMatchObject = checkingObj;
                    OutMatchGroup = checkingMatchGroup;
                    return true;
                }
            }
            OutMatchGroup = null;
            OutMatchObject = default(T);
            return false;
        }


    }


}