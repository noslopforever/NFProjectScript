using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// A singleton to manage all InfoSerializers.
    /// 
    /// - Make pair with Info-types and InfoSerializers.
    /// - Find the best InfoSerializer of a serializing Info.
    /// 
    /// </summary>
    public class InfoSerializerManager
    {
        private InfoSerializerManager()
        {
            RegSerializer(typeof(Info), new InfoSerializer_Default());
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static InfoSerializerManager Instance { get; } = new InfoSerializerManager();

        Dictionary<Type, InfoSerializer> _SerializerTable = new Dictionary<Type, InfoSerializer>();

        /// <summary>
        /// Register a new Serializer to an info.
        /// </summary>
        /// <param name="InType"></param>
        /// <param name="InInfoSerializer"></param>
        public void RegSerializer(Type InType, InfoSerializer_Default InInfoSerializer)
        {
            _SerializerTable.Add(InType, InInfoSerializer);
        }

        /// <summary>
        /// Find the best serializer matches InInfo.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public InfoSerializer FindSerializer(Info InInfo)
        {
            return FindSerializer(InInfo.GetType());
        }

        /// <summary>
        /// Find the best serializer matches InType.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public InfoSerializer FindSerializer(Type InType)
        {
            if (InType == typeof(object) || InType == null)
            { return null; }

            // Find InfoSerializer paired with InType
            InfoSerializer result = null;
            if (_SerializerTable.TryGetValue(InType, out result))
            {
                return result;
            }

            // Not found, return default.
            return FindSerializer(InType.BaseType);
        }
    }

}
