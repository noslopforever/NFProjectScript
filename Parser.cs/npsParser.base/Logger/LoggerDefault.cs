using System;
using System.Collections.Generic;
using System.IO;

namespace nf.protoscript
{

    /// <summary>
    /// Default logger, write logs to console, use 'ANSI Colour codes' to present different colors;
    /// </summary>
    public class LoggerDefault : ILogger
    {
        TextWriter InfoWriter { get; set; } = Console.Out;
        TextWriter WarningWriter { get; set; } = Console.Out;
        TextWriter ErrorWriter { get; set; } = Console.Out;
        TextWriter InternalWriter { get; set; } = Console.Out;


        public string FileScope { get; set; } = "";

        public void Log(ELoggerType InType, string InGroup, int InLogCodeID, string InLogCode, params object[] InAppendParams)
        {
            TextWriter writer = SelectWriter(InType);
            string typeHead = SelectTypeCode(InType);
            string colorHead = SelectTypeColorANSICode(InType);

            writer.Write($"{colorHead}{typeHead}{InLogCodeID} > ");

            string log = string.Format(InLogCode, InAppendParams);
            writer.Write(log);
            writer.WriteLine($"\u001b[0m");
        }

        public void Log(ELoggerType InType, int InLine, int InColumn, string InGroup, int InLogCodeID, string InLogCode, params object[] InAppendParams)
        {
            string log = $"[{FileScope}][{InLine}] : " + string.Format(InLogCode, InAppendParams);
            Log(InType, InGroup, InLogCodeID, log);
        }



        private static string SelectTypeCode(ELoggerType InType)
        {
            switch (InType)
            {
                case ELoggerType.Verbose:   return "V";
                case ELoggerType.Info:      return "I";
                case ELoggerType.Warning:   return "W";
                case ELoggerType.Error:     return "E";
                case ELoggerType.Fatal:     return "F";
                case ELoggerType.Internal:  return "X";
            }
            return "?";
        }

        private static ConsoleColor SelectTypeColor(ELoggerType InType)
        {
            switch (InType)
            {
                case ELoggerType.Verbose:   return ConsoleColor.Gray;
                case ELoggerType.Info:      return ConsoleColor.White;
                case ELoggerType.Warning:   return ConsoleColor.Yellow;
                case ELoggerType.Error:     return ConsoleColor.Red;
                case ELoggerType.Fatal:     return ConsoleColor.DarkRed;
                case ELoggerType.Internal:  return ConsoleColor.Green;
            }
            return ConsoleColor.White;
        }

        private static string SelectTypeColorANSICode(ELoggerType InType)
        {
            switch (InType)
            {
                case ELoggerType.Verbose:   return "\u001b[90m";
                case ELoggerType.Info:      return "\u001b[37m";
                case ELoggerType.Warning:   return "\u001b[33m";
                case ELoggerType.Error:     return "\u001b[31m";
                case ELoggerType.Fatal:     return "\u001b[1;31m";
                case ELoggerType.Internal:  return "\u001b[32m";
            }
            return "\u001b[0m";
        }


        private TextWriter SelectWriter(ELoggerType InType)
        {
            TextWriter writer = InfoWriter;
            switch (InType)
            {
                case ELoggerType.Verbose:
                case ELoggerType.Info:
                    writer = InfoWriter;
                    break;
                case ELoggerType.Warning:
                    writer = WarningWriter;
                    break;
                case ELoggerType.Error:
                case ELoggerType.Fatal:
                    writer = ErrorWriter;
                    break;
                case ELoggerType.Internal:
                    writer = InternalWriter;
                    break;
            }

            return writer;
        }
    }


}