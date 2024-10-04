using System;
using System.Collections.Generic;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Represents a token in the parsing process.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Gets the type of the token, such as Identifier, Number, FloatingNumber, Operator, etc.
        /// See <see cref="CommonTokenTypes"/> for some pre-defined token type strings.
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

        /// <summary>
        /// Checks if the token matches the given TokenChecker.
        /// </summary>
        /// <param name="InChecker">The TokenChecker to use for validation.</param>
        /// <returns>True if the token matches the checker, otherwise false.</returns>
        bool CheckToken(TokenChecker InChecker);

        /// <summary>
        /// Check if the token matches the conditions list in the InCheckGroup.
        /// </summary>
        /// <param name="InCheckGroup">The token check group to use for validation.</param>
        /// <returns>True if the token matches the checker, otherwise false.</returns>
        bool Check(TokenCheckGroup InCheckGroup)
        {
            foreach (var checker in InCheckGroup.Checkers)
            {
                if (CheckToken(checker))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the token matches the given token type.
        /// </summary>
        /// <param name="InTokenType">The token type to check against.</param>
        /// <returns>True if the token matches the given token type, otherwise false.</returns>
        bool Check(string InTokenType)
        {
            return Check(new TokenCheckGroup(InTokenType));
        }

        /// <summary>
        /// Checks if the token matches the given token type and specific token codes.
        /// </summary>
        /// <param name="InTokenType">The token type to check against.</param>
        /// <param name="InSpecTokenCodes">The specific token codes to check against.</param>
        /// <returns>True if the token matches the given token type and specific token codes, otherwise false.</returns>
        bool Check(string InTokenType, params string[] InSpecTokenCodes)
        {
            return Check(new TokenCheckGroup(InTokenType, InSpecTokenCodes));
        }

        /// <summary>
        /// Checks if the token matches the given token type, specific token codes, and case sensitivity.
        /// </summary>
        /// <param name="InTokenType">The token type to check against.</param>
        /// <param name="InIgnoreCase">Indicates whether the comparison should be case-insensitive.</param>
        /// <param name="InSpecTokenCodes">The specific token codes to check against.</param>
        /// <returns>True if the token matches the given token type, specific token codes, and case sensitivity, otherwise false.</returns>
        bool Check(string InTokenType, bool InIgnoreCase, params string[] InSpecTokenCodes)
        {
            return Check(new TokenCheckGroup(InTokenType, InIgnoreCase, InSpecTokenCodes));
        }

        /// <summary>
        /// Checks if the token matches any of the given token checkers.
        /// </summary>
        /// <param name="InCheckers">The token checkers to use for validation.</param>
        /// <returns>True if the token matches any of the given token checkers, otherwise false.</returns>
        bool Check(params TokenChecker[] InCheckers)
        {
            return Check(new TokenCheckGroup(InCheckers));
        }


    }


    /// <summary>
    /// Represents a token checker used to validate tokens.
    /// </summary>
    public struct TokenChecker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenChecker"/> struct with default values.
        /// </summary>
        public TokenChecker()
        {
            TokenType = "";
            SpecificTokenCodes = new string[0];
            IgnoreCase = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenChecker"/> struct with the specified parameters.
        /// </summary>
        /// <param name="InTokenType">The token type to check against.</param>
        /// <param name="InSpecCodes">The specific token codes to check against.</param>
        /// <param name="InIgnoreCase">Indicates whether the comparison should be case-insensitive.</param>
        public TokenChecker(string InTokenType, string[] InSpecCodes, bool InIgnoreCase)
        {
            TokenType = InTokenType;
            SpecificTokenCodes = InSpecCodes;
            IgnoreCase = InIgnoreCase;
        }

        /// <summary>
        /// Gets or sets the token type to check against.
        /// </summary>
        public string TokenType;

        /// <summary>
        /// Gets or sets the specific token codes to check against.
        /// </summary>
        public string[] SpecificTokenCodes;

        /// <summary>
        /// Gets or sets a value indicating whether the comparison should be case-insensitive.
        /// </summary>
        public bool IgnoreCase;

    }

    /// <summary>
    /// Represents a group of token checkers used to validate tokens.
    /// </summary>
    public struct TokenCheckGroup
    {
        /// <summary>
        /// nitializes a new instance of the <see cref="TokenCheckGroup"/> struct with the specified token type.
        /// </summary>
        /// <param name="InTokenType">The token type to check against.</param>
        public TokenCheckGroup(string InTokenType)
        {
            Checkers = new TokenChecker[] { new TokenChecker(InTokenType, new string[0], true) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCheckGroup"/> struct with the specified token type and specific token codes.
        /// </summary>
        /// <param name="InTokenType">The token type to check against.</param>
        /// <param name="InSpecTokenCodes">The specific token codes to check against.</param>
        public TokenCheckGroup(string InTokenType, params string[] InSpecTokenCodes)
        {
            Checkers = new TokenChecker[] { new TokenChecker(InTokenType, InSpecTokenCodes, true) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCheckGroup"/> struct with the specified token type, specific token codes, and case sensitivity.
        /// </summary>
        /// <param name="InTokenType">The token type to check against.</param>
        /// <param name="InIgnoreCase">Indicates whether the comparison should be case-insensitive.</param>
        /// <param name="InSpecTokenCodes">>The specific token codes to check against.</param>
        public TokenCheckGroup(string InTokenType, bool InIgnoreCase, params string[] InSpecTokenCodes)
        {
            Checkers = new TokenChecker[] { new TokenChecker(InTokenType, InSpecTokenCodes, InIgnoreCase) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCheckGroup"/> struct with the specified token checkers.
        /// </summary>
        /// <param name="InCheckers">The token checkers to use for validation.</param>
        public TokenCheckGroup(params TokenChecker[] InCheckers)
        {
            Checkers = InCheckers;
        }

        /// <summary>
        /// Gets the token checkers in this group.
        /// </summary>
        public TokenChecker[] Checkers = new TokenChecker[0];

    }

    /// <summary>
    /// Interface for parsing a token from a string.
    /// Each parser parses a specific pattern of token.
    /// </summary>
    public interface ITokenParser
    {
        /// <summary>
        /// Parses a token from the specified code starting at the given index.
        /// </summary>
        /// <param name="InCode">The input code to parse.</param>
        /// <param name="InStartIndex">The starting index in the input code.</param>
        /// <returns>The parsed token or null if no token is found.</returns>
        IToken ParseCodeToToken(string InCode, int InStartIndex);
    }

}