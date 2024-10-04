using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace nf.protoscript.parser
{


    /// <summary>
    /// A token parser that uses regular expressions to identify tokens.
    /// </summary>
    public class TokenParserRegex
        : ITokenParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenParserRegex"/> class.
        /// </summary>
        /// <param name="InRegexPattern">The regular expression pattern to match tokens.</param>
        /// <param name="InTokenType">The type of token to return when a match is found.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="InRegexPattern"/> or <paramref name="InTokenType"/> is null or empty.</exception>
        public TokenParserRegex(string InRegexPattern, string InTokenType)
        {
            if (string.IsNullOrEmpty(InRegexPattern))
            {
                throw new ArgumentNullException(nameof(InRegexPattern), "The regular expression pattern cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(InTokenType))
            {
                throw new ArgumentNullException(nameof(InTokenType), "The token type cannot be null or empty.");
            }

            // Compile the regular expression for better performance.
            _regex = new Regex(InRegexPattern, RegexOptions.Compiled);
            _tokenType = InTokenType;
        }

        // Begin ITokenParser implementation
        /// <inheritdoc />
        public IToken ParseCodeToToken(string InCode, int InStartIndex)
        {
            if (InCode == null)
            {
                throw new ArgumentException("Input code cannot be null.", nameof(InCode));
            }

            // Attempt to match the input code starting at the given index.
            var match = _regex.Match(InCode, InStartIndex);
            if (!match.Success || match.Index != InStartIndex)
            {
                return null;
            }

            // Create a new token with the matched value, token type, and debug information.
            var result = new Token(_tokenType, match.Value, $"{match.Index}:{match.Index + match.Length}");
            return result;
        }
        // End ITokenParser implementation

        // Private fields to store the compiled regular expression and the token type.
        private readonly Regex _regex;
        private readonly string _tokenType;

    }


}