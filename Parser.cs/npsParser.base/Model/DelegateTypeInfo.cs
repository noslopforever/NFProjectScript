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

        public override string ToString()
        {
            string typeName = base.ToString();

            // Gather params
            string paramsStr = "";
            ForeachSubInfoByHeader<ElementInfo>("param", param =>
            {
                paramsStr += $"{param.Name}, ";
            });
            // Try removing the last ', '
            if (paramsStr.Length > 2)
            {
                paramsStr = paramsStr.Remove(paramsStr.Length - 2);
            }

            // Gather return string.
            string retStr = "";
            if (ReturnType != null)
            {
                retStr = $": {ReturnType}";
            }

            return $"{typeName}({paramsStr}){retStr}";
        }

    }


}
