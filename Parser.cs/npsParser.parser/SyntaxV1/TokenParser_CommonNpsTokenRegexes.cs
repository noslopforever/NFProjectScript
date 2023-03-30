namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Common regexes to check tokens.
    /// </summary>
    public static class TokenParser_CommonNpsTokenRegexes
    {
        /// <summary>
        /// Regex to check an at:
        /// </summary>
        public static string At { get; } = @"@";

        /// <summary>
        /// Regex to check a sharp:
        /// </summary>
        public static string Sharp { get; } = @"#";

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
        /// Implement function/lambda.
        /// </summary>
        public static string ImplementLambda { get; } = "=>";

        /// <summary>
        /// Compound assign operators
        /// </summary>
        public static string CompoundAssign { get; } = "[+\\-*/%&|]=";

        /// <summary>
        /// Equal operators
        /// </summary>
        public static string Operators_Equal { get; } = "[!<>]=?|==";

        /// <summary>
        /// Regex to check an operator (mathmatic and logic)
        /// DONOT use this regex with OperatorsAndDot.
        /// </summary>
        public static string Operators { get; } = @"[+\-*/%&|]";

        /// <summary>
        /// Regex to check an operator (mathmatic and logic) and a dot.
        /// DONOT use this regex with Operators.
        /// </summary>
        public static string OperatorsAndDot { get; } = @"[\.+\-*/%&|]";

        /// <summary>
        /// Regex to check an assign
        /// This regex's priority must be lower than Operators/OperatorsAndDot.
        /// </summary>
        public static string Assign { get; } = @"=";

        /// <summary>
        /// Regex to check an open-paren (
        /// </summary>
        public static string OpenParen { get; } = @"\(";

        /// <summary>
        /// Regex to check a close-paren )
        /// </summary>
        public static string CloseParen { get; } = @"\)";

        /// <summary>
        /// Regex to check an open-bracket [
        /// </summary>
        public static string OpenBracket { get; } = @"\[";

        /// <summary>
        /// Regex to check a close-bracket ]
        /// </summary>
        public static string CloseBracket { get; } = @"\]";

        /// <summary>
        /// Regex to check an open-brach {
        /// </summary>
        public static string OpenBrace { get; } = @"{";

        /// <summary>
        /// Regex to check a close-brach }
        /// </summary>
        public static string CloseBrace { get; } = @"}";

        /// <summary>
        /// Regex to check a colon :
        /// </summary>
        public static string Colon { get; } = @":";

        /// <summary>
        /// Regex to check skips like WhiteSpaces/Tabs/Returns.
        /// </summary>
        public static string Skips { get; } = @"[ \t\r]+";

    }


}