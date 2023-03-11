using nf.protoscript.Serialization;
using System;
using System.Collections.Generic;
using System.Xml;

namespace nf.protoscript.utils.serialization.xml
{

    /// <summary>
    /// Helper to make conversion between SFD and XML-strings.
    /// </summary>
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
            _AddTypeAlias("n[]", typeof(int[]));
            _AddTypeAlias("f[]", typeof(float[]));
            _AddTypeAlias("b[]", typeof(bool[]));
            _AddTypeAlias("s[]", typeof(string[]));
            _AddTypeAlias("c[]", typeof(char[]));
            _AddTypeAlias("SFD[]", typeof(SerializationFriendlyData[]));

            _AutoFillNS.Add("System.Collection.Generic");
            _AutoFillNS.Add("System.Collection");
            _AutoFillNS.Add("System");
            _AutoFillNS.Add("nf.protoscript.Serialization");
            _AutoFillNS.Add("nf.protoscript.syntaxtree");
            _AutoFillNS.Add("nf.protoscript");
        }

        /// <summary>
        /// Write SFD properties to a Xml.
        /// </summary>
        /// <param name="InWriter"></param>
        /// <param name="InName"></param>
        /// <param name="InDataObj"></param>
        public static void WriteSFDProperty(XmlWriter InWriter, string InName, object InDataObj)
        {
            SerializationFriendlyData propData = InDataObj as SerializationFriendlyData;

            // not data, take it as POD values.
            if (propData == null)
            {
                InWriter.WriteAttributeString(InName, _MakePureValueString(InDataObj));
            }
            // data, exact it.
            else
            {
                if (propData.IsPODData())
                {
                    object podVal = propData.AsPODData();
                    InWriter.WriteAttributeString(InName, _MakePureValueString(podVal));
                    // No need for a complex version: there are no Non-SFD PODValues saved in any level of SFDs for now.
                    //InWriter.WriteAttributeString(InName, _MakeValueString(podVal));
                }
                else if (propData.IsType())
                {
                    InWriter.WriteAttributeString(InName, MakeTypeString(propData.AsType()));
                }
                else if (propData.IsInfoRef())
                {
                    InWriter.WriteAttributeString(InName, _MakeInfoRefString(propData.AsInfoRefName()));
                }
                else if (propData.IsNull())
                {
                    InWriter.WriteAttributeString(InName, _MakeNullString(propData.GetNullType()));
                }
                else if (propData.IsCollection())
                {
                    InWriter.WriteStartElement(InName);

                    InWriter.WriteAttributeString("CollType", MakeTypeString(propData.GetCollectionType()));

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

                    InWriter.WriteAttributeString("DictType", MakeTypeString(propData.GetDictionaryType()));
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

        /// <summary>
        /// Read XmlNode as a SFD
        /// </summary>
        /// <param name="InNode"></param>
        /// <returns></returns>
        public static SerializationFriendlyData ReadSFDNode(XmlNode InNode)
        {
            SerializationFriendlyData sfd = SerializationFriendlyData.NewEmpty();
            foreach (XmlAttribute xmlAttr in InNode.Attributes)
            {
                string attrKey = xmlAttr.LocalName;
                string attrValue = xmlAttr.Value;

                var exactData = _ExactValueFromIdentifiedString(attrValue);
                sfd[attrKey] = exactData;
            }

            if (sfd.HasMember("CollType"))
            {
                // try read subs.
                List<SerializationFriendlyData> list = new List<SerializationFriendlyData>();
                _ReadSFDCollections(list, InNode);
                return SerializationFriendlyData.NewCollection(sfd.GetCollectionType(), list);
            }
            if (sfd.HasMember("DictType"))
            {
                throw new NotImplementedException();
            }
            if (sfd.IsObject())
            {
                foreach (XmlNode subNode in InNode.ChildNodes)
                {
                    _ReadSFDProperties(sfd, subNode);
                }
            }
            return sfd;
        }

        private static SerializationFriendlyData _ReadSFDProperties(SerializationFriendlyData InSFD, XmlNode InNode)
        {
            string key = InNode.LocalName;
            var sfd = ReadSFDNode(InNode);
            InSFD[key] = sfd;
            return sfd;
        }

        private static void _ReadSFDCollections(List<SerializationFriendlyData> RefList, XmlNode InNode)
        {
            foreach (XmlNode itemNode in InNode.ChildNodes)
            {
                var sfdItem = ReadSFDNode(itemNode);
                RefList.Add(sfdItem);
            }
        }

        /// <summary>
        /// Auto-fill namespaces. These namespaces should be used to ful-fill short type names automatically.
        /// e.g. We got System.Collection.Array after ful-filling the 'Array' with 'System.Collection'.
        /// </summary>
        private static List<string> _AutoFillNS = new List<string>();

        /// <summary>
        /// Find Type by its alias (short name)
        /// </summary>
        private static Dictionary<string, Type> _AliasToTypeTable = new Dictionary<string, Type>();

        /// <summary>
        /// Find alias (short name) by Type.
        /// </summary>
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
        static Type _GetTypeByAlias(string InAlias)
        {
            if (_AliasToTypeTable.ContainsKey(InAlias))
            {
                return _AliasToTypeTable[InAlias];
            }
            return null;
        }

        private static string _MakeNullString(Type InType)
        {
            string typeStr = _ConvertTypeToString(InType);
            return IdentifyValueString("$N", typeStr);
        }

        private static string _MakeInfoRefString(string InInfoRefStr)
        {
            return IdentifyValueString("$R", InInfoRefStr);
        }

        private static string _MakePureValueString(object InDataObj)
        {
            Type objType = InDataObj.GetType();
            string typeName = _ConvertTypeToString(objType);

            return IdentifyValueString(typeName, InDataObj);
        }

        private static string MakeTypeString(Type InType)
        {
            string typename = _ConvertTypeToString(InType);
            return IdentifyValueString("$T", typename);
        }

        private static string _MakeValueString(object InDataObj)
        {
            Type objType = InDataObj.GetType();
            string typeName = _ConvertTypeToString(objType);

            return IdentifyValueString("$P" + typeName, InDataObj);
        }

        // Typename will never contains special characters like: : % $ / \ | ( ) ...
        // So we take the ':' as the splitter of all values.
        private static string IdentifyValueString(string InTypeCode, object InDataObj)
        {
            return $"{InTypeCode}:{InDataObj.ToString()}";
        }

        private static object _ExactValueFromIdentifiedString(string InString)
        {
            string typecode = "";
            string valStr = "";
            _SplitIdentifiedString(InString, out typecode, out valStr);
            if (typecode == "$N")
            {
                Type type = _ParseTypeFromString(valStr);
                return SerializationFriendlyData.NewNullData(type);
            }
            else if (typecode == "$R")
            {
                return SerializationFriendlyData.NewInfoRefName(valStr);
            }
            else if (typecode == "$T")
            {
                Type type = _ParseTypeFromString(valStr);
                return SerializationFriendlyData.NewTypeData(type);
            }
            else if (typecode.StartsWith("$P"))
            {
                string purePODTypeCode = typecode.Substring(2);
                object purePODVal = _ExactPurePODValue(purePODTypeCode, valStr);
                return SerializationFriendlyData.NewPODData(purePODVal.GetType(), purePODVal);
            }

            return _ExactPurePODValue(typecode, valStr);
        }

        private static void _SplitIdentifiedString(string InString, out string OutTypecode, out string OutValueStr)
        {
            int splitterIdx = InString.IndexOf(':');
            if (splitterIdx == -1)
            {
                throw new InvalidCastException("Invalid Identify String");
            }
            OutTypecode = InString.Substring(0, splitterIdx);
            OutValueStr = InString.Substring(splitterIdx + 1);
        }

        private static object _ExactPurePODValue(string InTypeCode, string InValueString)
        {
            // Pure POD value
            Type valType = _ParseTypeFromString(InTypeCode);
            object valInst = Convert.ChangeType(InValueString, valType);
            return valInst;
        }

        /// <summary>
        /// Convert Type to string.
        /// </summary>
        /// <param name="InType"></param>
        /// <returns></returns>
        private static string _ConvertTypeToString(Type InType)
        {
            string typeName = InType.FullName;

            // Get alias from the Type.
            string alias = _GetTypeAlias(InType);
            if (alias != null)
            {
                typeName = alias;
            }

            // Remove auto-fill-NSs from string
            foreach (var ns in _AutoFillNS)
            {
                if (typeName.StartsWith(ns)) {
                    typeName = typeName.Substring(ns.Length + 1);
                }
            }

            return typeName;
        }

        /// <summary>
        /// Read string as a Type.
        /// </summary>
        /// <param name="InString"></param>
        /// <returns></returns>
        private static Type _ParseTypeFromString(string InString)
        {
            Type type = Type.GetType(InString);
            if (type != null)
            { return type; }

            type = _GetTypeByAlias(InString);
            if (type != null)
            { return type; }

            // Try auto-fill-NSs
            foreach (var ns in _AutoFillNS)
            {
                string fullName = ns + "." + InString;
                type = Type.GetType(InString);
                if (type != null)
                { return type; }
            }

            return null;
        }

    }
}
