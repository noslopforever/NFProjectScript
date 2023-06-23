using nf.protoscript;
using nf.protoscript.syntaxtree;
using nf.protoscript.test;
using nf.protoscript.translator.expression;
using System;
using System.Diagnostics;

namespace npsParser.test.ExpressionTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            TypeInfo testExprs = TestCases.BasicExpressions();
            testExprs.ForeachSubInfoByHeader<ElementInfo>("method", mtdInfo =>
                {
                    Console.WriteLine($"Code emit sequences for {mtdInfo.Name}:");

                    var generator = new ExprCodeGen_Log();
                    generator.GenFunctionCodes(mtdInfo.Name, mtdInfo, mtdInfo.InitSyntax);
 
                    foreach (string codeLn in generator.Results)
                    {
                        Console.WriteLine("    " + codeLn);
                    }
                });

        }
    }
}
