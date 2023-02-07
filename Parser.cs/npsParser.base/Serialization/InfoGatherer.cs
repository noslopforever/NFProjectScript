﻿using System;
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
            gatherData.Typename = InInfo.GetType().ToString();
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
            Type infoType = Type.GetType(InData.Typename);
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
            var props = SerializationHelper._FindPropertiesHandledByGatherer(infoType);
            foreach (var prop in props)
            {
                // Read source value.
                object value = prop.GetValue(InSourceInfo);

                // Convert source value to friendly-data and save it.
                var dataToWrite = SerializationHelper.ConvertValueToData(prop.PropertyType, value);
                InTargetData.TryAdd(prop.Name, dataToWrite);
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
            Type infoType = Type.GetType(InSourceData.Typename);
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
            var props = SerializationHelper._FindPropertiesHandledByGatherer(infoType);
            foreach (var prop in props)
            {
                // TODO handle renames, re-types

                // Read saved data
                ISerializationFriendlyData data = null;
                if (InSourceData.TryGetExtraData(prop.Name, out data))
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