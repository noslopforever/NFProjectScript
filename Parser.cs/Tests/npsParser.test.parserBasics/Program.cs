using nf.protoscript.parser;
using nf.protoscript.Serialization;
using nf.protoscript.utils.serialization.xml;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace nf.protoscript.test
{
    class Program
    {
        static void Main(string[] args)
        {
            parser.syntax1.Parser testParser = new parser.syntax1.Parser(parser.syntax1.Parser.ESystemDefault.On);

            ICodeContentReader reader = null;
            ProjectInfo testProj = new ProjectInfo("TestProject");
            testParser.Parse(testProj, reader);

            // Output infos parsed by the test-parser
            var gatheredProjData = InfoGatherer.Gather(testProj);
            string xmlResult = "";
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Test Writer
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = new UTF8Encoding(false);
                    settings.NewLineChars = Environment.NewLine;

                    using (XmlWriter xmlWriter = XmlWriter.Create(ms, settings))
                    {
                        SFDXmlSerializer.WriteSFDProperty(xmlWriter, "Project", gatheredProjData);
                    }

                    xmlResult = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine(xmlResult);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Xml writer failed.");
                Console.WriteLine(ex.Message);
            }

        }
    }
}
