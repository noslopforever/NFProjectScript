
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

        /// <summary>
        /// Settings of the data binding.
        /// </summary>
        public DataBindingSettings Settings { get; private set; } = null;

    }

}