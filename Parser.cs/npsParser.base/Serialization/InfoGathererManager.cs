using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// A singleton to manage all InfoGatherers.
    /// 
    /// - Make pair between Info-type and InfoGatherer.
    /// - Find the best InfoGatherer for an Info which is pending to be serialized.
    /// 
    /// </summary>
    public class InfoGathererManager
    {
        private InfoGathererManager()
        {
            RegisterInfoGatherer(typeof(Info), new InfoGatherer_Default());
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static InfoGathererManager Instance { get; } = new InfoGathererManager();

        Dictionary<Type, InfoGatherer> _GathererTable = new Dictionary<Type, InfoGatherer>();

        /// <summary>
        /// Register a new Gatherer to an info.
        /// </summary>
        /// <param name="InType"></param>
        /// <param name="InInfoGatherer"></param>
        public void RegisterInfoGatherer(Type InType, InfoGatherer InInfoGatherer)
        {
            _GathererTable.Add(InType, InInfoGatherer);
        }

        /// <summary>
        /// Find the best gatherer matches InInfo.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public InfoGatherer FindGatherer(Info InInfo)
        {
            return FindGatherer(InInfo.GetType());
        }

        /// <summary>
        /// Find the best gatherer matches InType.
        /// </summary>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public InfoGatherer FindGatherer(Type InType)
        {
            if (InType == typeof(object) || InType == null)
            { return null; }

            // Find InfoGatherer paired with InType
            InfoGatherer result = null;
            if (_GathererTable.TryGetValue(InType, out result))
            {
                return result;
            }

            // Not found, return default.
            return FindGatherer(InType.BaseType);
        }
    }

}
