using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// Serialization helper functions.
    /// </summary>
    public static class SerializationHelper
    {

        /// <summary>
        /// Restore values from serialization-friendly data.
        /// 
        /// If restore failed, throw exceptions.
        /// </summary>
        /// <param name="InData"></param>
        /// <returns></returns>
        public static object RestoreValueFromData(Type InTargetType, SerializationFriendlyData InData)
        {
            // TODO 202302051343 use manager to select the approxiate converter dynamically.

            // InfoRefData: always be read as a ****Info
            if (InData.IsInfoRef())
            {
                return _RestoreAsInfoRef(InData);
            }

            // InfoData: always be read as a ****Info instance. But should be handled in InfoGatherer, not here.
            if (InData.IsObjectOf(typeof(Info)))
            {
                throw new InvalidProgramException();
            }

            // SyntaxData : always be read as a STNode****
            if (InData.IsObjectOf(typeof(STNodeBase)))
            {
                return _RestoreAsSyntax(InData);
            }

            // POD Data
            if (InData.IsPODData())
            {
                return InData.AsPODData();
            }

            // Collection Data
            if (InData.IsCollection())
            {
                IReadOnlyList<SerializationFriendlyData> srcCollData = InData.AsCollection();

                // Handle Array type (object[])
                if (InData.SourceValueType.IsArray)
                {
                    // Create array instance, pass in the size.
                    object targetObj = Activator.CreateInstance(InData.SourceValueType, new object[] { srcCollData.Count });
                    Array targetArray = targetObj as Array;

                    // then fill values.
                    Type elemType = InData.SourceValueType.GetElementType();
                    for (int i = 0; i < srcCollData.Count; i++)
                    {
                        object targetElemVal = RestoreValueFromData(elemType, srcCollData[i]);
                        targetArray.SetValue(targetElemVal, i);
                    }

                    return targetArray;

                    // TODO support xD array (object[,] | object[,,] | object[][])
                }
                else
                {
                    // TODO support collection restore.
                    throw new NotImplementedException();
                }
            }

            if (InData.IsCollection())
            {
                throw new NotImplementedException();
            }

            // Null Data
            if (InData.IsNull())
            {
                return null;
            }

            throw new InvalidCastException();
        }

        /// <summary>
        /// Convert values to serialization-friendly data.
        /// </summary>
        /// <param name="InValueDeclType">InValue should be null, in this case, use this type to determine how to convert the value.</param>
        /// <param name="InValue"></param>
        /// <returns></returns>
        public static SerializationFriendlyData ConvertValueToData(Type InValueDeclType, object InValue)
        {
            // TODO 202302051343 use manager to select the approxiate converter dynamically.

            if (InValue is SerializationFriendlyData)
            {
                return InValue as SerializationFriendlyData;
            }

            // null value, check InValueDeclType
            if (InValue == null)
            {
                var data = SerializationFriendlyData.NewNullData(InValueDeclType);
                return data;
            }

            // ****Info: Always convert to InfoRefData
            if (InValue is Info)
            {
                var data = _ConvertToInfoRef(InValue as Info);
                return data;
            }

            // STNode****: Always convert to SyntaxData
            if (InValue is STNodeBase)
            {
                var data = _ConvertToSyntax(InValue as STNodeBase);
                return data;
            }

            // Array: write it to collection
            if (InValue.GetType().IsArray)
            {
                Type elemType = InValue.GetType().GetElementType();
                Array arrayVal = InValue as Array;

                var convertedCollection = new List<SerializationFriendlyData>() { Capacity = arrayVal.Length };
                for (int i = 0; i < arrayVal.Length; ++i)
                {
                    var elemData = ConvertValueToData(elemType, arrayVal.GetValue(i));
                    convertedCollection.Add(elemData);
                }
                var data = SerializationFriendlyData.NewCollection(arrayVal.GetType(), convertedCollection);
                return data;
            }

            // TODO support ICollections like List<>
            // TODO support IDictionary like Dictionary<,>

            // POD types: Always convert to PODData
            if (InValue is string
                || InValue.GetType().IsPrimitive
                || InValue.GetType().IsEnum
                || InValue.GetType().IsValueType
                )
            {
                var data = SerializationFriendlyData.NewPODData(InValue.GetType(), InValue);
                return data;
            }

            throw new InvalidCastException();
        }


        /// <summary>
        /// Convert a Info to a InfoRefData
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        private static SerializationFriendlyData _ConvertToInfoRef(Info InInfo)
        {
            string infoFullname = InfoHelper.GetFullnameOfInfo(InInfo);
            return SerializationFriendlyData.NewInfoRefName(InInfo.GetType(), infoFullname);
        }

        /// <summary>
        /// Restore a Info from a InfoRefData.
        /// </summary>
        /// <param name="InTypeRefData"></param>
        /// <returns></returns>
        private static Info _RestoreAsInfoRef(SerializationFriendlyData InTypeRefData)
        {
            Info foundInfo = InfoHelper.FindInfoByFullname(InTypeRefData.AsInfoRefName());
            return foundInfo;
        }

        /// <summary>
        /// Convert SyntaxTree into SyntaxData.
        /// </summary>
        /// <param name="InSTNode"></param>
        /// <returns></returns>
        private static SerializationFriendlyData _ConvertToSyntax(STNodeBase InSTNode)
        {
            // TODO Support ISyntaxTreeNode provided by 3rd-library.

            var nodeType = InSTNode.GetType();
            var targetData = SerializationFriendlyData.NewObject(nodeType);

            // Write property values.
            var props = _FindPropertiesHandledByGatherer(nodeType);
            foreach (var prop in props)
            {
                // Get values in 
                object val = prop.GetValue(InSTNode);
                var data = ConvertValueToData(prop.PropertyType, val);

                targetData.Add(prop.Name, data);
            }

            return targetData;
        }

        /// <summary>
        /// Restore SyntaxTree from SyntaxData.
        /// </summary>
        /// <param name="InSTNodeData"></param>
        /// <returns></returns>
        private static syntaxtree.ISyntaxTreeNode _RestoreAsSyntax(SerializationFriendlyData InSTNodeData)
        {
            System.Diagnostics.Debug.Assert(InSTNodeData.SourceValueType.IsSubclassOf(typeof(STNodeBase)));

            // Restore node instance by SyntaxData.Class.
            Type nodeType = InSTNodeData.SourceValueType;

            //STNodeBase targetNode = Activator.CreateInstance(nodeType) as STNodeBase;
            STNodeBase targetNode = Activator.CreateInstance(
                // type
                nodeType
                // binding flags
                , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                // binder
                , null
                // parameters
                , new object[0]
                // culture info
                , null
                ) as STNodeBase;

            // Read properties.
            var props = _FindPropertiesHandledByGatherer(nodeType);
            foreach (var prop in props)
            {
                // Read data from SyntaxData
                SerializationFriendlyData data = null;
                if (!InSTNodeData.TryGetExtra(prop.Name, out data))
                { continue; }

                // If property is a STNode
                object val = RestoreValueFromData(prop.PropertyType, data);
                prop.SetValue(targetNode, val);
            }

            return targetNode;
        }

        //public static MethodDelegateTypeData Convert(DelegateTypeInfo InDelegateInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public static DelegateTypeInfo Restore()
        //{
        //    throw new NotImplementedException();
        //}


        /// <summary>
        /// Select properties handled by Gatherer.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<PropertyInfo> _FindPropertiesHandledByGatherer(Type InType)
        {
            List<PropertyInfo> gatherProps = new List<PropertyInfo>();
            var props = InType.GetProperties();
            foreach (var prop in props)
            {
                if (_IsPropertyHandledByGatherer(prop))
                {
                    gatherProps.Add(prop);
                }
            }
            return gatherProps;
        }


        /// <summary>
        /// Check if the InProperty needs to be gathered.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal static bool _IsPropertyHandledByGatherer(PropertyInfo prop)
        {
            //// Exclude system and base-Info properties.
            //if (prop.ReflectedType == typeof(Info)
            //    || prop.ReflectedType == typeof(object)
            //    )
            //{
            //    return false;
            //}

            // If the property is marked as Serializable, gather it.
            if (prop.GetCustomAttribute<SerializableInfoAttribute>() != null)
            {
                return true;
            }

            return false;
        }

    }


}
