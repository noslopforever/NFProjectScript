using nf.protoscript.syntaxtree;
using System;
using System.Dynamic;
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
        public static object RestoreValueFromData(Type InTargetType, ISerializationFriendlyData InData)
        {
            // TODO 202302051343 use manager to select the approxiate converter dynamically.

            // InfoRefData: always be read as a ****Info
            if (InData is InfoRefData)
            {
                return _Restore(InData as InfoRefData);
            }
            // InfoData: always be read as a ****Info instance. But should be handled in InfoGatherer, not here.
            if (InData is InfoData)
            {
                throw new InvalidProgramException();
            }

            // SyntaxData : always be read as a STNode****
            if (InData is SyntaxData)
            {
                return _Restore(InData as SyntaxData);
            }

            // POD Data
            if (InData is PODData)
            {
                return (InData as PODData).Value;
            }

            // Collection Data
            if (InData is CollectionData)
            {
                var srcCollData = InData as CollectionData;

                // Handle Array type (object[])
                if (srcCollData.CollectionType.IsArray)
                {
                    // Create array instance, pass in the size.
                    object targetObj = Activator.CreateInstance(srcCollData.CollectionType, new object[] { srcCollData.Count });
                    Array targetArray = targetObj as Array;

                    // then fill values.
                    Type elemType = srcCollData.CollectionType.GetElementType();
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

            if (InData is DictionaryData)
            {
                throw new NotImplementedException();
            }

            // Null Data
            if (InData is NullData)
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
        public static ISerializationFriendlyData ConvertValueToData(Type InValueDeclType, object InValue)
        {
            // TODO 202302051343 use manager to select the approxiate converter dynamically.

            // null value, check InValueDeclType
            if (InValue == null)
            {
                var data = new NullData(InValueDeclType);
                return data;
            }
            else
            {
                // ****Info: Always convert to InfoRefData
                if (InValue is Info)
                {
                    var data = _Convert(InValue as Info);
                    return data;
                }

                // STNode****: Always convert to SyntaxData
                if (InValue is STNodeBase)
                {
                    var data = _Convert(InValue as STNodeBase);
                    return data;
                }

                // Array: write it to collection
                if (InValue.GetType().IsArray)
                {
                    Type elemType = InValue.GetType().GetElementType();
                    Array arrayVal = InValue as Array;

                    CollectionData targetCollData = new CollectionData()
                    {
                        CollectionType = InValue.GetType(),
                        Capacity = arrayVal.Length
                    };
                    for (int i = 0; i < arrayVal.Length; ++i)
                    {
                        var elemData = ConvertValueToData(elemType, arrayVal.GetValue(i));
                        targetCollData.Add(elemData);
                    }
                    return targetCollData;
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
                    var data = new PODData(InValue);
                    return data;
                }

            }

            throw new InvalidCastException();
        }


        /// <summary>
        /// Convert a Info to a InfoRefData
        /// </summary>
        /// <param name="InTypeInfo"></param>
        /// <returns></returns>
        private static InfoRefData _Convert(Info InTypeInfo)
        {
            string infoFullname = InfoHelper.GetFullnameOfInfo(InTypeInfo);
            return new InfoRefData(infoFullname);
        }

        /// <summary>
        /// Restore a Info from a InfoRefData.
        /// </summary>
        /// <param name="InTypeRefData"></param>
        /// <returns></returns>
        private static Info _Restore(InfoRefData InTypeRefData)
        {
            Info foundInfo = InfoHelper.FindInfoByFullname(InTypeRefData.Fullname);
            return foundInfo;
        }

        /// <summary>
        /// Convert SyntaxTree into SyntaxData.
        /// </summary>
        /// <param name="InSyntaxTree"></param>
        /// <returns></returns>
        private static SyntaxData _Convert(ISyntaxTreeNode InSyntaxTree)
        {
            // TODO Support ISyntaxTreeNode provided by 3rd-library.

            SyntaxData targetData = new SyntaxData();

            var srcNode = InSyntaxTree as STNodeBase;
            var treeType = srcNode.GetType();
            targetData.Typename = treeType.FullName;
            var nodeType = srcNode.GetType();

            // Write property values.
            var props = _FindPropertiesHandledByGatherer(nodeType);
            foreach (var prop in props)
            {
                // Get values in 
                object val = prop.GetValue(srcNode);
                var data = ConvertValueToData(prop.PropertyType, val);

                targetData.TryAdd(prop.Name, data);
            }

            return targetData;
        }

        /// <summary>
        /// Restore SyntaxTree from SyntaxData.
        /// </summary>
        /// <param name="InSyntaxData"></param>
        /// <returns></returns>
        private static syntaxtree.ISyntaxTreeNode _Restore(SyntaxData InSyntaxData)
        {
            // Restore node instance by SyntaxData.Class.
            Type nodeType = Type.GetType(InSyntaxData.Typename);
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
                ISerializationFriendlyData data = null;
                if (!InSyntaxData.TryGetExtraData(prop.Name, out data))
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
