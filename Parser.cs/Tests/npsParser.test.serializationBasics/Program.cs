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

                // Test reader
                using (var txtReader = new StringReader(xmlResult))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(txtReader);

                    object returnVal = SFDXmlSerializer.ReadSFDNode(xmlDoc.LastChild);

                    try
                    {
                        ProjectInfo restoredProj = InfoGatherer.Restore(null, gatheredProjData) as ProjectInfo;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR] Xml deserialize failed.");
                        Console.WriteLine(ex.Message);
                    }
                }

            }

            //// test serializer: Json
            //{

            //    try
            //    {
            //        var serializeOption = new JsonSerializerOptions()
            //        {
            //            AllowTrailingCommas = false,
            //            WriteIndented = true,
            //            Converters =
            //            {
            //                new JsonTypeConverterFactory()
            //            }
            //        };
            //        string jsonStr = JsonSerializer.Serialize(gatheredProjData, serializeOption);
            //        Console.WriteLine(jsonStr);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("[ERROR] Json writer failed.");
            //        Console.WriteLine(ex.Message);
            //    }
            //}

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


/* Xml Result

<?xml version="1.0" encoding="utf-8"?>
<Project ObjectType="ProjectInfo" SFD_IsObject="b:True" Header="s:" Name="s:TestProj">
  <SubInfos CollectionType="SerializationFriendlyData[]">
    <Item_0 ObjectType="TypeInfo" SFD_IsObject="b:True" Header="s:model" Name="s:classA">
      <SubInfos CollectionType="SerializationFriendlyData[]">
        <Item_0 ObjectType="MemberInfo" SFD_IsObject="b:True" Header="s:property" Name="s:propA" Archetype="$nfo:__sys__/integer">
          <InitSyntax ObjectType="STNodeConstant" SFD_IsObject="b:True" ValueTypeString="s:integer" ValueString="s:100" />
          <SubInfos CollectionType="SerializationFriendlyData[]">
            <Item_0 ObjectType="AttributeInfo" SFD_IsObject="b:True" Header="s:Property" Name="s:Anonymous_Property_Attribute" InitSyntaxTree="$null:ISyntaxTreeNode" />
          </SubInfos>
        </Item_0>
        <Item_1 ObjectType="MemberInfo" SFD_IsObject="b:True" Header="s:property" Name="s:propB" Archetype="$nfo:__sys__/integer">
          <InitSyntax ObjectType="STNodeBinaryOp" SFD_IsObject="b:True" OpCode="s:add">
            <LHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propA" />
            <RHS ObjectType="STNodeConstant" SFD_IsObject="b:True" ValueTypeString="s:integer" ValueString="s:100" />
          </InitSyntax>
        </Item_1>
        <Item_2 ObjectType="MethodInfo" SFD_IsObject="b:True" Header="s:method" Name="s:TestMethodA" MethodSignature="$nfo:TestProj/func_I_I_Type">
          <ExecSequence ObjectType="STNodeSequence" SFD_IsObject="b:True">
            <NodeList CollectionType="ISyntaxTreeNode[]">
              <Item_0 ObjectType="STNodeAssign" SFD_IsObject="b:True">
                <LHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propA" />
                <RHS ObjectType="STNodeBinaryOp" SFD_IsObject="b:True" OpCode="s:add">
                  <LHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propB" />
                  <RHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:InParam" />
                </RHS>
              </Item_0>
              <Item_1 ObjectType="STNodeAssign" SFD_IsObject="b:True">
                <LHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:___return___" />
                <RHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propA" />
              </Item_1>
            </NodeList>
          </ExecSequence>
        </Item_2>
        <Item_3 ObjectType="MethodInfo" SFD_IsObject="b:True" Header="s:method" Name="s:TestMethodB" MethodSignature="$nfo:TestProj/funcV_IR_Type">
          <ExecSequence ObjectType="STNodeSequence" SFD_IsObject="b:True">
            <NodeList CollectionType="ISyntaxTreeNode[]">
              <Item_0 ObjectType="STNodeAssign" SFD_IsObject="b:True">
                <LHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propA" />
                <RHS ObjectType="STNodeBinaryOp" SFD_IsObject="b:True" OpCode="s:add">
                  <LHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propA" />
                  <RHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:RefParam" />
                </RHS>
              </Item_0>
              <Item_1 ObjectType="STNodeAssign" SFD_IsObject="b:True">
                <LHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:RefParam" />
                <RHS ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propA" />
              </Item_1>
            </NodeList>
          </ExecSequence>
        </Item_3>
        <Item_4 ObjectType="MethodInfo" SFD_IsObject="b:True" Header="s:method" Name="s:TestMethodC" MethodSignature="$nfo:TestProj/func_V_V_Type">
          <ExecSequence ObjectType="STNodeSequence" SFD_IsObject="b:True">
            <NodeList CollectionType="ISyntaxTreeNode[]">
              <Item_0 ObjectType="STNodeCall" SFD_IsObject="b:True" FuncName="s:TestMethodB">
                <Params CollectionType="ISyntaxTreeNode[]">
                  <Item_0 ObjectType="STNodeGetVar" SFD_IsObject="b:True" IDName="s:propA" />
                </Params>
              </Item_0>
            </NodeList>
          </ExecSequence>
        </Item_4>
      </SubInfos>
    </Item_0>
    <Item_1 ObjectType="DelegateTypeInfo" SFD_IsObject="b:True" Header="s:FuncType" Name="s:func_I_I_Type">
      <SubInfos CollectionType="SerializationFriendlyData[]">
        <Item_0 ObjectType="MemberInfo" SFD_IsObject="b:True" Header="s:param" Name="s:___return___" Archetype="$nfo:__sys__/integer" InitSyntax="$null:ISyntaxTreeNode">
          <SubInfos CollectionType="SerializationFriendlyData[]">
            <Item_0 ObjectType="AttributeInfo" SFD_IsObject="b:True" Header="s:Return" Name="s:__Anonymous_Return_Property__" InitSyntaxTree="$null:ISyntaxTreeNode" />
          </SubInfos>
        </Item_0>
        <Item_1 ObjectType="MemberInfo" SFD_IsObject="b:True" Header="s:param" Name="s:InParam" Archetype="$nfo:__sys__/integer" InitSyntax="$null:ISyntaxTreeNode" />
        <Item_2 ObjectType="MemberInfo" SFD_IsObject="b:True" Header="s:param" Name="s:RefParam" Archetype="$nfo:__sys__/integer" InitSyntax="$null:ISyntaxTreeNode" />
      </SubInfos>
    </Item_1>
    <Item_2 ObjectType="DelegateTypeInfo" SFD_IsObject="b:True" Header="s:FuncType" Name="s:funcV_IR_Type" />
    <Item_3 ObjectType="DelegateTypeInfo" SFD_IsObject="b:True" Header="s:FuncType" Name="s:func_V_V_Type" />
  </SubInfos>
</Project>
 
 */