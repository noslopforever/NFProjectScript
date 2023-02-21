
using System;
using System.Collections.Generic;

namespace nf.protoscript
{

    /// <summary>
    /// ObjectType which determines the way to find the (source/target) object.
    /// </summary>
    public enum EDataBindingObjectType
    {
        /// <summary>
        /// 'This' object.
        /// </summary>
        This,

        /// <summary>
        /// Data Context of a ContentObject
        /// </summary>
        DataContext,

        /// <summary>
        /// Static global object, find by name.
        /// </summary>
        StaticGlobal,

        /// <summary>
        /// Static Resource, the name may contains more informations (like types and pathes) than Global.
        /// </summary>
        Resource,

        /// <summary>
        /// Host or parent element,
        /// </summary>
        Ancestor,

    }


    /// <summary>
    /// DataBinding path
    /// </summary>
    public class DataBindingPath
    {
        internal DataBindingPath()
        { }

        public DataBindingPath(string InStr)
        {
            if (-1 != InStr.IndexOf(".") && -1 != InStr.IndexOf("["))
            {
                throw new NotImplementedException();
            }

            Entries = new Entry[] { new PropertyEntry(InStr) };
        }

        /// <summary>
        /// DataBinding entries.
        /// </summary>
        public abstract class Entry
        {
            public abstract void GatherPropertyNames(List<string> RefList);
        }

        /// <summary>
        /// Property entry (A.B) in the entries
        /// </summary>
        public class PropertyEntry : Entry
        {
            internal PropertyEntry()
            { }
            public PropertyEntry(string InStr)
            { Name = InStr; }

            public string Name { get; private set; }

            public override string ToString()
            {
                return $"{Name}";
            }

            public override void GatherPropertyNames(List<string> RefList)
            {
                RefList.Add(Name);
            }

        }

        /// <summary>
        /// Collection entry (Coll[Key]) in the entries.
        /// </summary>
        public class CollectionEntry: Entry
        {
            public string Name { get; private set; }

            public DataBindingPath KeyPath { get; private set; }

            public override string ToString()
            {
                string keyStr = KeyPath.ToString();
                return $"{Name}[{keyStr}]";
            }

            public override void GatherPropertyNames(List<string> RefList)
            {
                RefList.Add(Name);
                RefList.AddRange(KeyPath.GatherPropertyNames());
            }

        }

        /// <summary>
        /// Gather property names in this path.
        /// 
        /// g:A.B.C[g:E.F.G].D  B,C,F,G,D should be gathered which means these properties' update should trigger the databinding of this path.
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GatherPropertyNames()
        {
            List<string> list = new List<string>();
            foreach (var e in Entries)
            {
                e.GatherPropertyNames(list);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Entries from the left to the right.
        /// </summary>
        public Entry[] Entries { get; }

        /// <summary>
        /// Is the path a simple property. (HP, Name, Rank, Score ...)
        /// </summary>
        public bool IsSimpleProperty 
        {
            get
            {
                return Entries.Length == 1 && Entries[0] is PropertyEntry;
            } 
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < Entries.Length; i++)
            {
                var e = Entries[i];
                if (i > 0)
                {
                    result += ".";
                }
                result += e.ToString();
            }
            return result;
        }

    }


    /// <summary>
    /// Common DataBinding settings
    /// </summary>
    public class DataBindingSettings
    {
        public DataBindingSettings(string InSourcePath, string InTargetPath)
        {
            SourceObjectType = EDataBindingObjectType.DataContext;
            SourceObjectName = "";
            SourcePath = new DataBindingPath(InSourcePath);

            TargetObjectType = EDataBindingObjectType.This;
            TargetObjectName = "";
            TargetPath = new DataBindingPath(InTargetPath);
        }

        public DataBindingSettings(
            EDataBindingObjectType InSourceObjectType
            , string InSourceObjectName
            , string InSourcePath
            , EDataBindingObjectType InTargetObjectType
            , string InTargetObjectName
            , string InTargetPath
            )
        {
            SourceObjectType = InSourceObjectType;
            SourceObjectName = InSourceObjectName;
            SourcePath = new DataBindingPath(InSourcePath);

            TargetObjectType = InTargetObjectType;
            TargetObjectName = InTargetObjectName;
            TargetPath = new DataBindingPath(InTargetPath);
        }

        public EDataBindingObjectType SourceObjectType { get; }

        public string SourceObjectName { get; }

        public DataBindingPath SourcePath { get; }

        public EDataBindingObjectType TargetObjectType { get; }

        public string TargetObjectName { get; }

        public DataBindingPath TargetPath { get; }

    }

}
