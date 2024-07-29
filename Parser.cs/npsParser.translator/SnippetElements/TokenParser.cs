using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Represents a token in the parsing process.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Gets the type of the token, such as Identity, Number, FloatingNumber, Operator, etc.
        /// </summary>
        string TokenType { get; }

        /// <summary>
        /// Gets the string representation of the token.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Gets debugging information including the filename, line, and column of the token.
        /// </summary>
        string DebugOutput { get; }
    }

    /// <summary>
    /// Default implementation of a token.
    /// </summary>
    public class Token : IToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="InType">The type of the token.</param>
        /// <param name="InCode">The string representation of the token.</param>
        /// <param name="InDebugText">Optional debugging information about the token.</param>
        public Token(string InType, string InCode, string InDebugText = "")
        {
            TokenType = InType;
            Code = InCode;
            DebugOutput = InDebugText;
        }

        // Begin IToken implementation
        public string TokenType { get; }
        public string Code { get; }
        public string DebugOutput { get; }
        // End IToken implementation
    }

    /// <summary>
    /// Interface for parsing a token from a string.
    /// </summary>
    public interface ITokenParser
    {
        /// <summary>
        /// Parses a token from the specified code starting at the given index.
        /// </summary>
        /// <param name="InCode">The input code to parse.</param>
        /// <param name="InStartIndex">The starting index in the input code.</param>
        /// <returns>The parsed token or null if no token is found.</returns>
        IToken Parse(string InCode, int InStartIndex);
    }

    /// <summary>
    /// Token parser that uses regular expressions to identify tokens.
    /// </summary>
    public class TokenParserRegex : ITokenParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenParserRegex"/> class.
        /// </summary>
        /// <param name="InRegexPattern">The regular expression pattern to match tokens.</param>
        /// <param name="InTokenType">The type of token to return when a match is found.</param>
        public TokenParserRegex(string InRegexPattern, string InTokenType)
        {
            _regex = new Regex(InRegexPattern, RegexOptions.Compiled);
            _tokenType = InTokenType;
        }

        // Begin ITokenParser implementation
        public IToken Parse(string InCode, int InStartIndex)
        {
            var match = _regex.Match(InCode, InStartIndex);
            if (!match.Success || match.Index != InStartIndex)
            {
                return null;
            }

            var result = new Token(_tokenType, match.Value, $"{match.Index}:{match.Index + match.Length}");
            return result;
        }
        // End ITokenParser implementation

        private readonly Regex _regex;
        private readonly string _tokenType;

    }

    //public class TokenParserStartEndChar
    //    : ITokenParser
    //{
    //    public TokenParserStartEndChar(char[] InStartChar, char[] InEndChar)
    //    {
    //    }
    //    public TokenParserStartEndChar(Func<char, bool> InStartCharChecker, Func<char, bool> InEndCharChecker)
    //    {
    //    }
    //    public TokenParserStartEndChar(Func<char, bool> InEndCharChecker, char[] InEndChar)
    //    {
    //    }
    //    public TokenParserStartEndChar(char[] InStartChar, Func<char, bool> InEndCharChecker)
    //    {
    //    }
    //    Func<char, bool> StartCharChecker { get; }
    //    Func<char, bool> EndCharChecker { get; }
    //    public IToken Parse(string InCode, int InStartIndex, int InEndIndex)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    /// <summary>
    /// A tokenizer that parses a string into a list of tokens using a collection of parsers.
    /// </summary>
    public class Tokenizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="InParsers">An array of parsers to use for tokenization.</param>
        public Tokenizer(params ITokenParser[] InParsers)
        {
            Parsers = InParsers;
        }

        /// <summary>
        /// Gets the parsers used to analyze each token.
        /// </summary>
        private ITokenParser[] Parsers { get; }

        /// <summary>
        /// Tokenizes the input code into a list of tokens.
        /// </summary>
        /// <param name="InCode">The input code to tokenize.</param>
        /// <returns>A read-only list of tokens.</returns>
        /// <exception cref="ArgumentException">Thrown when an unknown character is encountered.</exception>
        public IReadOnlyList<IToken> Tokenize(string InCode)
        {
            var tokens = new List<IToken>();
            int index = 0;

            while (index < InCode.Length)
            {
                bool tokenFound = false;

                // Attempt to parse the next token using each parser.
                foreach (var parser in Parsers)
                {
                    try
                    {
                        var token = parser.Parse(InCode, index);
                        if (token == null)
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(token.TokenType))
                        {
                            tokens.Add(token);
                        }
                        index += token.Code.Length;
                        tokenFound = true;
                        break;
                    }
                    catch (ArgumentException)
                    {
                        // Skip this parser and try the next one.
                    }
                }

                if (!tokenFound)
                {
                    throw new ArgumentException($"Unknown character at position {index}: '{InCode[index]}'", nameof(InCode));
                }
            }

            return tokens.AsReadOnly();
        }
    }


}