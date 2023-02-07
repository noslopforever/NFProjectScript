using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using System.Runtime.InteropServices;
using nf.protoscript.Serialization;
using System.Xml.Serialization;
using System.Text.Json;

namespace nf.protoscript.test
{

    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;

            ProjectInfo testProj = TestCases.BasicLanguage();

            // Serialization: gather testProj to an InfoData.
            var gatheredProjData = InfoGatherer.Gather(testProj);

            // Test deserialization from data immediately
            {
                try
                {
                    ProjectInfo restoredProj = InfoGatherer.Restore(null, gatheredProjData) as ProjectInfo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Immediately deserialize failed.");
                    Console.WriteLine(ex.Message);
                }
            }

            // test serializer: XML
            {
                try
                {
                    XmlSerializer xmlSerial = new XmlSerializer(typeof(SerializationFriendlyData));
                    TextWriter writer = new StringWriter();
                    xmlSerial.Serialize(writer, gatheredProjData);
                    Console.WriteLine(writer.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Xml writer failed.");
                    Console.WriteLine(ex.Message);
                }

            }

            // test serializer: Json
            {
                try
                {
                    var serializeOption = new JsonSerializerOptions()
                    {
                        AllowTrailingCommas = false,
                        WriteIndented = true,
                    };
                    string jsonStr = JsonSerializer.Serialize(gatheredProjData, serializeOption);
                    Console.WriteLine(jsonStr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Json writer failed.");
                    Console.WriteLine(ex.Message);
                }
            }

        }

    }
}
