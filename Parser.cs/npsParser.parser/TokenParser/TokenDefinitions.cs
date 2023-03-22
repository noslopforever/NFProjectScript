using System;
using System.Linq;
using System.Collections.Generic;

namespace nf.protoscript.parser.token
{

    /// <summary>
    /// Token definitions
    /// </summary>
    public enum ETokenType : int
    {
        /// <summary>
        /// Unknown token type.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Strings
        /// </summary>
        String = 0,

        /// <summary>
        /// Floating number
        /// </summary>
        Floating = 1,

        /// <summary>
        /// Integer number
        /// </summary>
        Integer = 2,

        /// <summary>
        /// Identity
        /// </summary>
        ID = 100,

        /// <summary>
        /// ,
        /// </summary>
        Comma = 200,

        /// <summary>
        /// \
        /// </summary>
        NextLine = 201,

        /// <summary>
        /// #
        /// </summary>
        Sharp = 202,

        /// <summary>
        /// @
        /// </summary>
        At = 203,

        /// <summary>
        /// + - * / % += -= *= /= %= == != ! & | < > <= >= 
        /// </summary>
        Operator = 300,

        /// <summary>
        /// =
        /// </summary>
        Assign = 301,

        /// <summary>
        /// .
        /// </summary>
        Dot = 302,

        /// <summary>
        /// :
        /// </summary>
        Colon = 303,

        /// <summary>
        /// ...
        /// </summary>
        Ellipsis = 304,

        /// <summary>
        /// (
        /// </summary>
        OpenParen = 401,

        /// <summary>
        /// )
        /// </summary>
        CloseParen = 402,

        /// <summary>
        /// [
        /// </summary>
        OpenBracket = 403,

        /// <summary>
        /// ]
        /// </summary>
        CloseBracket = 404,

        /// <summary>
        /// {
        /// </summary>
        OpenBrace = 405,

        /// <summary>
        /// }
        /// </summary>
        CloseBrace = 406,

        /// <summary>
        /// $WhiteSpace
        /// </summary>
        Skip = 10000,

    }


    /// <summary>
    /// Common regexes to check tokens.
    /// </summary>
    public static class CommonTokenRegexes
    {
        /// <summary>
        /// Regex to check an at:
        /// </summary>
        public static string At { get; } = (@"@");

        /// <summary>
        /// Regex to check a sharp:
        /// </summary>
        public static string Sharp { get; } = (@"#");

        /// <summary>
        /// Regex to check "abc \"def\" ghi"
        /// </summary>
        public static string DoubleQuoteString { get; } = "\"(\\\\\"|[^\"])*\"";

        /// <summary>
        /// Regex to check 'string in quotes'
        /// </summary>
        public static string QuoteString { get; } = "'([^'])*'";

        /// <summary>
        /// Regex to check `string in apostrophes`
        /// </summary>
        public static string ApostropheString { get; } = "`([^`])*`";

        /// <summary>
        /// Regex to check floating numbers like 1.f, 3.14f, 0.5f
        /// This regex's priority must be higher than Integer.
        /// </summary>
        public static string FloatingNumber { get; } = @"\d+\.[\d]*[f]?";

        /// <summary>
        /// Regex to check E-floating numbers like 1e3, 1e-4
        /// This regex's priority must be higher than Integer.
        /// </summary>
        public static string EFloatingNumber { get; } = @"\d+e[\-]?\d+";

        /// <summary>
        /// Regex to check integer numbers like 0,1,2,3,4
        /// </summary>
        public static string Integer { get; } = @"\d+";

        /// <summary>
        /// Regex to check identity.
        /// This regex's priority must be lower than Integer.
        /// </summary>
        public static string Identity { get; } = @"[$\w]+";

        /// <summary>
        /// Regex to check a comma.
        /// </summary>
        public static string Comma { get; } = @"\,";

        /// <summary>
        /// Regex to check a backward-slash.
        /// </summary>
        public static string BackwardSlash { get; } = @"\\";

        /// <summary>
        /// Regex to check a ellipsis (...)
        /// This regex's priority must be higher than Dot.
        /// </summary>
        public static string Ellipsis { get; } = @"\.\.\.";

        /// <summary>
        /// Regex to check a dot (.)
        /// </summary>
        public static string Dot { get; } = @".\";

        /// <summary>
        /// Regex to check an operator (mathmatic and logic)
        /// DONOT use this regex with OperatorsAndDot.
        /// </summary>
        public static string Operators { get; } = @"[+\-*/%&|]|[!<>]=?|==";

        /// <summary>
        /// Regex to check an operator (mathmatic and logic) and a dot.
        /// DONOT use this regex with Operators.
        /// </summary>
        public static string OperatorsAndDot { get; } = @"[+\-*/%&|]|[!<>]=?|==|\.";

        /// <summary>
        /// Regex to check an assign
        /// This regex's priority must be lower than Operators/OperatorsAndDot.
        /// </summary>
        public static string Assign { get; } = (@"[+\-*/%&|]?=");

        /// <summary>
        /// Regex to check an open-paren (
        /// </summary>
        public static string OpenParen { get; } = (@"\(");

        /// <summary>
        /// Regex to check a close-paren )
        /// </summary>
        public static string CloseParen { get; } = (@"\)");

        /// <summary>
        /// Regex to check an open-bracket [
        /// </summary>
        public static string OpenBracket { get; } = (@"\[");

        /// <summary>
        /// Regex to check a close-bracket ]
        /// </summary>
        public static string CloseBracket { get; } = (@"\]");

        /// <summary>
        /// Regex to check an open-brach {
        /// </summary>
        public static string OpenBrace { get; } = (@"{");

        /// <summary>
        /// Regex to check a close-brach }
        /// </summary>
        public static string CloseBrace { get; } = (@"}");

        /// <summary>
        /// Regex to check a colon :
        /// </summary>
        public static string Colon { get; } = (@":");

        /// <summary>
        /// Regex to check skips like WhiteSpaces/Tabs/Returns.
        /// </summary>
        public static string Skips { get; } = (@"[ \t\r]+");

        public static string ALL { get; } = ".";

    }


}