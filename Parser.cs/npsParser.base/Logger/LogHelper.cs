using System;

namespace nf.protoscript
{
    /// <summary>
    /// Helper functions of log system.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Bind the InInfo with its source.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <param name="InLogSource"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void BindLogSourceWithInfo(Info InInfo, ILogSource InLogSource)
        {
            InInfo.Extra.LogSource = InLogSource;
        }

        /// <summary>
        /// Bind the InInfo with which file it comes from.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <param name="InFile"></param>
        /// <param name="InLineIndex"></param>
        /// <param name="InColumnIndex"></param>
        public static void BindFileSourceWithInfo(Info InInfo, string InFile, int InLineIndex, int InColumnIndex)
        {
            var fileSrc = new LogSourceFileDefault(InFile, InLineIndex, InColumnIndex);
            BindLogSourceWithInfo(InInfo, fileSrc);
        }

        /// <summary>
        /// Try to exact the log source bound to the InInfo.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns>May return LogSource null if no source is bound with the InInfo.</returns>
        public static ILogSource ExactLogSourceFromInfo(Info InInfo)
        {
            try
            {
                return InInfo.Extra.LogSource as ILogSource;
            }
            catch { }

            return LogSourceNull.Instance;
        }

    }

}