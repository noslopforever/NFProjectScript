using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Resources;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using nf.protoscript.test;
using nf.protoscript.translator;
using nf.protoscript.translator.DefaultScheme;
using nf.protoscript.translator.DefaultScheme.Elements;

namespace nf.protoscript.test
{

    partial class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            Console.WriteLine("Which translator do you want to test?");

            // Construct the XML Info Translator.
            var xmlTranslator = new InfoTranslatorDefault();
            {
                SchemeYamlLoader.LoadSchemes(xmlTranslator, Resources.ExpressionSchemes);
                SchemeYamlLoader.LoadSchemes(xmlTranslator, Resources.Schemes_XmlTranslatorSchemes);
            }
            // Construct the Gearset JS Translator.
            var gearsetJSTranslator = new InfoTranslatorDefault();
            {
                SchemeYamlLoader.LoadSchemes(gearsetJSTranslator, Resources.ExpressionSchemes);
                SchemeYamlLoader.LoadSchemes(gearsetJSTranslator, Resources.Schemes_GearsetJSTranslatorSchemes);
            }

            var testTranslators = new InfoTranslatorAbstract[]
            {
                xmlTranslator
                , gearsetJSTranslator
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