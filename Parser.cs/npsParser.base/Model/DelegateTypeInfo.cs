namespace nf.protoscript
{
    /// <summary>
    /// Infos to describe a delegate type (or function type).
    /// -   Contains Input and Output parameters. (Sub MemberTypes of this type)
    /// </summary>
    public class DelegateTypeInfo : TypeInfo
    {
        public DelegateTypeInfo(Info InParentInfo, string InHeader, string InName, TypeInfo InReturnType)
            : base(InParentInfo, InHeader, InName)
        {
            ReturnType = InReturnType;
        }

        /// <summary>
        /// Return type of the function (delegate)
        /// </summary>
        public TypeInfo ReturnType { get; private set; }

    }


}
