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
        public ParserException(ParserErrorType InErrorType
            , CodeLine InCodeLn
            , Token InErrorToken = null
            , params object[] InAppends
            )
        {
            ErrorType = InErrorType;
            ErrorCodeLine = InCodeLn;
            ErrorToken = InErrorToken;
            AppendData = InAppends;
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
        /// Append data
        /// </summary>
        public object[] AppendData { get; }

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
                object[] parms = new object[AppendData.Length + 1];
                parms[0] = ErrorToken != null ? ErrorToken.ToString() : "<null>";
                for (int i = 0; i < AppendData.Length; i++)
                {
                    var appData = AppendData[i];
                    parms[i] = appData != null ? appData : "<null>";
                }

                return string.Format($"{ErrorSiteString} : {ErrorType.AsciiMessage}", parms);
            }
        }

    }

}
