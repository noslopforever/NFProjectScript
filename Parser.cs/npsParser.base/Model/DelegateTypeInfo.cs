namespace nf.protoscript
{
    /// <summary>
    /// Infos to describe a delegate type (or function type).
    /// -   Contains Input and Output parameters. (Sub MemberTypes of this type)
    /// </summary>
    public class DelegateTypeInfo : TypeInfo
    {
        public DelegateTypeInfo(Info InParentInfo, string InHeader, string InName)
            : base(InParentInfo, InHeader, InName)
        {
        }

    }


}
