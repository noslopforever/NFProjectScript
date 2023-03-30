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


}