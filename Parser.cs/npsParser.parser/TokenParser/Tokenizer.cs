using System;
using System.Collections.Generic;

namespace nf.protoscript.parser
{
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
                        var token = parser.ParseCodeToToken(InCode, index);
                        if (token == null)
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(token.TokenType)
                            && !token.Check(CommonTokenTypes.Skip)
                            )
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