using System;
using System.Collections.Generic;

namespace nf.protoscript
{

    /// <summary>
    /// Type of a log message.
    /// </summary>
    public enum ELoggerType
    {
        Verbose,
        Info,
        Warning,
        Error,
        Fatal,
        Internal,
    }

    /// <summary>
    /// Logger interface.
    /// </summary>
    public interface ILogger
    {

        /// <summary>
        /// Generic log.
        /// </summary>
        void Log(ELoggerType InType, string InGroup, int InLogCodeID, string InLogCode, params object[] InAppendParams);

        /// <summary>
        /// The current file scope.
        /// </summary>
        string FileScope { get; set; }

        /// <summary>
        /// File scope based log.
        /// </summary>
        void Log(ELoggerType InType, int InLine, int InColumn, string InGroup, int InLogCodeID, string InLogCode, params object[] InAppendParams);

    }


}