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
using System.Xml;
using System.Xml.Schema;
using System.ComponentModel;
using System.Text.Json.Serialization;

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

    [Serializable]
    public class SerialDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>
        , IXmlSerializable
    {
        public void WriteXml(XmlWriter InWriter)       // Serializer
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            InWriter.WriteStartElement("Dictionary");
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                InWriter.WriteStartElement("element");
                InWriter.WriteStartElement("key");
                KeySerializer.Serialize(InWriter, kv.Key);
                InWriter.WriteEndElement();
                InWriter.WriteStartElement("value");
                ValueSerializer.Serialize(InWriter, kv.Value);
                InWriter.WriteEndElement();
                InWriter.WriteEndElement();
            }
            InWriter.WriteEndElement();
        }
        public void ReadXml(XmlReader InReader)       // Deserializer
        {
            InReader.Read();
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            InReader.ReadStartElement();
            while (InReader.NodeType != XmlNodeType.EndElement)
            {
                InReader.ReadStartElement("element");
                InReader.ReadStartElement("key");
                TKey tk = (TKey)keySerializer.Deserialize(InReader);
                InReader.ReadEndElement();
                InReader.ReadStartElement("value");
                TValue vl = (TValue)ValueSerializer.Deserialize(InReader);
                InReader.ReadEndElement();
                InReader.ReadEndElement();

                this.Add(tk, vl);
                InReader.MoveToContent();
            }
            InReader.ReadEndElement();
            InReader.ReadEndElement();

        }
        public XmlSchema GetSchema()
        {
            return null;
        }

    }

}
