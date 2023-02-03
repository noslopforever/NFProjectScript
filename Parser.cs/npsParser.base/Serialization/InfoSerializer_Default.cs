using System;
using System.Collections.Generic;
using System.Reflection;

namespace nf.protoscript.Serialization
{
    /// <summary>
    /// The default InfoSerializer.
    /// </summary>
    public class InfoSerializer_Default
        : InfoSerializer
    {
        protected override void DeserializeData(InfoSerializationData InSourceData, Info InTargetInfo)
        {
            Type infoType = InTargetInfo.GetType();
            var props = _SelectSerialProperties(infoType);
            foreach(var prop in props)
            {
                object value = null;

                // TODO handle renames, re-types
                InSourceData.AppendData.TryGetValue(prop.Name, out value);
                prop.SetValue(InTargetInfo, value);
            }
        }

        protected override void SerializeData(Info InSourceInfo, InfoSerializationData InTargetData)
        {
            Type infoType = InSourceInfo.GetType();

            // Exact properties which save in sub-classes of Info.
            var props = _SelectSerialProperties(infoType);
            foreach (var prop in props)
            {
                InTargetData.AppendData.Add(prop.Name, prop.GetValue(InSourceInfo));
            }
        }

        /// <summary>
        /// Select properties handled by this Serializer.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> _SelectSerialProperties(Type InType)
        {
            List<PropertyInfo> serialProps = new List<PropertyInfo>();
            var props = InType.GetProperties();
            foreach (var prop in props)
            {
                if (_IsSerialProperty(prop))
                {
                    serialProps.Add(prop);
                }
            }
            return serialProps;
        }

        /// <summary>
        /// Check if the InProperty needs to be serialized.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private bool _IsSerialProperty(PropertyInfo prop)
        {
            // Exclude system and base-Info properties.
            if (prop.ReflectedType == typeof(Info)
                || prop.ReflectedType == typeof(object)
                )
            {
                return false;
            }

            // If the property is marked as Serializable, serialize it.
            if (prop.GetCustomAttribute<SerializableInfoAttribute>() != null)
            {
                return true;
            }

            return false;
        }

    }



}
