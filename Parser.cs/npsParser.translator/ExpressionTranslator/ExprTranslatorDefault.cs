using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{


    public class ExprTranslatorDefault
        : ExprTranslatorAbstract
    {

        // Begin ExprTranslatorAbstract interfaces

        protected override ISTNodeTranslateScheme ErrorScheme(STNodeBase InErrorNode)
        {
            throw new NotImplementedException();
        }

        public override ISTNodeTranslateScheme FindSchemeByName(string InSchemeName)
        {
            if (_genericSchemes.TryGetValue(InSchemeName, out var scheme))
            {
                return scheme;
            }
            return null;
        }

        protected override ISTNodeTranslateScheme QueryNullScheme(TypeInfo InConstType)
        {
            throw new NotImplementedException();
        }

        protected override ISTNodeTranslateScheme QueryConstGetScheme(TypeInfo InConstType, string InValueString)
        {
            return DefaultConstScheme;
        }

        protected override ISTNodeTranslateScheme QueryConstGetStringScheme(TypeInfo InConstType, string InString)
        {
            return DefaultConstScheme;
        }

        protected override ISTNodeTranslateScheme QueryConstGetInfoScheme(TypeInfo InConstType, Info InInfo)
        {
            return DefaultConstScheme;
        }

        protected override ISTNodeTranslateScheme QueryMemberAccessScheme(
            EExprVarAccessType InAccessType
            , TypeInfo InContextType
            , string InMemberID
            , out TypeInfo OutMemberType
            )
        {
            var elemInfo = InfoHelper.FindPropertyOfType(InContextType, InMemberID);
            if (elemInfo == null)
            {
                OutMemberType = CommonTypeInfos.Any;
                // TODO log warning: cannot find the property in type.
            }
            else
            {
                OutMemberType = elemInfo.ElementType;
            }

            // Find special MemberAccess schemes by (HostType, PropertyName)
            foreach (var selectorKvp in _memberAccessSelectors)
            {
                if (selectorKvp.Value.IsMatch(InAccessType, InContextType, InMemberID, elemInfo))
                {
                    return selectorKvp.Value.Scheme;
                }
            }

            // Return default InScheme.
            switch (InAccessType)
            {
                case EExprVarAccessType.Get:
                    return DefaultVarGetScheme;
                case EExprVarAccessType.Set:
                    return DefaultVarSetScheme;
                case EExprVarAccessType.Ref:
                    return DefaultVarRefScheme;
            }
            return null;
        }

        protected override ISTNodeTranslateScheme QueryGlobalVarAccessScheme(
            EExprVarAccessType InAccessType
            , Info InGlobalInfo
            , string InVarName
            )
        {
            switch (InAccessType)
            {
                case EExprVarAccessType.Get:
                    return DefaultVarGetScheme;
                case EExprVarAccessType.Set:
                    return DefaultVarSetScheme;
                case EExprVarAccessType.Ref:
                    return DefaultVarRefScheme;
            }
            return null;
        }

        protected override ISTNodeTranslateScheme QueryMethodVarAccessScheme(
            EExprVarAccessType InAccessType
            , IExprTranslateContext InTranslatingContext
            , ElementInfo InMethodInfo
            , string InVarName
            )
        {
            switch (InAccessType)
            {
                case EExprVarAccessType.Get:
                    return DefaultVarGetScheme;
                case EExprVarAccessType.Set:
                    return DefaultVarSetScheme;
                case EExprVarAccessType.Ref:
                    return DefaultVarRefScheme;
            }
            return null;
        }



        protected override ISTNodeTranslateScheme QueryBinOpScheme(STNodeBinaryOp InBinOpNode, TypeInfo InLhsType, TypeInfo InRhsType, out TypeInfo OutResultType)
        {
            // TODO decide OutResultType by InBinOpNode.OpCode
            OutResultType = CommonTypeInfos.Any;
            return DefaultBinOpScheme;
        }

        protected override ISTNodeTranslateScheme QueryHostAccessScheme(STNodeMemberAccess InMemberAccessNode, TypeInfo InHostType)
        {
            return DefaultHostAccessScheme;
        }

        // ~ End ExprTranslatorAbstract interfaces


        /// <summary>
        /// Register a MemberAccess SchemeSelector.
        /// </summary>
        /// <param name="InSelector"></param>
        public void AddMemberAccessSchemeSelector(IMemberAccessSchemeSelector InSelector)
        {
            _memberAccessSelectors.Add(InSelector.Priority, InSelector);
        }

        /// <summary>
        /// Add generic schemes
        /// </summary>
        /// <param name="InKey"></param>
        /// <param name="InSnippet"></param>
        public void AddScheme(string InKey, STNodeTranslateSnippet InSnippet)
        {
            _genericSchemes[InKey] = new STNodeTranslateSchemeDefault(InSnippet);
        }

        //
        // Default schemes
        //

        public ISTNodeTranslateScheme DefaultConstScheme { get; set; }
        public ISTNodeTranslateScheme DefaultVarGetScheme { get; set; }
        public ISTNodeTranslateScheme DefaultVarRefScheme { get; set; }
        public ISTNodeTranslateScheme DefaultVarSetScheme { get; set; }
        public ISTNodeTranslateScheme DefaultBinOpScheme { get; set; }
        public ISTNodeTranslateScheme DefaultHostAccessScheme { get; set; }

        //
        // Scheme Selectors
        //

        SortedList<int, IMemberAccessSchemeSelector> _memberAccessSelectors 
            = new SortedList<int, IMemberAccessSchemeSelector>(new DuplicateKeyComparer<int>());

        // Generic schemes
        Dictionary<string, ISTNodeTranslateScheme> _genericSchemes
            = new Dictionary<string, ISTNodeTranslateScheme>();

        // TODO Move it into the 'Base' project.
        class DuplicateKeyComparer<TKey>
            : IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1; // Handle equality as being greater. Note: this will break Remove(key) or
                else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
                    return result;
            }

            #endregion
        }

    }



}
