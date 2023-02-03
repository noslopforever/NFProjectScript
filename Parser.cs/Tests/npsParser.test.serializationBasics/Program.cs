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

            {
                // Serialization: serialize testProj to an InfoSerializationData.
                var serialData = InfoSerializer.Serialize(testProj);

                // Test deserialization from data immediately
                {
                    try
                    {
                        ProjectInfo deserialProj = InfoSerializer.Deserialize(null, serialData) as ProjectInfo;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR] Immediately deserialize failed.");
                        Console.WriteLine(ex.Message);
                    }
                }

                // test formatter: XML
                {
                    try
                    {
                        XmlSerializer xmlSerial = new XmlSerializer(typeof(InfoSerializationData));
                        TextWriter writer = new StringWriter();
                        xmlSerial.Serialize(writer, serialData);
                        Console.WriteLine(writer.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR] Xml writer failed.");
                        Console.WriteLine(ex.Message);
                    }

                }

                // test formatter: Json
                {
                    try
                    {
                        string jsonStr = JsonSerializer.Serialize(serialData);
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
}
