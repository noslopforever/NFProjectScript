using System;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// Info serializer to serialize a Info into a InfoSerializationData.
    /// </summary>
    public abstract class InfoSerializer
    {
        /// <summary>
        /// Serialize InInfo into an InfoSerializationData, so it can be handled with Xml or Json Serializer.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public static InfoSerializationData Serialize(Info InInfo)
        {
            InfoSerializationData serialData = new InfoSerializationData();

            // Handle basic infos.
            serialData.Class = InInfo.GetType().ToString();
            serialData.Header = InInfo.Header;
            serialData.Name = InInfo.Name;

            // Handle info specific datas.
            var serializer = InfoSerializerManager.Instance.FindSerializer(InInfo);
            System.Diagnostics.Debug.Assert(serializer != null);
            serializer.SerializeData(InInfo, serialData);

            // Handle SubInfos.
            foreach (var sub in InInfo.SubInfos)
            {
                serialData.SubInfos.Add(Serialize(sub));
            }

            return serialData;
        }

        /// <summary>
        /// Deserialize an InfoSerializationData into an info.
        /// </summary>
        /// <param name="InParentInfo"></param>
        /// <param name="InData"></param>
        /// <returns></returns>
        public static Info Deserialize(Info InParentInfo, InfoSerializationData InData)
        {
            // Find an approxiate serializer.
            Type infoType = Type.GetType(InData.Class);
            var serializer = InfoSerializerManager.Instance.FindSerializer(infoType);
            System.Diagnostics.Debug.Assert(serializer != null);

            // New info instance from InData.
            Info info= serializer.DeserializeInstance(InData, InParentInfo);

            // Handle info specific datas.
            serializer.DeserializeData(InData, info);

            // Handle SubInfos
            foreach (var subData in InData.SubInfos)
            {
                Info subInfo = Deserialize(info, subData);
            }

            return info;
        }

        /// <summary>
        /// Serialize datas of InSourceInfo.
        /// </summary>
        /// <param name="InSourceInfo"></param>
        /// <param name="InTargetData"></param>
        protected abstract void SerializeData(Info InSourceInfo, InfoSerializationData InTargetData);

        /// <summary>
        /// Create Info instance when deserializing.
        /// </summary>
        /// <param name="InSourceData"></param>
        /// <param name="InParentInfo"></param>
        /// <returns></returns>
        protected virtual Info DeserializeInstance(InfoSerializationData InSourceData, Info InParentInfo)
        {
            Type infoType = Type.GetType(InSourceData.Class);
            Info result = Activator.CreateInstance(infoType, InParentInfo, InSourceData.Header, InSourceData.Name) as Info;
            return result;
        }

        /// <summary>
        /// Deserialize InTargetInfo's datas from InSourceData.
        /// </summary>
        /// <param name="InSourceData"></param>
        /// <param name="InTargetInfo"></param>
        protected abstract void DeserializeData(InfoSerializationData InSourceData, Info InTargetInfo);

    }



}
