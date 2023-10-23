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
    /// Log source.
    /// </summary>
    public interface ILogSource
    {
        /// <summary>
        /// Log source string.
        /// </summary>
        string SourceString { get; }
    }

    /// <summary>
    /// Log source from a file.
    /// </summary>
    public interface IFileLogSource
        :ILogSource
    {
        /// <summary>
        /// Source file location
        /// </summary>
        string SourceFile { get; }

        /// <summary>
        /// Source line
        /// </summary>
        int SourceLine { get; }

        /// <summary>
        /// Source column
        /// </summary>
        int SourceColumn { get; }

    }

    /// <summary>
    /// A log entry
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Type of the log.
        /// </summary>
        ELoggerType LoggerType { get; }

        /// <summary>
        /// Source of the log.
        /// </summary>
        ILogSource LogSource { get; }

        /// <summary>
        /// Group of the log.
        /// </summary>
        string Group { get; }

        /// <summary>
        /// Code ID of the log.
        /// </summary>
        int LogCodeID { get; }

        /// <summary>
        /// Message of the log.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Append data of the log.
        /// </summary>
        object[] Appends { get; }

    }

    /// <summary>
    /// Logger interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs recorded.
        /// </summary>
        IEnumerable<ILog> RecordLogs { get; }

        /// <summary>
        /// Records a log.
        /// </summary>
        /// <param name="InLog"></param>
        void Log(ILog InLog);

        /// <summary>
        /// Generic none-source log.
        /// </summary>
        void Log(ELoggerType InType, string InGroup, int InLogCodeID, string InLogCode, params object[] InAppendParams);

        /// <summary>
        /// The current file scope.
        /// </summary>
        /// TODO deprecated by LogSources.
        string FileScope { get; set; }

        /// <summary>
        /// File scope based log.
        /// </summary>
        void Log(ELoggerType InType, int InLine, int InColumn, string InGroup, int InLogCodeID, string InLogCode, params object[] InAppendParams);

    }


    /// <summary>
    /// Common Template of logs in similar format.
    /// </summary>
    public interface ILogTemplate
    {
        /// <summary>
        /// Type of the log.
        /// </summary>
        ELoggerType DefaultLoggerType { get; }

        /// <summary>
        /// Group of the log.
        /// </summary>
        string Group { get; }

        /// <summary>
        /// CodeID of the log.
        /// </summary>
        int LogCodeID { get; }

        /// <summary>
        /// Message template.
        /// </summary>
        string MessageTemplate { get; }

        /// <summary>
        /// Append data required by the log.
        /// </summary>
        IList<(string, Type)> ParamDecls { get;}

    }

}