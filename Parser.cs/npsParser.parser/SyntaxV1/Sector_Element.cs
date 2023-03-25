using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;

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
            Event,
            Parameter,
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
        public EType Type { get; set; }

        /// <summary>
        /// Result parsed
        /// </summary>
        public object ParsedResult { get; }

        /// <summary>
        /// Name of the element.
        /// </summary>
        public string Name { get; }


        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            Info parentInfo = InParentSector.CollectedInfo;
            if (parentInfo == null)
            {
                // TODO log error.
                throw new NotImplementedException();
                return null;
            }

            // Skip some elements because they should be handled by other processes.
            // For example:
            // - parameters should be handled by their host methods.
            // - members of inline-type should be handled by the element which introduces the inline-type.
            if (Type == EType.Parameter)
            {
                return null;
            }
            else if (Type == EType.Member)
            {
                var elemDef = ParsedResult as STNode_ElementDef;

                // Let TypeSig to find the target TypeInfo.
                TypeInfo typeInfo = CommonTypeInfos.Any;
                if (elemDef.TypeSig != null)
                {
                    typeInfo = elemDef.TypeSig.LocateTypeInfo(InProjectInfo, parentInfo);
                }

                var elemInfo = new ElementInfo(parentInfo, "member", elemDef.DefName, typeInfo, elemDef.InitExpression);
                return elemInfo;
            }

            return null;
        }


    }



}
