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
        public static SerializationFriendlyData Gather(Info InInfo)
        {
            SerializationFriendlyData gatherData = SerializationFriendlyData.NewObject(InInfo.GetType());

            // Handle basic infos.
            gatherData["Header"] = InInfo.Header;
            gatherData["Name"] = InInfo.Name;

            // Handle info specific datas.
            var gatherer = InfoGathererManager.Instance.FindGatherer(InInfo);
            System.Diagnostics.Debug.Assert(gatherer != null);
            gatherer.GatherData(InInfo, gatherData);

            // Handle SubInfos if not empty.
            if (InInfo.SubInfos.Count != 0)
            {
                var subInfos = new List<SerializationFriendlyData>();
                foreach (var sub in InInfo.SubInfos)
                {
                    var subInfoData = Gather(sub);
                    subInfos.Add(subInfoData);
                }
                gatherData["SubInfos"] = subInfos;
            }

            return gatherData;
        }

        /// <summary>
        /// Restore an InfoData into an Info.
        /// </summary>
        /// <param name="InParentInfo"></param>
        /// <param name="InData"></param>
        /// <returns></returns>
        public static Info Restore(Info InParentInfo, SerializationFriendlyData InData)
        {
            System.Diagnostics.Debug.Assert(InData.SourceValueType.IsSubclassOf(typeof(Info)));

            // Find an approxiate gatherer.
            Type infoType = InData.SourceValueType;
            var gatherer = InfoGathererManager.Instance.FindGatherer(infoType);
            System.Diagnostics.Debug.Assert(gatherer != null);

            // New info instance from InData.
            Info info = gatherer.RestoreInstance(InData.SourceValueType, InData["Header"] as string, InData["Name"] as string, InParentInfo);

            // Handle info specific datas.
            gatherer.RestoreData(InData, info);

            // Handle SubInfos
            if (InData.HasMember("SubInfos"))
            {
                var subInfos = InData["SubInfos"] as List<SerializationFriendlyData>;
                foreach (var subData in subInfos)
                {
                    Info subInfo = Restore(info, subData);
                }
            }

            return info;
        }


        // overridable

        /// <summary>
        /// Gather datas of InSourceInfo.
        /// </summary>
        /// <param name="InSourceInfo"></param>
        /// <param name="InTargetData"></param>
        protected virtual void GatherData(Info InSourceInfo, SerializationFriendlyData InTargetData)
        {
            Type infoType = InSourceInfo.GetType();

            // Exact properties which save in sub-classes of Info.
            var props = SerializationHelper._FindPropertiesHandledByGatherer(infoType);
            foreach (var prop in props)
            {
                // Read source value.
                object value = prop.GetValue(InSourceInfo);

                // Convert source value to friendly-data and save it.
                var dataToWrite = SerializationHelper.ConvertValueToData(prop.PropertyType, value);
                InTargetData.TryAddExtra(prop.Name, dataToWrite);
            }
        }

        /// <summary>
        /// Create Info instance when restoring.
        /// </summary>
        /// <param name="InSourceData"></param>
        /// <param name="InParentInfo"></param>
        /// <returns></returns>
        protected virtual Info RestoreInstance(Type InInfoType, string InHeader, string InName, Info InParentInfo)
        {
            Info result = Activator.CreateInstance(
                // type
                InInfoType
                // binding flags
                , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                // binder
                , null
                // parameters
                , new object[] {
                    InParentInfo
                    , InHeader
                    , InName
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
        protected virtual void RestoreData(SerializationFriendlyData InSourceData, Info InTargetInfo)
        {
            Type infoType = InTargetInfo.GetType();
            var props = SerializationHelper._FindPropertiesHandledByGatherer(infoType);
            foreach (var prop in props)
            {
                // TODO handle renames, re-types

                // Read saved data
                SerializationFriendlyData data = null;
                if (InSourceData.TryGetExtra(prop.Name, out data))
                {
                    // Restore friendly-data to value.
                    object valueToRestore = SerializationHelper.RestoreValueFromData(prop.PropertyType, data);

                    // Write it to the target property.
                    // The value may be null.
                    prop.SetValue(InTargetInfo, valueToRestore);
                }
            }
        }


    }



}
