using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Element Sector like members, child-actors, components.
    /// </summary>
    public class ElementSector
        : Sector
    {
        public enum EType
        {
            Unknown,
            Member,
            ComponentOrChild,
            EventAttachment,
            Event,
            Reference,
        }

        internal ElementSector(Token[] InTokens, EType InType, object InParsedResult)
            : base(InTokens)
        {
            Type = InType;
            ParsedResult = InParsedResult;
        }

        internal static ElementSector NewMemberSector(Token[] InTokens, analysis.STNode_ElementDef InElemDef)
        {
            return new ElementSector(InTokens, EType.Member, InElemDef);
        }

        /// <summary>
        /// Type of the element.
        /// </summary>
        EType Type { get; }

        /// <summary>
        /// Result parsed
        /// </summary>
        public object ParsedResult { get; }

        /// <summary>
        /// Name of the element.
        /// </summary>
        string Name { get; }

        public override Info CollectInfos(ProjectInfo InProjectInfo, Info InParentInfo)
        {
            var tl = new TokenList(this.Tokens);

            if (Type == EType.Member)
            {
                var elemDef = ParsedResult as STNode_ElementDef;

                // Let TypeSig to find the target TypeInfo.
                TypeInfo typeInfo = CommonTypeInfos.Any;
                if (elemDef.TypeSig != null)
                {
                    typeInfo = elemDef.TypeSig.LocateTypeInfo(InProjectInfo, InParentInfo);
                }

                ElementInfo elemInfo = new ElementInfo(InParentInfo, "member", elemDef.DefName, typeInfo, elemDef.InitExpression);
                return elemInfo;
            }

            return null;
        }
    }


}
