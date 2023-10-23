using System;

namespace nf.protoscript
{

    /// <summary>
    /// "Null" log source.
    /// </summary>
    public class LogSourceNull
        : ILogSource
    {
        private LogSourceNull() { }

        // Begin ILogSource interfaces
        public string SourceString { get { return "null"; } }
        // ~ End ILogSource interfaces

        /// <summary>
        /// The single instance of the "null" log source.
        /// </summary>
        public static LogSourceNull Instance { get; } = new LogSourceNull();
    }


    /// <summary>
    /// A log source that records which file/line/column the target event occurred on.
    /// </summary>
    public class LogSourceFileDefault
        : IFileLogSource
    {
        public LogSourceFileDefault(string InSourceFile, int InSourceLine, int InSourceColumn)
        {
            SourceFile = InSourceFile;
            SourceLine = InSourceLine;
            SourceColumn = InSourceColumn;
        }

        // Begin IFileLogSource interfaces
        public string SourceString { get { return ToString(); } }
        public string SourceFile { get; }
        public int SourceLine { get; }
        public int SourceColumn { get; }
        // ~ End IFileLogSource interfaces

        // Begin object interfaces
        public override string ToString()
        {
            return $"{SourceFile} ({SourceLine}:{SourceColumn})";
        }
        // ~ End object interfaces

    }

}