using nf.protoscript.translator;
using nf.protoscript.translator.DefaultSnippetElements;

namespace npsParser.test.ClassTranslator
{
    internal static class TestXmlTranslator
    {
        public static InfoTranslatorDefault Load()
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