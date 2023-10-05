using nf.protoscript;
using nf.protoscript.translator;
using nf.protoscript.translator.DefaultSnippetElements;

namespace npsParser.test.ClassTranslator
{
    internal static class TestXmlTranslator
    {
        public static InfoTranslatorDefault Load()
        {
            var xmlTrans = new InfoTranslatorDefault();
            xmlTrans.DefaultExprTranslator = new TestExprTranslatorCpp();

            xmlTrans.AddScheme("TypeTranslator",
                new InfoTranslateSnippet(
                    new ElementConstString("<")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(" Name=\"")
                    , new ElementNodeValue("Name")
                    , new ElementConstString("\">")
                    , new ElementNewLine()
                    , new ElementIndentBlock(
                        new ElementConstString("<properties>")
                        , new ElementNewLine()
                        , new ElementIndentBlock(
                            new ElementForeachSubCall("PropertyTranslator", "member")
                        )
                        , new ElementNewLine()
                        , new ElementConstString("</properties>")
                    )
                    , new ElementNewLine()
                    , new ElementIndentBlock(
                        new ElementConstString("<ctor>")
                        , new ElementNewLine()
                        , new ElementIndentBlock(
                            new ElementNewMethod("ctor",
                                new ElementChangeContext("HostType",
                                    new ElementForeachSubCall("CtorInitTranslator", "member")
                                )
                            )
                        )
                        , new ElementNewLine()
                        , new ElementConstString("</ctor>")
                    )
                    , new ElementNewLine()
                    , new ElementIndentBlock(
                        new ElementConstString("<methods>")
                        , new ElementNewLine()
                        , new ElementIndentBlock(
                            new ElementForeachSubCall("MethodTranslator", "method")
                        )
                        , new ElementNewLine()
                        , new ElementConstString("</methods>")
                    )
                    , new ElementNewLine()
                    , new ElementConstString("</")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(">")
                )
            );

            xmlTrans.AddScheme("PropertyTranslator",
                new InfoTranslateSnippet(
                    new ElementConstString("<")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(" Name=\"")
                    , new ElementNodeValue("Name")
                    , new ElementConstString("\" ")
                    , new ElementConstString(" Type=\"")
                    , new ElementNodeValue("ElementType")
                    , new ElementConstString("\" />")
                )
            );

            xmlTrans.AddScheme("CtorInitTranslator",
                new InfoTranslateSnippet(
                    new ElementConstString("// Init ")
                    , new ElementNodeValue("Name")
                    , new ElementNewLine()
                )
            );

            xmlTrans.AddScheme("MethodTranslator",
                new InfoTranslateSnippet(
                    new ElementConstString("<")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(" Name=\"")
                    , new ElementNodeValue("Name")
                    , new ElementConstString("\" >")
                    , new ElementNewLine()
                    , new ElementIndentBlock(
                        new ElementMethodBody()
                    )
                    , new ElementNewLine()
                    , new ElementConstString("</")
                    , new ElementNodeValue("Header")
                    , new ElementConstString(">")
                )
            );

            return xmlTrans;
        }
    }
}