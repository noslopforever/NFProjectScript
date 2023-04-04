using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Error type of the parser.
    /// </summary>
    public class ParserErrorType
    {
        public ParserErrorType(int InUniqueID, string InMessage)
        {
            UniqueID = InUniqueID;
            AsciiMessage = InMessage;
        }

        /// <summary>
        /// Unique ID of the error. Should be used as the key in dictionary.
        /// </summary>
        public int UniqueID { get; }

        /// <summary>
        /// Message in ascii codec.
        /// </summary>
        public string AsciiMessage { get; }

        //
        // pre-defined error types.
        //

        public static ParserErrorType UnexpectedToken { get; }
            = new ParserErrorType(100, "Unexpected token");

        public static ParserErrorType UnrecognizedSector { get; }
            = new ParserErrorType(101, "Unrecognized sector");

        public static ParserErrorType UnrecognizedElement { get; }
            = new ParserErrorType(200, "Unrecognized element");

        public static ParserErrorType UnrecognizedMethod { get; }
            = new ParserErrorType(210, "Unrecognized method");

    }


}
