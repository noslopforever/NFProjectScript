using nf.protoscript;
using nf.protoscript.test;
using System;
using System.Diagnostics;

namespace npsParser.test.basicEditorApplet
{


    partial class Program
    {

        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            ProjectInfo testProj = TestCases.BasicDataBinding();
            var translator = new SimpleHtml5PageTranslator();
            translator.Translate(testProj);
        }


    }



}
