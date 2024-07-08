
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// DataBinding specific node.
    /// </summary>
    public class STNodeDataBinding : STNodeBase
    {
        internal STNodeDataBinding()
        {
        }

        // TODO Introduce DataSystem to manage all DataBinding related Infos and STNodes.
        internal static TypeInfo TEMP_StaticDataBindingType = new TypeInfo(SystemTypePackageInfo.Instance, "systype", "databinding");

        public STNodeDataBinding(DataBindingSettings InSettings)
        {
            Settings = InSettings;
        }

        public STNodeDataBinding(string InSourcePath, string InTargetPath)
        {
            Settings = new DataBindingSettings(InSourcePath, InTargetPath);
        }

        public STNodeDataBinding(
            EDataBindingObjectType InSourceObjectType
            , string InSourceObjectName
            , string InSourcePath
            , EDataBindingObjectType InTargetObjectType
            , string InTargetObjectName
            , string InTargetPath
            )
        {
            Settings = new DataBindingSettings(
                InSourceObjectType
                , InSourceObjectName
                , InSourcePath
                , InTargetObjectType
                , InTargetObjectName
                , InTargetPath
                );
        }

        public override void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            // TODO param in databinding params
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return TEMP_StaticDataBindingType;
        }

        /// <summary>
        /// Settings of the data binding.
        /// </summary>
        public DataBindingSettings Settings { get; private set; } = null;

        // Begin object interfaces
        public override string ToString()
        {
            return $"DataBinding {{ Settings = {Settings} }}";
        }
        // ~ End object interfaces

    }

}