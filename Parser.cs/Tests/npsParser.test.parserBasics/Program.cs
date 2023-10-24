using nf.protoscript.modelchecker;
using nf.protoscript.parser;
using nf.protoscript.Serialization;
using nf.protoscript.utils.serialization.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace nf.protoscript.test
{
    class Program
    {
        static void Main(string[] args)
        {
            parser.syntax1.Parser testParser = parser.syntax1.Parser.CreateDefault();

            // Parse nps file.
            ProjectInfo testProj = new ProjectInfo("TestProject");
            ICodeContentReader reader = StringCodeContentReader.LoadFromString("TestCodeFile", TextResources.TestCodeFile);

            testParser.Parse(testProj, reader);

            // Perform post-parse checkers.
            {
                ElementOverrideCollector ovrElemsCollector = new ElementOverrideCollector();
                ovrElemsCollector.GatherOverrideElementsInProject(testProj);
                ovrElemsCollector.Dump(Console.Out);

                // Analysis override elements.
                List<ILog> checkErrors = new List<ILog>();
                foreach (var root in ovrElemsCollector.RootNodes)
                {
                    ElementOverrideAnalyzer.AnalyzeOverrideTree(checkErrors, root);
                }

                // Dump errors.
                foreach (var log in checkErrors)
                {
                    Logger.Instance.Log(log);
                }
            }

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
