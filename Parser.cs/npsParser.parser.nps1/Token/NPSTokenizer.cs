using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// NPS script tokenizer.
    /// </summary>
    public class NPSTokenizer
        : Tokenizer
    {
        public NPSTokenizer()
            : base(
                // @ word
                new TokenParserRegex("@", CommonTokenTypes.At),
                // # word
                new TokenParserRegex("#", CommonTokenTypes.Sharp),

                // DoubleQuoteString: "abc \"def\" ghi"
                new TokenParserRegex("\"(\\\\\"|[^\"])*\"", CommonTokenTypes.String),
                // QuoteString: 'string in quotes'
                new TokenParserRegex("'[^\']*'", CommonTokenTypes.String),
                // ApostropheString: `string in apostrophes`
                new TokenParserRegex("`[^`]*`", CommonTokenTypes.String),
                // Floating: 1.f, 3.14f, 0.5f  <<MUST BE HIGHER THAN INTEGER>>
                new TokenParserRegex(@"\d+\.(\d+)?[f]?", CommonTokenTypes.Floating),
                // E-floating: 1e3, 1e-4, 1.5e+2f
                new TokenParserRegex(@"\d+\.(\d+)?[eE][+-]?\d+", CommonTokenTypes.Floating),
                // Integer: 0, 1, 2, 3
                new TokenParserRegex(@"\d+", CommonTokenTypes.Integer),
                // Identifier: $abc, abc, keywords
                new TokenParserRegex(@"[$\w]+", CommonTokenTypes.ID),

                // Ellipsis: ... <<MUST BE HIGHER THAN OPERATOR(.)>>
                new TokenParserRegex(@"\.\.\.", CommonTokenTypes.Ellipsis),

                // operators: implement lambda <<MUST BE HIGHER THAN ASSIGN>>
                new TokenParserRegex("=>", CommonTokenTypes.Operator),
                // operators: assign or compound assign
                new TokenParserRegex("[+\\-*/%&|]?=", CommonTokenTypes.Operator),
                // Operators: comparasion
                new TokenParserRegex("[!<>]=?|==", CommonTokenTypes.Operator),
                // Operators: dot, mathmatical and logical operators
                new TokenParserRegex(@"[\.+\-*/%&|]", CommonTokenTypes.Operator),

                new TokenParserRegex(@"\,", CommonTokenTypes.Comma),
                new TokenParserRegex(@"\\", CommonTokenTypes.NextLine),
                new TokenParserRegex(@"\(", CommonTokenTypes.OpenParen),
                new TokenParserRegex(@"\)", CommonTokenTypes.CloseParen),
                new TokenParserRegex(@"\[", CommonTokenTypes.OpenBracket),
                new TokenParserRegex(@"\]", CommonTokenTypes.CloseBracket),
                new TokenParserRegex(@"\{", CommonTokenTypes.OpenBrace),
                new TokenParserRegex(@"\}", CommonTokenTypes.CloseBrace),
                new TokenParserRegex(@"\:", CommonTokenTypes.Colon),
                new TokenParserRegex(@"\;", CommonTokenTypes.Semicolon),
                new TokenParserRegex(@"[ \t\r]+", CommonTokenTypes.Skip)

            )
        {
        }

        public static NPSTokenizer Instance { get; } = new NPSTokenizer();

    }

    
}