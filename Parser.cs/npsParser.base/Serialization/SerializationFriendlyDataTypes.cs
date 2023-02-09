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

        public static SerializationFriendlyData NewEmpty()
        { 
            return new SerializationFriendlyData();
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

            if (!(savedVal is SerializationFriendlyData))
            {
                return false;
            }

            OutData = savedVal as SerializationFriendlyData;
            return true;
        }

        /// <summary>
        /// Get extra data by name.
        /// </summary>
        /// <param name="InPropName"></param>
        /// <returns></returns>
        public SerializationFriendlyData GetExtra(string InPropName)
        {
            return this[InPropName] as SerializationFriendlyData;
        }

        #region "Data Accessors"

        public bool IsNull()
        {
            return HasMember("NullValueType");
        }

        public Type GetNullType()
        {
            var nullTypeData = this["NullValueType"] as SerializationFriendlyData;
            return nullTypeData.AsType();
        }

        public static SerializationFriendlyData NewNullData(Type InDeclValueType)
        {
            var dynData = new SerializationFriendlyData();
            dynData["NullValueType"] = SerializationFriendlyData.NewTypeData(InDeclValueType);
            return dynData;
        }

        public bool IsType()
        {
            return HasMember("Type");
        }

        public Type AsType()
        {
            Type type = this["Type"] as Type;
            return type;
        }

        public static SerializationFriendlyData NewTypeData(Type InDeclValueType)
        {
            var dynData = new SerializationFriendlyData();
            dynData["Type"] = InDeclValueType;
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

            var dynData = new SerializationFriendlyData();
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

        public static SerializationFriendlyData NewInfoRefName(string InInfoFullName)
        {
            var dynData = new SerializationFriendlyData();
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

        public Type GetCollectionType()
        {
            var collTypeData = this["CollType"] as SerializationFriendlyData;
            return collTypeData.AsType();
        }

        public static SerializationFriendlyData NewCollection(Type InDeclCollectionType, List<SerializationFriendlyData> InCollection)
        {
            var dynData = new SerializationFriendlyData();
            dynData["CollType"] = SerializationFriendlyData.NewTypeData(InDeclCollectionType);
            dynData["Collection"] = InCollection;
            return dynData;
        }

        public bool IsDictionary()
        {
            return HasMember("Dictionary");
        }

        public IReadOnlyDictionary<SerializationFriendlyData, SerializationFriendlyData> AsDictionary()
        {
            var dict = this["Dictionary"] as Dictionary<SerializationFriendlyData, SerializationFriendlyData>;
            return dict;
        }

        public Type GetDictionaryType()
        {
            var dictTypeData = this["DictType"] as SerializationFriendlyData;
            return dictTypeData.AsType();
        }

        public static SerializationFriendlyData NewDictionary(Type InDeclDictType, Dictionary<SerializationFriendlyData, SerializationFriendlyData> InDictionary)
        {
            var dynData = new SerializationFriendlyData();
            dynData["DictType"] = SerializationFriendlyData.NewTypeData(InDeclDictType);
            dynData["Dictionary"] = InDictionary;
            return dynData;
        }


        /// <summary>
        /// Data is object, all members in the data will be treated as properties of the target object.
        /// </summary>
        /// <returns></returns>
        public bool IsObject()
        {
            return HasMember("SFD_ObjectType");
        }

        /// <summary>
        /// Check if the InType is assignable from this object data.
        /// </summary>
        /// <param name="InType"></param>
        /// <returns></returns>
        public bool IsObjectOf(Type InType)
        {
            return IsObject()
                && InType.IsAssignableFrom(GetObjectType())
                ;
        }

        public object AsObject()
        {
            // Take this dictionary as an object.
            return this;
        }

        public Type GetObjectType()
        {
            var objTypeData = this["SFD_ObjectType"] as SerializationFriendlyData;
            return objTypeData.AsType();
        }

        public static SerializationFriendlyData NewObject(Type InDeclObjectType)
        {
            var dynData = new SerializationFriendlyData();
            dynData["SFD_ObjectType"] = SerializationFriendlyData.NewTypeData(InDeclObjectType);
            return dynData;
        }

        #endregion

    }


}
