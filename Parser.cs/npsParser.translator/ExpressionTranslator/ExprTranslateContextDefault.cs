using System.Collections.Generic;


namespace nf.protoscript.translator.expression
{
    /// <summary>
    /// Default implementation of ExprTranslateContext
    /// </summary>
    public class ExprTranslateContextDefault
        : IExprTranslateContext
    {
        public ExprTranslateContextDefault(
            Info InHostInfo
            , IEnumerable<Scope> InScopeChain
            )
        {
            HostInfo = InHostInfo;
            ScopeChain = InScopeChain;
        }

        /// <summary>
        /// scope to find variables.
        /// </summary>
        public class Scope
            : IExprTranslateContext.IScope
        {

            public Scope(Info InScopeInfo
                , string InScopeHostName
                , string InScopeHostPresent
                )
            {
                ScopeInfo = InScopeInfo;
                ScopeName = InScopeHostName;
                ScopePresentCode = InScopeHostPresent;
            }

            /// <summary>
            /// Info of the scope
            /// </summary>
            public Info ScopeInfo { get; }

            /// <summary>
            /// Host Name of the scope, <see cref="IExprTranslateContext.IVariable"/>  for more informations.
            /// </summary>
            public string ScopeName { get; }

            /// <summary>
            /// Host Present of the scope, <see cref="IExprTranslateContext.IVariable"/>  for more informations.
            /// </summary>
            public string ScopePresentCode { get; }

        }

        /// <summary>
        /// Variable from ElementInfo
        /// </summary>
        public class ElementInfoVar
            : IExprTranslateContext.IVariable
        {
            public ElementInfoVar(ElementInfo InVarElement, Scope InScope)
            {
                VarElement = InVarElement;
                HostScope = InScope;
            }

            public ElementInfo VarElement { get; }

            // Begin IVariable interfaces
            public string Name { get { return VarElement.Name; } }
            public TypeInfo VarType { get { return VarElement.ElementType; } }
            public IExprTranslateContext.IScope HostScope { get; }
            public ISTNodeTranslateScheme OverrideVarGetScheme { get; }
            public ISTNodeTranslateScheme OverrideVarSetScheme { get; }
            public ISTNodeTranslateScheme OverrideVarRefScheme { get; }
            // ~ End IVariable interfaces.
        }

        public Info HostInfo { get; }
        public IEnumerable<Scope> ScopeChain { get; }

        public IExprTranslateContext.IVariable FindVariable(string InName)
        {
            foreach (Scope scope in ScopeChain)
            {
                if (scope.ScopeInfo is TypeInfo)
                {
                    var propertyInfo = InfoHelper.FindPropertyOfType(scope.ScopeInfo as TypeInfo, InName);
                    if (propertyInfo != null)
                    {
                        return new ElementInfoVar(propertyInfo, scope);
                    }
                }

                // TODO find local and temp first.

                // Try find archetype and global variables.
                if (scope.ScopeInfo is ElementInfo)
                {
                    var varInfo = InfoHelper.FindPropertyInArchetype(scope.ScopeInfo as ElementInfo, InName);
                    if (varInfo != null)
                    {
                        return new ElementInfoVar(varInfo, scope);
                    }
                }

                if (scope.ScopeInfo is ProjectInfo)
                {
                    var globalInfo = InfoHelper.FindPropertyInGlobal(scope.ScopeInfo as ProjectInfo, InName);
                    if (globalInfo != null)
                    {
                        return new ElementInfoVar(globalInfo, scope);
                    }
                }
            }

            return null;
        }

    }


}
