using System;
using System.Linq;
using System.Collections.Generic;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Common Token Type Strings. This is used to identify the token type in IToken.
    /// </summary>
    public static class CommonTokenTypes
    {
        /// <summary>
        /// 'White space', this token should be skipped when parsing.
        /// </summary>
        public const string Skip = "$SKIP";

        /// <summary>
        /// Unknown token type.
        /// </summary>
        public const string Unknown = "Unknown";

        /// <summary>
        /// Strings
        /// </summary>
        public  const string String = "String";

        /// <summary>
        /// Floating number
        /// </summary>
        public const string Floating = "Floating";

        /// <summary>
        /// Integer number
        /// </summary>
        public const string Integer = "Integer";

        /// <summary>
        /// Identifier
        /// </summary>
        public const string ID = "ID";

        /// <summary>
        /// ,
        /// </summary>
        public const string Comma = ",";

        /// <summary>
        /// \
        /// </summary>
        public const string NextLine = "$NL";

        /// <summary>
        /// #
        /// </summary>
        public const string Sharp = "#";

        /// <summary>
        /// @
        /// </summary>
        public const string At = "@";

        /// <summary>
        /// + - * / % = += -= *= /= %= == != ! & | < > <= >=
        /// </summary>
        public const string Operator = "Operator";

        /// <summary>
        /// .
        /// </summary>
        public const string Dot = ".";

        /// <summary>
        /// :
        /// </summary>
        public const string Colon = ":";

        /// <summary>
        /// ;
        /// </summary>
        public const string Semicolon = ";";

        /// <summary>
        /// ...
        /// </summary>
        public const string Ellipsis = "...";

        /// <summary>
        /// (
        /// </summary>
        public const string OpenParen = "(";

        /// <summary>
        /// )
        /// </summary>
        public const string CloseParen = ")";

        /// <summary>
        /// [
        /// </summary>
        public const string OpenBracket = "[";

        /// <summary>
        /// ]
        /// </summary>
        public const string CloseBracket = "]";

        /// <summary>
        /// {
        /// </summary>
        public const string OpenBrace = "{";

        /// <summary>
        /// }
        /// </summary>
        public const string CloseBrace = "}";


    }



}