using System;
using System.IO;
using System.Diagnostics;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using System.Runtime.InteropServices;
using nf.protoscript.Serialization;
using System.Xml.Serialization;
using System.Text.Json;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Xml;
using System.Text;

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
                    using (MemoryStream ms = new MemoryStream())
                    {

                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.Encoding = new UTF8Encoding(false);
                        settings.NewLineChars = Environment.NewLine;

                        using (XmlWriter xmlWriter = XmlWriter.Create(ms, settings))
                        {
                            SFDXmlSerializer.WriteSFDProperty(xmlWriter, "Project", gatheredProjData);
                        }

                        string xml = Encoding.UTF8.GetString(ms.ToArray());
                        Console.WriteLine(xml);
                    }
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
                        Converters =
                        {
                            new JsonTypeConverterFactory()
                        }
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

    public class JsonTypeConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(Type))
            {
                return true;
            }
            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new JsonTypeConverter();
        }
    }

    public class JsonTypeConverter : JsonConverter<Type>
    {
        public override Type Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return Type.GetType(reader.GetString()!);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Type InType,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(InType.Name);
        }
    }

}
