using nf.protoscript.parser.token;
using System;

namespace nf.protoscript.parser
{
    /// <summary>
    /// Parser exception.
    /// </summary>
    public class ParserException
        : Exception
    {
        public ParserException(ParserErrorType InErrorType, CodeLine InCodeLn, Token InErrorToken = null)
        {
            ErrorType = InErrorType;
            ErrorCodeLine = InCodeLn;
            ErrorToken = InErrorToken;
        }

        /// <summary>
        /// Error type
        /// </summary>
        public ParserErrorType ErrorType { get; }

        /// <summary>
        /// Site of the error.
        /// </summary>
        public CodeLine ErrorCodeLine { get; private set; }

        /// <summary>
        /// Error token.
        /// </summary>
        public Token ErrorToken { get; }

        /// <summary>
        /// Column index of the error token.
        /// </summary>
        public int ErrorTokenColumnIndex
        {
            get
            {
                if (ErrorToken == null)
                { return -1; }

                int totalLen = ErrorCodeLine.Content.TrimEnd().Length;
                int colIndex = totalLen - ErrorToken.LengthToTheEnd;
                return colIndex;
            }
        }

        /// <summary>
        /// Get the string which indicates the place where the error occurred in.
        /// </summary>
        public string ErrorSiteString
        {
            get
            {
                if (ErrorToken != null)
                {
                    return $"{ErrorCodeLine.Reader.Filename}[{ErrorCodeLine.LineNumber}:{ErrorTokenColumnIndex}]";
                }
                return $"{ErrorCodeLine.SiteString}";
            }
        }

        // override error message.
        public override string Message
        {
            get
            {
                return $"{ErrorSiteString} : {ErrorType.AsciiMessage}";
            }
        }

    }

}
