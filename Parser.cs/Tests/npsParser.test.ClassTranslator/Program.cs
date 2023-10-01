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

    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            Console.WriteLine("Which translator do you want to test?");

            var xmlTrans = _LoadXmlTranslator();
            //var ueTrans = _LoadUECppTranslator();
            //var jsTrans = _LoadJsTranslator();
            //var pyTrans = _LoadPyTranslator();
            var selectedTranslator = xmlTrans;
            TestCases.BasicExprs().ForeachSubInfo<TypeInfo>(type => _GenerateTypeInfo(selectedTranslator, type));
            TestCases.AdvancedExpressions().ForeachSubInfo<TypeInfo>(type => _GenerateTypeInfo(selectedTranslator, type));
        }

        private static void _GenerateTypeInfo(InfoTranslatorDefault InTranslator, TypeInfo InTargetType)
        {
            Console.WriteLine($"Code emit sequences for Type: {InTargetType.Name}");

            var codeLns = InTranslator.TranslateInfo(InTargetType, "TypeTranslator");
            foreach ( var code in codeLns )
            {
                Console.WriteLine(code);
            }
        }

        private static InfoTranslatorDefault _LoadXmlTranslator()
        {
            var xmlTrans = new InfoTranslatorDefault();
            xmlTrans.AddScheme("TypeTranslator",
                new InfoTranslateSnippet(
                    new ElementConstString("<")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(" Name=\"")
                    , new ElementNodeValue("Name")
                    , new ElementConstString("\">")
                    , new ElementNewLine()
                    , new ElementIndentBlock(
                        new ElementConstString("<infos>")
                        , new ElementNewLine()
                        , new ElementIndentBlock(
                            new ElementForEachSubCall("InfoTranslator", "")
                        )
                        , new ElementNewLine()
                        , new ElementConstString("</infos>")
                    )
                    , new ElementNewLine()
                    //, new ElementIndentBlock(
                    //    new ElementConstString("<properties>")
                    //    , new ElementIndentBlock(
                    //        new ElementForeachSubCall("PropertyTranslator", "property")
                    //    )
                    //    , new ElementConstString("</properties>")
                    //)
                    //, new ElementIndentBlock(
                    //    new ElementConstString("<methods>")
                    //    , new ElementIndentBlock(
                    //        new ElementForeachSubCall("MethodTranslator", "method")
                    //    )
                    //    , new ElementConstString("</methods>")
                    //)
                    , new ElementConstString("</")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(">")
                )
            );

            xmlTrans.AddScheme("InfoTranslator",
                new InfoTranslateSnippet(
                    new ElementConstString("<")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(" Name=\"")
                    , new ElementNodeValue("Name")
                    , new ElementConstString("\" />")
                )
            );

            //xmlTrans.AddScheme("PropertyTranslator",
            //);

            //xmlTrans.AddScheme("MethodTranslator",
            //);

            return xmlTrans;
        }
    }


}