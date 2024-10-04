using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Represents the type of errors that occur during parsing.
    /// </summary>
    public class ParserErrorType
        : LogTemplateDefault
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserErrorType" /> class with the specificed unique ID, message and param declarations.
        /// </summary>
        /// <param name="InUniqueID">The unique ID of the error.</param>
        /// <param name="InMessage">The error message in ASCII format.</param>
        /// <param name="InParamDecls">The parameter declarations for the error message.</param>
        public ParserErrorType(int InUniqueID, string InMessage, params (string, Type)[] InParamDecls)
            : base(ELoggerType.Error, "Parser", InUniqueID, InMessage, InParamDecls)
        {
            UniqueID = InUniqueID;
            AsciiMessage = InMessage;
        }

        /// <summary>
        /// Gets the unique ID of the error. This should be used as the key in a dictionary.
        /// </summary>
        public int UniqueID { get; }

        /// <summary>
        /// Gets the error message in ASCII format.
        /// </summary>
        public string AsciiMessage { get; }


        //
        // pre-defined error types.
        //

        /// <summary>
        /// Error type for file loading issues.
        /// </summary>
        public static ParserErrorType Parser_LoadFileError { get; }
            = new ParserErrorType(1, "Load file error");

        /// <summary>
        /// Error type for unrecognized sectors in the parsed content.
        /// </summary>
        public static ParserErrorType Parser_UnrecognizedSector { get; }
            = new ParserErrorType(100, "Unrecognized sector");

        /// <summary>
        /// Error type for unexpected tokens encoutered during parsing.
        /// </summary>
        public static ParserErrorType Factory_UnexpectedToken { get; }
            = new ParserErrorType(110, "Unexpected token {0}"
                , ("TokenName", typeof(Token))
                );

        /// <summary>
        /// Error type for unrecognized elements inthe parsed content.
        /// </summary>
        public static ParserErrorType Factory_UnrecognizedElement { get; }
            = new ParserErrorType(200, "Unrecognized element");

        /// <summary>
        /// Error type for unrecognized methods inthe parsed content.
        /// </summary>
        public static ParserErrorType Factory_UnrecognizedMethod { get; }
            = new ParserErrorType(210, "Unrecognized method");

        /// <summary>
        /// Error type for missing parent information when collecting data.
        /// </summary>
        public static ParserErrorType Collect_NoParentInfo { get; }
            = new ParserErrorType(300, "No Parent Info When Collecting.");

        /// <summary>
        /// Error type for unexpected tokens encountered during AST construction.
        /// </summary>
        public static ParserErrorType AST_UnexpectedToken { get; }
            = new ParserErrorType(400, "Unexpected token {0}, (expect: {1})"
                , ("ErrorToken", typeof(Token))
                , ("ExpectedTokenType", typeof(string[]))
                );

        /// <summary>
        /// Error type for unexpected term tokens encountered during AST construction.
        /// </summary>
        public static ParserErrorType AST_UnexpectedTermToken { get; }
            = new ParserErrorType(401, "Unexpected term token {0}"
                , ("ErrorToken", typeof(Token))
                );

        /// <summary>
        /// Error type for unexpected end of input during AST construction.
        /// </summary>
        public static ParserErrorType AST_UnexpectedEnd { get; }
            = new ParserErrorType(402, "Unexpected End");

        /// <summary>
        /// Error type for invalid expressions encountered during AST construction.
        /// </summary>
        public static ParserErrorType AST_InvalidExpression { get; }
            = new ParserErrorType(420, "Invalid expression : {0}"
                , ("ErrorToken", typeof(Token))
                );

        /// <summary>
        /// Error type for invalid parameters encountered during AST construction.
        /// </summary>
        public static ParserErrorType AST_InvalidParam { get; }
            = new ParserErrorType(421, "Invalid param: {0}"
                , ("ErrorToken", typeof(Token))
                );

    }


}
