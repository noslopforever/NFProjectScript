using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.test
{


    static class DataBindingFeature
    {
        public static List<string> DataBindingNames { get; } = new List<string>();

        public static List<MemberInfo> DataSourceProperties { get; } = new List<MemberInfo>();

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
                typeInfo.ForeachSubInfo<MemberInfo>(m => {
                    if (DataBindingNames.Contains(m.Name))
                    {
                        DataSourceProperties.Add(m);
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
            if (InSyntaxNode is STNodeGetVar)
            {
                DataBindingNames.Add((InSyntaxNode as STNodeGetVar).IDName);
            }
            else if (InSyntaxNode is STNodeBinaryOp)
            {
                var binOp = InSyntaxNode as STNodeBinaryOp;
                _GatherDataBindingNamesFromSyntaxes(binOp.LHS);
                _GatherDataBindingNamesFromSyntaxes(binOp.RHS);
            }
            else if (InSyntaxNode is STNodeSub)
            {
                var sub = InSyntaxNode as STNodeSub;
                _GatherDataBindingNamesFromSyntaxes(sub.LHS);
                _GatherDataBindingNamesFromSyntaxes(sub.RHS);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static bool IsDataSourceProperty(MemberInfo InMember)
        {
            return DataSourceProperties.Contains(InMember);
        }
    }



}
