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
            , Token InErrorToken = null
            , params object[] InAppends
            )
        {
            ErrorType = InErrorType;
            ErrorToken = InErrorToken;
            AppendData = InAppends;
        }

        /// <summary>
        /// Error type
        /// </summary>
        public ParserErrorType ErrorType { get; }

        /// <summary>
        /// Error token.
        /// </summary>
        public Token ErrorToken { get; }

        /// <summary>
        /// Append data
        /// </summary>
        public object[] AppendData { get; }

        /// <summary>
        /// Localized error message.
        /// </summary>
        public string GetLocalizedMessage()
        {
            // TODO find error message by UniqueID in different locales.
            object[] parms = _GetMessageAppendDatas();

            return Message;
        }

        // override error message.
        public override string Message
        {
            get
            {
                object[] parms = _GetMessageAppendDatas();
                return string.Format($"{ErrorType.AsciiMessage}", parms);
            }
        }

        /// <summary>
        /// Merge AppendData with ErrorToken-string.
        /// </summary>
        /// <returns></returns>
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
