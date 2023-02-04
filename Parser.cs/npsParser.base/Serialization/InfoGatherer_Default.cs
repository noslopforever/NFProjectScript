using System;
using System.Collections.Generic;
using System.Reflection;

namespace nf.protoscript.Serialization
{
    /// <summary>
    /// The default InfoGatherer.
    /// </summary>
    public class InfoGatherer_Default
        : InfoGatherer
    {
        protected override void RestoreData(InfoData InSourceData, Info InTargetInfo)
        {
            Type infoType = InTargetInfo.GetType();
            var props = _FindPropertiesHandledByThis(infoType);
            foreach(var prop in props)
            {
                object value = null;

                // TODO handle renames, re-types
                if (InSourceData.AppendData.TryGetValue(prop.Name, out value))
                {
                    prop.SetValue(InTargetInfo, value);
                }
            }
        }

        protected override void GatherData(Info InSourceInfo, InfoData InTargetData)
        {
            Type infoType = InSourceInfo.GetType();

            // Exact properties which save in sub-classes of Info.
            var props = _FindPropertiesHandledByThis(infoType);
            foreach (var prop in props)
            {
                object value = prop.GetValue(InSourceInfo);
                // If value is an Info, it should always be a reference.

                InTargetData.AppendData.Add(prop.Name, value);
            }
        }

        /// <summary>
        /// Select properties handled by this Gatherer.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> _FindPropertiesHandledByThis(Type InType)
        {
            List<PropertyInfo> gatherProps = new List<PropertyInfo>();
            var props = InType.GetProperties();
            foreach (var prop in props)
            {
                if (_IsPropertyHandledByThis(prop))
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
        private bool _IsPropertyHandledByThis(PropertyInfo prop)
        {
            // Exclude system and base-Info properties.
            if (prop.ReflectedType == typeof(Info)
                || prop.ReflectedType == typeof(object)
                )
            {
                return false;
            }

            // If the property is marked as Serializable, gather it.
            if (prop.GetCustomAttribute<SerializableInfoAttribute>() != null)
            {
                return true;
            }

            return false;
        }

    }



}
