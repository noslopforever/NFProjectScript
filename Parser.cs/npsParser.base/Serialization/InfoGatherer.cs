using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// Gather datas from an Info.
    /// </summary>
    public class InfoGatherer
    {
        // facades

        /// <summary>
        /// Gather InfoDatas from an InInfo.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public static InfoData Gather(Info InInfo)
        {
            InfoData gatherData = new InfoData();

            // Handle basic infos.
            gatherData.Class = InInfo.GetType().ToString();
            gatherData.Header = InInfo.Header;
            gatherData.Name = InInfo.Name;

            // Handle info specific datas.
            var gatherer = InfoGathererManager.Instance.FindGatherer(InInfo);
            System.Diagnostics.Debug.Assert(gatherer != null);
            gatherer.GatherData(InInfo, gatherData);

            // Handle SubInfos.
            foreach (var sub in InInfo.SubInfos)
            {
                var subInfoData = Gather(sub);
                gatherData.SubInfos.Add(subInfoData);
            }

            return gatherData;
        }

        /// <summary>
        /// Restore an InfoData into an Info.
        /// </summary>
        /// <param name="InParentInfo"></param>
        /// <param name="InData"></param>
        /// <returns></returns>
        public static Info Restore(Info InParentInfo, InfoData InData)
        {
            // Find an approxiate gatherer.
            Type infoType = Type.GetType(InData.Class);
            var gatherer = InfoGathererManager.Instance.FindGatherer(infoType);
            System.Diagnostics.Debug.Assert(gatherer != null);

            // New info instance from InData.
            Info info= gatherer.RestoreInstance(InData, InParentInfo);

            // Handle info specific datas.
            gatherer.RestoreData(InData, info);

            // Handle SubInfos
            foreach (var subData in InData.SubInfos)
            {
                Info subInfo = Restore(info, subData);
            }

            return info;
        }


        // overridable

        /// <summary>
        /// Gather datas of InSourceInfo.
        /// </summary>
        /// <param name="InSourceInfo"></param>
        /// <param name="InTargetData"></param>
        protected virtual void GatherData(Info InSourceInfo, InfoData InTargetData)
        {
            Type infoType = InSourceInfo.GetType();

            // Exact properties which save in sub-classes of Info.
            var props = _FindPropertiesHandledByThis(infoType);
            foreach (var prop in props)
            {
                object value = prop.GetValue(InSourceInfo);

                object valueToWrite = value;

                // If value is an Info, it should always be a reference.

                // TODO friendly-value converter.
                //var cvter = InfoGathererManager.Instance.FindConverterFor(value);
                //if (cvter != null)
                //{
                //    valueToWrite = cvter.Convert(value);
                //}

                InTargetData.AppendData.TryAdd(prop.Name, valueToWrite);
            }
        }

        /// <summary>
        /// Create Info instance when restoring.
        /// </summary>
        /// <param name="InSourceData"></param>
        /// <param name="InParentInfo"></param>
        /// <returns></returns>
        protected virtual Info RestoreInstance(InfoData InSourceData, Info InParentInfo)
        {
            Type infoType = Type.GetType(InSourceData.Class);
            //var ctor = infoType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            //    , null
            //    , new Type[] { typeof(Info), typeof(string), typeof(string) }
            //    , null
            //    );
            //Info result = ctor.Invoke(new object[] { InParentInfo, InSourceData.Header, InSourceData.Name }) as Info;
            Info result = Activator.CreateInstance(
                // type
                infoType
                // binding flags
                , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                // binder
                , null
                // parameters
                , new object[] {
                    InParentInfo
                    , InSourceData.Header
                    , InSourceData.Name
                    }
                // culture info
                , null
                ) as Info;
            return result;
        }

        /// <summary>
        /// Restore InTargetInfo's datas from InSourceData.
        /// </summary>
        /// <param name="InSourceData"></param>
        /// <param name="InTargetInfo"></param>
        protected virtual void RestoreData(InfoData InSourceData, Info InTargetInfo)
        {
            Type infoType = InTargetInfo.GetType();
            var props = _FindPropertiesHandledByThis(infoType);
            foreach (var prop in props)
            {
                // TODO handle renames, re-types

                object value = null;
                var dict = (IDictionary<string, object>)InSourceData.AppendData;
                if (dict.TryGetValue(prop.Name, out value))
                {
                    // value maybe null.
                    prop.SetValue(InTargetInfo, value);
                }
            }
        }
        


        // internal

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
