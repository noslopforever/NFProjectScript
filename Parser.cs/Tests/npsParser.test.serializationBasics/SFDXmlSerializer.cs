using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using nf.protoscript.Serialization;
using System;

namespace nf.protoscript.test
{

    public static class SFDXmlSerializer
    {
        static SFDXmlSerializer()
        {
            _AddTypeAlias("n", typeof(int));
            _AddTypeAlias("f", typeof(float));
            _AddTypeAlias("b", typeof(bool));
            _AddTypeAlias("s", typeof(string));
            _AddTypeAlias("c", typeof(char));
            _AddTypeAlias("SFD", typeof(SerializationFriendlyData));

            _AutoFillNS.Add("System");
            _AutoFillNS.Add("System.Collection");
            _AutoFillNS.Add("System.Collection.Generic");
            _AutoFillNS.Add("nf.protoscript");
            _AutoFillNS.Add("nf.protoscript.syntaxtree");
            _AutoFillNS.Add("nf.protoscript.Serialization");
        }

        public static void WriteSFDProperty(XmlWriter InWriter, string InName, object InDataObj)
        {
            SerializationFriendlyData propData = InDataObj as SerializationFriendlyData;

            // not data, take it as POD values.
            if (propData == null)
            {
                WritePODAttributeString(InWriter, InName, InDataObj);
            }
            // data, exact it.
            else
            {
                if (propData.IsPODData())
                {
                    object podVal = propData.AsPODData();
                    WritePODAttributeString(InWriter, InName, podVal);
                }
                else if (propData.IsInfoRef())
                {
                    InWriter.WriteAttributeString(InName, MakeInfoRefString(propData.AsInfoRefName()));
                }
                else if (propData.IsNull())
                {
                    InWriter.WriteAttributeString(InName, MakeNullString(propData.GetNullType()));
                }
                else if (propData.IsCollection())
                {
                    InWriter.WriteStartElement(InName);

                    InWriter.WriteAttributeString("CollectionType", MakeTypeString(propData.GetCollectionType()));

                    // Special codes for collection
                    var dataCol = propData.AsCollection();
                    for (int i = 0; i < dataCol.Count; i++)
                    {
                        var subData = dataCol[i];
                        WriteSFDProperty(InWriter, $"Item_{i}", subData);
                    }

                    InWriter.WriteEndElement();
                }
                else if (propData.IsDictionary())
                {
                    InWriter.WriteStartElement(InName);

                    InWriter.WriteAttributeString("DictionaryType", MakeTypeString(propData.GetDictionaryType()));
                    // Write subs.
                    var dict = propData.AsDictionary();
                    foreach (var kvp in dict)
                    {
                        InWriter.WriteStartElement("Item");

                        WriteSFDProperty(InWriter, "Key", kvp.Key);
                        WriteSFDProperty(InWriter, "Value", kvp.Value);

                        InWriter.WriteEndElement();
                    }

                    InWriter.WriteEndElement();
                }
                else
                {
                    InWriter.WriteStartElement(InName);

                    // Write sub properties.
                    foreach (var kvp in propData)
                    {
                        // Write subs.
                        WriteSFDProperty(InWriter, kvp.Key, kvp.Value);
                    }

                    InWriter.WriteEndElement();
                }
            }

        }

        //public static object ReadSFDProperty(XmlReader InReader, string InName)
        //{
        //    string attrValStr = InReader.GetAttribute(InName);
        //    if (attrValStr != null)
        //    {
        //        object valObj = ExactValueFromIdentifiedString(attrValStr);
        //        return valObj;
        //    }
        //    else
        //    {
        //        InReader.ReadStartElement(InName);
        //        for (int i = 0; i < InReader.AttributeCount; i++)
        //        {
        //            string InAttrName = InReader.GetAttribute(i);
        //        }
        //        InReader.ReadEndElement();
        //    }
        //}


        private static List<string> _AutoFillNS = new List<string>();

        private static Dictionary<string, Type> _AliasToTypeTable = new Dictionary<string, Type>();
        private static Dictionary<Type, string> _TypeToAliasTable = new Dictionary<Type, string>();
        static void _AddTypeAlias(string InName, Type InType)
        {
            _AliasToTypeTable[InName] = InType;
            _TypeToAliasTable[InType] = InName;
        }

        static string _GetTypeAlias(Type InType)
        {
            if (_TypeToAliasTable.ContainsKey(InType))
            {
                return _TypeToAliasTable[InType];
            }
            return null;
        }

        private static string MakeNullString(Type InType)
        {
            string typeStr = MakeTypeString(InType);
            return IdentifyValueString("$null", typeStr);
        }

        private static string MakeInfoRefString(string InInfoRefStr)
        {
            return IdentifyValueString("$nfo", InInfoRefStr);
        }

        private static string MakeValueString(object InDataObj)
        {
            Type objType = InDataObj.GetType();
            string typeName = MakeTypeString(objType);

            return IdentifyValueString(typeName, InDataObj);
        }

        // Typename will never contains special characters like: : % $ / \ | ( ) ...
        // So we take the ':' as the splitter of all values.
        private static string IdentifyValueString(string InTypeCode, object InDataObj)
        {
            return $"{InTypeCode}:{InDataObj.ToString()}";
        }
        private static void ParseIdentifiedString(string InString, out string OutTypecode, out string OutValueStr)
        {
            int splitterIdx = InString.IndexOf(':');
            if (splitterIdx == -1)
            {
                throw new InvalidCastException("Invalid Identify String");
            }
            OutTypecode = InString.Substring(0, splitterIdx);
            OutValueStr = InString.Substring(splitterIdx + 1);
        }

        private static object ExactValueFromIdentifiedString(string InString)
        {
            string typecode = "";
            string valStr = "";
            ParseIdentifiedString(InString, out typecode, out valStr);
            if (typecode == "$null")
            {
                Type type = ParseTypeFromString(valStr);
                return SerializationFriendlyData.NewNullData(type);
            }
            else if (typecode == "$nfo")
            {
                return SerializationFriendlyData.NewInfoRefName(valStr);
            }

            Type valType = ParseTypeFromString(typecode);
            object valInst = Convert.ChangeType(valStr, valType);
            return valInst;
        }

        private static string MakeTypeString(Type InType)
        {
            string typeName = InType.Name;

            string alias = _GetTypeAlias(InType);
            if (alias != null)
            {
                typeName = alias;
            }

            return typeName;
        }

        private static Type ParseTypeFromString(string InString)
        {
            return Type.GetType(InString);
        }

        private static void WritePODAttributeString(XmlWriter InWriter, string InName, object InDataObj)
        {
            if (InDataObj is Type)
            {
                string typeStr = MakeTypeString(InDataObj as Type);
                InWriter.WriteAttributeString(InName, typeStr);
            }
            else
            {
                InWriter.WriteAttributeString(InName, MakeValueString(InDataObj));
            }
        }



    }

}
