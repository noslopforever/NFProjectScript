using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// Serialization friendly data
    /// </summary>
    public interface ISerializationFriendlyData
    {
    }

    /// <summary>
    /// POD (int, byte, string, structure ...) data.
    /// </summary>
    public sealed class PODData
        : ISerializationFriendlyData
    {
        internal PODData()
        {
        }
        public PODData(object InValue)
        {
            Value = InValue;
        }

        /// <summary>
        /// The POD value.
        /// </summary>
        public object Value { get; set; }

    }

    /// <summary>
    /// CollectionData that holds a collection
    /// </summary>
    public sealed class CollectionData
        : List<ISerializationFriendlyData>
        , ISerializationFriendlyData
    {

        /// <summary>
        /// Type of the collection.
        /// </summary>
        public Type CollectionType { get; set; }

    }

    /// <summary>
    /// DictionaryData which holds a dictionary.
    /// </summary>
    public sealed class DictionaryData
        : Dictionary<ISerializationFriendlyData, ISerializationFriendlyData>
        , ISerializationFriendlyData
    {

        /// <summary>
        /// Type of the dictionary.
        /// </summary>
        public Type DictionaryType { get; set; }

    }

    /// <summary>
    /// Null data but saves Type of the value.
    /// </summary>
    public sealed class NullData
        : ISerializationFriendlyData
    {
        internal NullData()
        {
        }
        public NullData(Type InType)
        {
            Type = InType;
        }

        /// <summary>
        /// Type of the value
        /// </summary>
        public Type Type { get; set; }

    }

    /// <summary>
    /// Data to save info references.
    /// </summary>
    public sealed class InfoRefData
        : ISerializationFriendlyData
    {
        internal InfoRefData()
        {
        }
        public InfoRefData(string InTypeFullname)
        {
            Fullname = InTypeFullname;
        }

        /// <summary>
        /// Path of the info: 
        /// -   Package.Type.SubInfo.SubInfo
        /// </summary>
        public string Fullname { get; set; }

    }


    //public sealed class MethodDelegateTypeData
    //    : SerializationFriendlyDataBase
    //{
    //    // TODO ...
    //}


    /// <summary>
    /// Dynamic serialization friendly data.
    /// </summary>
    public abstract class DynamicSerializationFriendlyData
        : ISerializationFriendlyData
    {
        /// <summary>
        /// Class of the source object.
        /// </summary>
        public string Typename { get; set; }

        /// <summary>
        /// Append datas of info: Member's InitExpr, Method's Codebody, etc...
        /// </summary>
        private ExpandoObject Extra { get; set; } = new ExpandoObject();

        /// <summary>
        /// Try get extra data by name.
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="OutData"></param>
        /// <returns></returns>
        public bool TryGetExtraData(string InName, out ISerializationFriendlyData OutData)
        {
            OutData = null;

            IDictionary<string, object> dict = Extra as IDictionary<string, object>;
            object dataObj = null;
            if (!dict.TryGetValue(InName, out dataObj))
            {
                return false;
            }

            // The dataObj must implement ISerializationFriendlyDataBase
            if (!typeof(ISerializationFriendlyData).IsAssignableFrom(dataObj.GetType()))
            {
                throw new InvalidCastException();
            }

            OutData = dataObj as ISerializationFriendlyData;
            return true;
        }

        /// <summary>
        /// Try add some extra
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InData"></param>
        /// <returns></returns>
        public bool TryAdd(string InName, ISerializationFriendlyData InData)
        {
            return Extra.TryAdd(InName, InData);
        }

    }

    /// <summary>
    /// Hold all Info's necessary datas which can be used to reconstruct the Info itself in the future.
    /// InfoData is Serializer-friendly.
    /// </summary>
    public sealed class InfoData
        : DynamicSerializationFriendlyData
    {
        public InfoData()
        {
        }

        /// <summary>
        /// Info's header
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Info's Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sub-infos
        /// </summary>
        public List<InfoData> SubInfos { get; } = new List<InfoData>();

    }


    /// <summary>
    /// Data to save values in Syntaxes
    /// </summary>
    public sealed class SyntaxData
        : DynamicSerializationFriendlyData
    {
        public SyntaxData()
        {
        }

    }



}
