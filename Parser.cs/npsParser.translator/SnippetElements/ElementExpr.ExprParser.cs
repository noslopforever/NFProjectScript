using nf.protoscript.syntaxtree;
using nf.protoscript.parser;
using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultScheme.Elements.Internal
{

    /// <summary>
    /// Provides an expression parser used in ElementExpr class.
    /// </summary>
    internal static class ElementExprParser
    {
        /// <summary>
        /// Initializes the tokenizer used by the expression parser.
        /// </summary>
        static ElementExprParser()
        {
            _tokenizer = new Tokenizer(
                new TokenParserRegex(@"[$a-zA-Z_][a-zA-Z0-9_$]*", "Identity")
                , new TokenParserRegex(@"\d+", "Number")
                , new TokenParserRegex(@"\d*\.\d+", "FloatingNumber")
                , new TokenParserRegex("\"(\\\\\"|[^\"])*\"", "String")
                , new TokenParserRegex(@"[\(\)\[\],]", "Separator")
                , new TokenParserRegex(@"[\+\-\*/%=\.]", "Operator")
                , new TokenParserRegex(@"[ \t\r]+", "") // Skip whitespace
            );
        }

        /// <summary>
        /// Parses the provided code and generates a syntax tree node.
        /// </summary>
        /// <param name="InCode">The code to parse.</param>
        /// <returns>The generated syntax tree node.</returns>
        public static ISyntaxTreeNode ParseCode(string InCode)
        {
            // Tokenize the code.
            var tokens = _tokenizer.Tokenize(InCode);

            // Use the expression parser to parse the tokens.
            var exprParser = new ASTParser_Expression();
            int startIndex = 0;
            var stNode = exprParser.Parse(tokens, ref startIndex);

            return stNode;
        }

        /// <summary>
        /// The tokenizer used by this parser.
        /// </summary>
        private static readonly Tokenizer _tokenizer;
    }
}