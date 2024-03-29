using System;
using System.IO;
using System.Collections.Generic;
using CommandLine;
using System.Diagnostics;
using nf.protoscript;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.cs
{

    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option("stepmode", Required = false, HelpText = "Step to run, for debug purpose.")]
        public bool StepMode { get; set; } = false;

        [Option("showtokens", Required = false, HelpText = "Set output to verbose Tokens.")]
        public bool ShowToken { get; set; } = false;

        [Option("showstatements", Required = false, HelpText = "Set output to verbose Statements.")]
        public bool ShowStatements { get; set; } = false;

        [Option("showmodels", Required = false, HelpText = "Set output to verbose Models.")]
        public bool ShowModels { get; set; } = false;

        [Option('f', "file", Default = "", HelpText = "Implicit a file to be parsed.")]
        public string File { get; set; }

        [Option('d', "directory", Default = ".", HelpText = "Implicit a directory that all files in it will be parsed.")]
        public string Directory { get; set; }

        [Option('l', "targetlang", Default = "", HelpText = "Translate nps to the target language.")]
        public string TargetLang { get; set; }

        [Option('t', "targetdir", Default = "", HelpText = "All output files will be found in this directory.")]
        public string TargetDir { get; set; }

        /// <summary>
        /// Scheme root directories.
        /// </summary>
        [Option("schemeroot", HelpText = "Add directories which contain schemes to find.")]
        public IEnumerable<string> SchemeRoot { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;
            Console.WriteLine("Start npsc.cs compiler. PID = {0}, BASEDIR={1}", procID, AppDomain.CurrentDomain.BaseDirectory);
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    HandleStepMode(o.StepMode);

                    // 
                });

        }


        private static void HandleStepMode(bool InEnabled)
        {
            if (InEnabled)
            {
                Console.WriteLine("[Parser.cs]: STEP MODE, Press <ENTER> to Continue ... ");
                Console.Read();
            }
        }


    }
}
