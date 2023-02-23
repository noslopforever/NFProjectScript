using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.test
{


    static class DataBindingFeature
    {
        public static List<string> DataBindingNames { get; } = new List<string>();

        public static List<ElementInfo> DataSourceProperties { get; } = new List<ElementInfo>();

        /// <summary>
        /// Try find databinding attributes and gather all databinding names.
        /// </summary>
        /// <param name="InInfo"></param>
        public static void GatherDataBindingNames(Info InInfo)
        {
            AttributeInfo dbAttr = InInfo.FindTheFirstSubInfoWithHeader<AttributeInfo>("db");
            if (dbAttr != null && dbAttr.InitSyntaxTree != null)
            {
                _GatherDataBindingNamesFromSyntaxes(dbAttr.InitSyntaxTree);
            }

            // recursive.
            foreach (var sub in InInfo.SubInfos)
            {
                GatherDataBindingNames(sub);
            }
        }

        /// <summary>
        /// Try mark all DataSource properties and types which contains DataSource-properties.
        /// </summary>
        /// <param name="InInfo"></param>
        public static void GatherDataSourceProperties(Info InInfo)
        {
            // Only members can trigger data-bindings.
            TypeInfo typeInfo = InInfo as TypeInfo;
            if (typeInfo != null)
            {
                typeInfo.ForeachSubInfo<ElementInfo>(elemInfo => {
                    if (DataBindingNames.Contains(elemInfo.Name))
                    {
                        DataSourceProperties.Add(elemInfo);
                    }
                });
            }

            // recursive.
            foreach (var sub in InInfo.SubInfos)
            {
                GatherDataSourceProperties(sub);
            }
        }

        private static void _GatherDataBindingNamesFromSyntaxes(ISyntaxTreeNode InSyntaxNode)
        {
            var sndb = InSyntaxNode as STNodeDataBinding;
            if (sndb != null)
            {
                var propNames = sndb.Settings.SourcePath.GatherPropertyNames();
                foreach (var n in propNames)
                {
                    DataBindingNames.Add(n);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static bool IsDataSourceProperty(ElementInfo InMember)
        {
            return DataSourceProperties.Contains(InMember);
        }
    }



}
