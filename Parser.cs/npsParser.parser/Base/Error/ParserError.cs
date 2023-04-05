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

        public static ParserErrorType Parser_LoadFileError { get; }
            = new ParserErrorType(1, "Load file error");

        public static ParserErrorType Parser_UnrecognizedSector { get; }
            = new ParserErrorType(100, "Unrecognized sector");

        public static ParserErrorType Factory_UnexpectedToken { get; }
            = new ParserErrorType(110, "Unexpected token {0}");

        public static ParserErrorType Factory_UnrecognizedElement { get; }
            = new ParserErrorType(200, "Unrecognized element");

        public static ParserErrorType Factory_UnrecognizedMethod { get; }
            = new ParserErrorType(210, "Unrecognized method");

        public static ParserErrorType Collect_NoParentInfo { get; }
            = new ParserErrorType(300, "No Parent Info When Collecting.");

        public static ParserErrorType AST_UnexpectedToken { get; }
            = new ParserErrorType(400, "Unexpected token {0}, (expect: {1})");

        public static ParserErrorType AST_UnexpectedTermToken { get; }
            = new ParserErrorType(401, "Unexpected term token {0}");

        public static ParserErrorType AST_InvalidExpression { get; }
            = new ParserErrorType(420, "Invalid expression : {0}");

        public static ParserErrorType AST_InvalidParam { get; }
            = new ParserErrorType(421, "Invalid param: {0}");


    }


}
