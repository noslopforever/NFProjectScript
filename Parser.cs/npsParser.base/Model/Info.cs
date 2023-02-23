using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace nf.protoscript
{

    /// <summary>
    /// Base class for all conceptions include in a proto: classes, members, datas, tables, plans, dead-lines, worlds, comments, attributes, etc.
    /// </summary>
    /// All infos will be organized as a tree which root is an project.
    public abstract class Info
    {
        /// <summary>
        /// A special info, the root of all other Infos.
        /// 
        /// Info without Parent (like Project, System) will always be the sub of this root info.
        /// </summary>
        internal class InternalRootInfo
            : Info
        {
            internal InternalRootInfo()
                : base("", "$ROOT")
            {

            }
        }

        /// <summary>
        /// A special info, the root of all other Infos.
        /// 
        /// Info without Parent (like Project, System) will always be the sub of this root info.
        /// </summary>
        public static Info Root { get; } = new InternalRootInfo();


        // ctor for the Root info.
        private Info(string InHeader, string InName)
        {
            ParentInfo = null;
            Header = InHeader;
            Name = InName;
        }

        public Info(Info InParentInfo, string InHeader, string InName)
        {
            if (InParentInfo == null)
            {
                InParentInfo = Root;
            }
            InParentInfo.SubAdd(this);

            ParentInfo = InParentInfo;
            Header = InHeader;
            Name = InName;

            Extra = new ExpandoObject();
            Extra.Holder = this;
        }

        /// <summary>
        /// The parent node of this info.
        /// </summary>
        public Info ParentInfo { get; }

        /// <summary>
        /// Header: indicate which role will be played by this info.
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// Name: Name of the info.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Sub infos of this node.
        /// </summary>
        public IReadOnlyCollection<Info> SubInfos { get { return mSubInfos; } }

        // internal sub-infos.
        private List<Info> mSubInfos = new List<Info>();


        /// <summary>
        /// Extra infos registered in this info.
        /// </summary>
        public dynamic Extra { get; private set; }

        /// <summary>
        /// Add InChild to this node.
        /// </summary>
        /// <param name="InChild"></param>
        private void SubAdd(Info InChild)
        {
            mSubInfos.Add(InChild);
        }

        /// <summary>
        /// Test if 'Extra' contains some property.
        /// </summary>
        /// <param name="InPropertyName"></param>
        /// <returns></returns>
        public bool IsExtraContains(string InPropertyName)
        {
            var dict = (IDictionary<string, object>)Extra;
            return dict.ContainsKey(InPropertyName);
        }

        /// <summary>
        /// Try get some property's value in 'Extra'. If not found, throw 'KeyNotFound' exception.
        /// </summary>
        /// <param name="InPropertyName"></param>
        /// <returns></returns>
        public object GetExtraProperty(string InPropertyName)
        {
            var dict = (IDictionary<string, object>)Extra;
            return dict[InPropertyName];
        }

        /// <summary>
        /// Iterate all properties in 'Extra'.
        /// </summary>
        /// <param name="InFunc"></param>
        public void ForeachExtraProperties(Func<string, object, bool> InFunc)
        {
            var dict = (IDictionary<string, object>)Extra;
            foreach (var pair in dict)
                if (!InFunc(pair.Key, pair.Value))
                    break;
        }

        /// <summary>
        /// Check if the info has a sub-info with InSubName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InSubName"></param>
        /// <returns></returns>
        public bool HasSubInfoWithName<T>(string InSubName)
            where T : Info
        {
            var selectedInfos = from info in mSubInfos
                                where (info is T
                                    && info.Name == InSubName)
                                select info as T;

            return selectedInfos.Count() != 0;
        }

        /// <summary>
        /// Check if there is a sub-info which header is InHeaderName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InHeaderName"></param>
        /// <returns></returns>
        public bool HasSubInfoWithHeader<T>(string InHeaderName)
            where T : Info
        {
            var selectedInfos = from info in mSubInfos
                                where (info is T
                                    && info.Header == InHeaderName)
                                select info as T;

            return selectedInfos.Count() != 0;
        }


        /// <summary>
        /// Iterate all sub infos by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        public void ForeachSubInfo<T>(Func<T, bool> InFunc)
            where T : Info
        {
            var selectedInfos = from info in mSubInfos
                                where info is T
                                select info as T;
            foreach (T sub in selectedInfos)
                if (!InFunc(sub))
                    return;
        }

        /// <summary>
        /// Iterate all sub infos by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        public void ForeachSubInfo<T>(Action<T> InFunc)
            where T : Info
        {
            ForeachSubInfo<T>(sub => {
                InFunc(sub);
                return true;
            });
        }

        /// <summary>
        /// Iterate all sub infos by the predict function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InPred"></param>
        public void ForeachSubInfo<T>(Func<T, bool> InFunc, Func<T, bool> InPred)
            where T : Info
        {
            var selectedInfos = from info in mSubInfos
                                where info is T
                                select info as T;
            foreach (T sub in selectedInfos)
                if (InPred(sub))
                    if (!InFunc(sub))
                        return;

        }

        /// <summary>
        /// Iterate all sub infos by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        public void ForeachSubInfo<T>(Action<T> InFunc, Func<T, bool> InPred)
            where T : Info
        {
            ForeachSubInfo<T>(sub => {
                InFunc(sub);
                return true;
            }
            , InPred);
        }

        /// <summary>
        /// Iterate all sub infos by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        public void ForeachSubInfoExclude<T, TE0>(Func<T, bool> InFunc)
            where T : Info
            where TE0 : Info
        {
            var selectedInfos = from info in mSubInfos
                                where !(info is TE0)
                                select info as T;
            foreach (T sub in selectedInfos)
                if (!InFunc(sub))
                    return;
        }

        /// <summary>
        /// Iterate all sub infos by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        public void ForeachSubInfoExclude<T, TE0>(Action<T> InFunc)
            where T : Info
            where TE0 : Info
        {
            ForeachSubInfoExclude<T, TE0>(sub => {
                InFunc(sub);
                return true;
            });
        }

        /// <summary>
        /// Foreach sub infos with InSubName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InSubName"></param>
        public void ForeachSubInfoByName<T>(Func<T, bool> InFunc, string InSubName)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Name == InSubName; });
        }

        /// <summary>
        /// Foreach sub infos with InSubName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InSubName"></param>
        public void ForeachSubInfoByName<T>(Action<T> InFunc, string InSubName)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Name == InSubName; });
        }

        /// <summary>
        /// Foreach sub infos with InSubName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InSubName"></param>
        public void ForeachSubInfoByName<T>(string InSubName, Func<T, bool> InFunc)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Name == InSubName; });
        }

        /// <summary>
        /// Foreach sub infos with InSubName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InSubName"></param>
        public void ForeachSubInfoByName<T>(string InSubName, Action<T> InFunc)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Name == InSubName; });
        }



        /// <summary>
        /// Foreach sub infos with InHeaderName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InHeaderName"></param>
        public void ForeachSubInfoByHeader<T>(Func<T, bool> InFunc, string InHeaderName)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Header == InHeaderName; });
        }

        /// <summary>
        /// Foreach sub infos with InHeaderName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InHeaderName"></param>
        public void ForeachSubInfoByHeader<T>(Action<T> InFunc, string InHeaderName)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Header == InHeaderName; });
        }

        /// <summary>
        /// Foreach sub infos with InHeaderName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InHeaderName"></param>
        public void ForeachSubInfoByHeader<T>(string InHeaderName, Func<T, bool> InFunc)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Header == InHeaderName; });
        }

        /// <summary>
        /// Foreach sub infos with InHeaderName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InFunc"></param>
        /// <param name="InHeaderName"></param>
        public void ForeachSubInfoByHeader<T>(string InHeaderName, Action<T> InFunc)
            where T : Info
        {
            ForeachSubInfo<T>(InFunc, sub => { return sub.Header == InHeaderName; });
        }

        /// <summary>
        /// Find the first subinfo that matches InPred
        /// </summary>
        /// <param name="p"></param>
        public T FindTheFirstSubInfo<T>(Func<T, bool> InPred)
            where T: Info
        {
            var selectedInfos = from info in mSubInfos
                                where (info is T
                                    && InPred(info as T))
                                select info as T;

            if (selectedInfos.Any())
            {
                return selectedInfos.First();
            }
            return null;
        }


        /// <summary>
        /// Find the first sub info with the certain name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InSubName"></param>
        /// <returns></returns>
        public T FindTheFirstSubInfoWithName<T>(string InSubName)
            where T : Info
        {
            return FindTheFirstSubInfo<T>(i => i.Name == InSubName);
        }

        /// <summary>
        /// Find the first sub info with the certain name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InHeaderName"></param>
        /// <returns></returns>
        public T FindTheFirstSubInfoWithHeader<T>(string InHeaderName)
            where T : Info
        {
            return FindTheFirstSubInfo<T>(i => i.Header == InHeaderName);
        }

    }

}
