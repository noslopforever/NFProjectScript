using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using System.Runtime.InteropServices;

namespace nf.protoscript.test
{

    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            ProjectInfo testProj = TestCases.BasicLanguage();

            // Translator: translate the project to a target development environment.
            {
                TestCppTranslator cppTranslator = new TestCppTranslator();
                cppTranslator.Translate(testProj);
            }

        }

    }
}
