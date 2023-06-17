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
                    Console.WriteLine($"Write Codes for {mtdInfo.Name}:");

                    IExprCodeEmitter emitter = new ExprCodeEmitter_Log();
                    CodeEmitVisitor emitVisitor = new CodeEmitVisitor(mtdInfo, emitter);
                    VisitByReflectionHelper.FindAndCallVisit(mtdInfo.InitSyntax, emitVisitor);

                    foreach(string codeLn in emitVisitor.EmittedCode.Codes)
                    {
                        Console.WriteLine("    " + codeLn);
                    }
                });

        }
    }
}
