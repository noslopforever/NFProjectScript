using System;

namespace nf.protoscript.translator.expression.schemeSelectors
{
    /// <summary>
    /// Member Access selector implemented by ConditionSnippet.
    /// </summary>
    public class MemberAccessSchemeSelector_ConditionSnippet
        : ISTNodeTranslateSchemeSelector
    {
        public MemberAccessSchemeSelector_ConditionSnippet(
            string InSelectorName
            , int InPriority
            , ConditionSnippet InCondition
            , ISTNodeTranslateScheme InScheme
            )
        {
            SelectorName = InSelectorName;
            Priority = InPriority;
            Scheme = InScheme;
            ConditionSnippet = InCondition;
        }

        // Begin ISTNodeTranslateSchemeSelector interfaces
        public string SelectorName { get; } = "";
        public int Priority { get; } = 0;
        public ISTNodeTranslateScheme Scheme { get; }

        public bool IsMatch(ExprTranslatorAbstract.ITranslatingContext InContext)
        {
            throw new NotImplementedException();
            //return ConditionSnippet.Check(InContext);
        }

        // ~ End ISTNodeTranslateSchemeSelector interfaces

        /// <summary>
        /// The snippet to check the codition.
        /// </summary>
        public ConditionSnippet ConditionSnippet { get; }

    }


    /// <summary>
    /// Snippet to check conditions.
    /// </summary>
    public class ConditionSnippet
    {
        public ConditionSnippet(ICondition InChecker)
        {
            RootChecker = InChecker;
        }

        /// <summary>
        /// Snippet element, the condition should be constructed by a checker-tree.
        /// </summary>
        public interface ICondition
        {
            bool Check(ExprTranslatorAbstract.ITranslatingContext InContext);
        }

        /// <summary>
        /// Root condition checker.
        /// </summary>
        ICondition RootChecker { get; }

        /// <summary>
        /// Check condition
        /// </summary>
        /// <param name="InSchemeInstance"></param>
        /// <returns></returns>
        public bool Check(ExprTranslatorAbstract.ITranslatingContext InContext)
        {
            return RootChecker.Check(InContext);
        }

        /// <summary>
        /// Default AND condition.
        /// </summary>
        public class And : ICondition
        {
            public And(ICondition[] InSubCondition)
            {
                SubConditions = InSubCondition;
            }

            ICondition[] SubConditions { get; }
            public bool Check(ExprTranslatorAbstract.ITranslatingContext InContext)
            {
                foreach (var SubChecker in SubConditions)
                {
                    if (!SubChecker.Check(InContext))
                    { return false; }
                }
                return true;
            }
        }
        /// <summary>
        /// Default OR condition.
        /// </summary>
        public class Or : ICondition
        {
            public Or(ICondition[] InSubCondition)
            {
                SubConditions = InSubCondition;
            }

            ICondition[] SubConditions { get; }
            public bool Check(ExprTranslatorAbstract.ITranslatingContext InContext)
            {
                foreach (var SubChecker in SubConditions)
                {
                    if (SubChecker.Check(InContext))
                    { return true; }
                }
                return false;
            }
        }
        /// <summary>
        /// Default NOT condition.
        /// </summary>
        public class Not : ICondition
        {
            public Not(ICondition InSubCondition)
            {
                SubCondition = InSubCondition;
            }
            ICondition SubCondition { get; }
            public bool Check(ExprTranslatorAbstract.ITranslatingContext InContext)
            {
                return !SubCondition.Check(InContext);
            }
        }

    }



}
