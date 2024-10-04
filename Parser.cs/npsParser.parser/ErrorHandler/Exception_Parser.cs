using System;

namespace nf.protoscript.parser
{
    /// <summary>
    /// Represents an exception that is thrown when a parsing error occurs.
    /// </summary>
    public class ParserException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserException" /> class with the specificed error type and optional error params.
        /// </summary>
        /// <param name="InErrorType">The type of the parsing error.</param>
        /// <param name="InErrorToken">The token associated with the error, if any. Defaults to null.</param>
        /// <param name="InAppends">Addtional data to append to the error message. Defaults to an empty array.</param>
        public ParserException(ParserErrorType InErrorType
            , IToken InErrorToken = null
            , params object[] InAppends
            )
        {
            ErrorType = InErrorType;
            ErrorToken = InErrorToken;
            AppendData = InAppends;
        }

        /// <summary>
        /// Gets the type of the parsing error.
        /// </summary>
        public ParserErrorType ErrorType { get; }

        /// <summary>
        /// Gets the token associated with the error, if any.
        /// </summary>
        public IToken ErrorToken { get; }

        /// <summary>
        /// Gets the additional data to append to the error message.
        /// </summary>
        public object[] AppendData { get; }

        /// <summary>
        /// Gets the localized error message.
        /// </summary>
        public string GetLocalizedMessage()
        {
            // TODO find error message by UniqueID in different locales.
            object[] parms = _GetMessageAppendDatas();

            return Message;
        }

        /// <inheritdoc />
        public override string Message
        {
            get
            {
                object[] parms = _GetMessageAppendDatas();
                return string.Format($"{ErrorType.AsciiMessage}", parms);
            }
        }

        /// <summary>
        /// Merges the AppendData with the ErrorToken-string.
        /// </summary>
        /// <returns>An array of objects containing the merged data.</returns>
        private object[] _GetMessageAppendDatas()
        {
            object[] parms = new object[AppendData.Length + 1];
            parms[0] = ErrorToken != null ? ErrorToken.ToString() : "<null>";
            for (int i = 0; i < AppendData.Length; i++)
            {
                var appData = AppendData[i];
                parms[i] = appData != null ? appData : "<null>";
            }

            return parms;
        }
    }

}
