using System;
using nf.protoscript.translator.expression;

namespace nf.protoscript.translator.expression.schemeSelectors
{
    /// <summary>
    /// Member Access selector implemented by Lambda expressions.
    /// </summary>
    public class MemberAccessSchemeSelector_Lambda
        : IMemberAccessSchemeSelector
    {
        public MemberAccessSchemeSelector_Lambda(
            string InSelectorName
            , int InPriority
            , EExprVarAccessType InAccessType
            , Func<TypeInfo, string, ElementInfo, bool> InConditionChecker
            , ISTNodeTranslateScheme InScheme
            )
        {
            SelectorName = InSelectorName;
            Priority = InPriority;
            AccessType = InAccessType;
            Scheme = InScheme;
            ConditionChecker = InConditionChecker;
        }

        // Begin ISTNodeTranslateSchemeSelector interfaces
        public string SelectorName { get; } = "";
        public int Priority { get; } = 0;
        public ISTNodeTranslateScheme Scheme { get; }
        // ~ End ISTNodeTranslateSchemeSelector interfaces

        // Begin IMemberAccessSchemeSelector interfaces
        public bool IsMatch(EExprVarAccessType InAccessType, TypeInfo InHostType, string InMemberName, ElementInfo InMemberElementInfo)
        {
            if (InAccessType != AccessType)
            {
                return false;
            }
            return ConditionChecker(InHostType, InMemberName, InMemberElementInfo);
        }
        // ~ End IMemberAccessSchemeSelector interfaces

        /// <summary>
        /// The Access Type presented by the Scheme.
        /// </summary>
        public EExprVarAccessType AccessType { get; }

        /// <summary>
        /// The lambda to check the codition.
        /// </summary>
        public Func<TypeInfo, string, ElementInfo, bool> ConditionChecker { get; }

    }


}
