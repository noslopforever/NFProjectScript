using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using nf.protoscript.test;
using nf.protoscript.translator;
using nf.protoscript.translator.DefaultSnippetElements;

namespace npsParser.test.ClassTranslator
{

    partial class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            Console.WriteLine("Which translator do you want to test?");

            var testTranslators = new InfoTranslatorAbstract[]
            {
                new TestXmlTranslator()
                , new TestGearsetJSTranslator()
            };
            foreach (var translator in testTranslators)
            {
                TestCases.BasicExprs().ForeachSubInfo<TypeInfo>(type => _GenerateTypeInfo(translator, type));
                TestCases.AdvancedExpressions().ForeachSubInfo<TypeInfo>(type => _GenerateTypeInfo(translator, type));
            }
        }

        private static void _GenerateTypeInfo(InfoTranslatorAbstract InTranslator, TypeInfo InTargetType)
        {
            Console.WriteLine($"Code emit sequences for Type: {InTargetType.Name}");

            TranslatingInfoContext typeCtx = new TranslatingInfoContext(null, InTargetType);
            var codeLns = InTranslator.TranslateInfo(typeCtx, "CommonTypeTranslator");
            foreach (var code in codeLns)
            {
                Console.WriteLine(code);
            }
        }
    }


}