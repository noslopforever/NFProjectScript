using System;
using System.Collections.Generic;
using System.IO;

namespace nf.protoscript
{

    /// <summary>
    /// Log facades.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logger currently used.
        /// </summary>
        public static ILogger Instance { get; set; } = new LoggerDefault();

    }


}