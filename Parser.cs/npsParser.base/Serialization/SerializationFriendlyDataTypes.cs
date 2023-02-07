using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// Serialization friendly data
    /// </summary>
    public sealed class SerializationFriendlyData
        : Dictionary<string, object>
    {
        private SerializationFriendlyData()
        {
        }

        /// <summary>
        /// Check if we have a propety names InName in this dynamic object.
        /// </summary>
        /// <param name="InName"></param>
        /// <returns></returns>
        public bool HasMember(string InName)
        {
            return ContainsKey(InName);
        }

        /// <summary>
        /// Try add some extra
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InData"></param>
        /// <returns></returns>
        public bool TryAddExtra(string InPropName, SerializationFriendlyData InData)
        {
            return TryAdd(InPropName, InData);
        }

        ///// <summary>
        ///// Add some extra.
        ///// 
        ///// If name-conflict, throw Key
        ///// </summary>
        ///// <param name="InName"></param>
        ///// <param name="InData"></param>
        ///// <returns></returns>
        //public void Add(string InPropName, SerializationFriendlyData InData)
        //{
        //    if (_Dictionary.ContainsKey(InPropName))
        //    {
        //        throw new ArgumentException();
        //    }
        //    _Dictionary[InPropName] = InData;
        //}

        /// <summary>
        /// Try get extra data by name.
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="OutData"></param>
        /// <returns></returns>
        public bool TryGetExtra(string InPropName, out SerializationFriendlyData OutData)
        {
            OutData = null;
            object savedVal = null;
            if (!TryGetValue(InPropName, out savedVal))
            {
                return false;
            }

            if (! (savedVal is SerializationFriendlyData))
            {
                return false;
            }

            OutData = savedVal as SerializationFriendlyData;
            return true;
        }

        /// <summary>
        /// Type of the source value that has been converted to this data.
        /// </summary>
        public Type SourceValueType { get; set; }

        #region "Data Accessors"

        public bool IsNull()
        {
            return HasMember("IsNullValue");
        }

        public static SerializationFriendlyData NewNullData(Type InDeclValueType)
        {
            var dynData = new SerializationFriendlyData() { SourceValueType = InDeclValueType };
            dynData["IsNullValue"] = true;
            return dynData;
        }

        public bool IsPODData()
        {
            return HasMember("PODValue");
        }

        public object AsPODData()
        {
            return this["PODValue"];
        }

        public static SerializationFriendlyData NewPODData(Type InDeclValueType, object InValue)
        {
            if (InValue == null)
            {
                return NewNullData(InDeclValueType);
            }

            var dynData = new SerializationFriendlyData() { SourceValueType = InValue.GetType() };
            dynData["PODValue"] = InValue;
            return dynData;
        }

        public bool IsInfoRef()
        {
            return HasMember("InfoRefFullName");
        }

        public string AsInfoRefName()
        {
            return this["InfoRefFullName"] as string;
        }

        public static SerializationFriendlyData NewInfoRefName(Type InDeclValueType, string InInfoFullName)
        {
            var dynData = new SerializationFriendlyData() { SourceValueType = InDeclValueType };
            dynData["InfoRefFullName"] = InInfoFullName;
            return dynData;
        }

        public bool IsCollection()
        {
            return HasMember("Collection");
        }

        public IReadOnlyList<SerializationFriendlyData> AsCollection()
        {
            var coll = this["Collection"] as List<SerializationFriendlyData>;
            return coll;
        }

        public static SerializationFriendlyData NewCollection(Type InDeclCollectionType, List<SerializationFriendlyData> InCollection)
        {
            var dynData = new SerializationFriendlyData() { SourceValueType = InDeclCollectionType };
            dynData["Collection"] = InCollection;
            return dynData;
        }

        public bool IsDictionary()
        {
            return HasMember("SFD_Dictionary");
        }

        public IReadOnlyDictionary<SerializationFriendlyData, SerializationFriendlyData> AsDictionary()
        {
            var dict = this["SFD_Dictionary"] as Dictionary<SerializationFriendlyData, SerializationFriendlyData>;
            return dict;
        }

        public static SerializationFriendlyData NewDictionary(Type InDeclDictType, Dictionary<SerializationFriendlyData, SerializationFriendlyData> InDictionary)
        {
            var dynData = new SerializationFriendlyData() { SourceValueType = InDeclDictType };
            dynData["SFD_Dictionary"] = InDictionary;
            return dynData;
        }


        /// <summary>
        /// Data is object, all members in the data will be treated as properties of the target object.
        /// </summary>
        /// <returns></returns>
        public bool IsObject()
        {
            return HasMember("SFD_IsObject");
        }

        /// <summary>
        /// Check if the InType is assignable from this object data.
        /// </summary>
        /// <param name="InType"></param>
        /// <returns></returns>
        public bool IsObjectOf(Type InType)
        {
            return InType.IsAssignableFrom(SourceValueType);
        }

        public object AsObject()
        {
            // Take this dictionary as an object.
            return this;
        }

        public static SerializationFriendlyData NewObject(Type InDeclObjectType)
        {
            var dynData = new SerializationFriendlyData() { SourceValueType = InDeclObjectType };
            dynData["SFD_IsObject"] = true;
            return dynData;
        }

        #endregion

    }


    ///// <summary>
    ///// POD (int, byte, string, structure ...) data.
    ///// </summary>
    //public sealed class PODData
    //    : ISerializationFriendlyData
    //{
    //    internal PODData()
    //    {
    //    }
    //    public PODData(object InValue)
    //    {
    //        Value = InValue;
    //    }

    //    /// <summary>
    //    /// The POD value.
    //    /// </summary>
    //    public object Value { get; set; }

    //}

    ///// <summary>
    ///// CollectionData that holds a collection
    ///// </summary>
    //public sealed class CollectionData
    //    : List<ISerializationFriendlyData>
    //    , ISerializationFriendlyData
    //{

    //    /// <summary>
    //    /// Type of the collection.
    //    /// </summary>
    //    public Type CollectionType { get; set; }

    //}

    ///// <summary>
    ///// DictionaryData which holds a dictionary.
    ///// </summary>
    //public sealed class DictionaryData
    //    : Dictionary<ISerializationFriendlyData, ISerializationFriendlyData>
    //    , ISerializationFriendlyData
    //{

    //    /// <summary>
    //    /// Type of the dictionary.
    //    /// </summary>
    //    public Type DictionaryType { get; set; }

    //}

    ///// <summary>
    ///// Null data but saves Type of the value.
    ///// </summary>
    //public sealed class NullData
    //    : ISerializationFriendlyData
    //{
    //    internal NullData()
    //    {
    //    }
    //    public NullData(Type InType)
    //    {
    //        Type = InType;
    //    }

    //    /// <summary>
    //    /// Type of the value
    //    /// </summary>
    //    public Type Type { get; set; }

    //}

    ///// <summary>
    ///// Data to save info references.
    ///// </summary>
    //public sealed class InfoRefData
    //    : ISerializationFriendlyData
    //{
    //    internal InfoRefData()
    //    {
    //    }
    //    public InfoRefData(string InTypeFullname)
    //    {
    //        Fullname = InTypeFullname;
    //    }

    //    /// <summary>
    //    /// Path of the info: 
    //    /// -   Package.Type.SubInfo.SubInfo
    //    /// </summary>
    //    public string Fullname { get; set; }

    //}


    ////public sealed class MethodDelegateTypeData
    ////    : SerializationFriendlyDataBase
    ////{
    ////    // TODO ...
    ////}


    ///// <summary>
    ///// Dynamic serialization friendly data.
    ///// </summary>
    //public abstract class DynamicSerializationFriendlyData
    //    : ISerializationFriendlyData
    //{
    //    /// <summary>
    //    /// Class of the source object.
    //    /// </summary>
    //    public string Typename { get; set; }

    //    /// <summary>
    //    /// Append datas of info: Member's InitExpr, Method's Codebody, etc...
    //    /// </summary>
    //    private ExpandoObject Extra { get; set; } = new ExpandoObject();

    //    /// <summary>
    //    /// Try get extra data by name.
    //    /// </summary>
    //    /// <param name="InName"></param>
    //    /// <param name="OutData"></param>
    //    /// <returns></returns>
    //    public bool TryGetExtraData(string InName, out ISerializationFriendlyData OutData)
    //    {
    //        OutData = null;

    //        IDictionary<string, object> dict = Extra as IDictionary<string, object>;
    //        object dataObj = null;
    //        if (!dict.TryGetValue(InName, out dataObj))
    //        {
    //            return false;
    //        }

    //        // The dataObj must implement ISerializationFriendlyDataBase
    //        if (!typeof(ISerializationFriendlyData).IsAssignableFrom(dataObj.GetType()))
    //        {
    //            throw new InvalidCastException();
    //        }

    //        OutData = dataObj as ISerializationFriendlyData;
    //        return true;
    //    }

    //    /// <summary>
    //    /// Try add some extra
    //    /// </summary>
    //    /// <param name="InName"></param>
    //    /// <param name="InData"></param>
    //    /// <returns></returns>
    //    public bool TryAdd(string InName, ISerializationFriendlyData InData)
    //    {
    //        return Extra.TryAdd(InName, InData);
    //    }

    //}

    ///// <summary>
    ///// Hold all Info's necessary datas which can be used to reconstruct the Info itself in the future.
    ///// InfoData is Serializer-friendly.
    ///// </summary>
    //public sealed class InfoData
    //    : DynamicSerializationFriendlyData
    //{
    //    public InfoData()
    //    {
    //    }

    //    /// <summary>
    //    /// Info's header
    //    /// </summary>
    //    public string Header { get; set; }

    //    /// <summary>
    //    /// Info's Name
    //    /// </summary>
    //    public string Name { get; set; }

    //    /// <summary>
    //    /// Sub-infos
    //    /// </summary>
    //    public List<InfoData> SubInfos { get; } = new List<InfoData>();

    //}


    ///// <summary>
    ///// Data to save values in Syntaxes
    ///// </summary>
    //public sealed class SyntaxData
    //    : DynamicSerializationFriendlyData
    //{
    //    public SyntaxData()
    //    {
    //    }

    //}



}
