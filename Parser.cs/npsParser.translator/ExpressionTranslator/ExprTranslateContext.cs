using System.Collections.Generic;
using static nf.protoscript.translator.expression.IExprTranslateContext;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Runtime context created by info-translators to describe the environment of the translating expression.
    /// </summary>
    public interface IExprTranslateContext
    {

        /// <summary>
        /// Info which hold this expression.
        /// 
        /// e.g.
        ///     Method expressions:             The host MethodInfo.
        ///     Ctor expressions:               The host TypeInfo.
        ///     Lambda expressions:             The TypeInfo where this lambda is defined.
        ///     Global function expressions:    null.
        /// 
        /// </summary>
        Info HostInfo { get; }

        ///// <summary>
        ///// Translated type of the host.
        ///// </summary>
        //ITranslatedType HostType { get; }
        ///// ~ Saved by each scope

        /// <summary>
        /// Scope to find variables.
        /// </summary>
        public interface IScope
        {
            /// <summary>
            /// Context info bound with the scope.
            /// </summary>
            Info ScopeInfo { get; }

            /// <summary>
            /// Name of the scope which holds variable in it.
            /// In the constructor and most methods, it may be this/self.
            /// </summary>
            string ScopeName { get; }

            /// <summary>
            /// Scope present code.
            /// Most of time, we use "this->", "this." or "self." to access a host's members and WILL NOT use host-name instantly.
            /// </summary>
            string ScopePresentCode { get; }

        }

        /// <summary>
        /// Variable found by this context.
        /// 
        /// The variable may be a Temporary variable which had not been bound with an ElementInfo.
        /// 
        /// </summary>
        public interface IVariable
        {
            ///// <summary>
            ///// Info of the variable, if have.
            ///// </summary>
            //ElementInfo VarElement { get; }

            /// <summary>
            /// Name of the variable.
            /// </summary>
            string Name { get; }

            /// <summary>
            /// Type of the Variable
            /// </summary>
            TypeInfo VarType { get; }

            /// <summary>
            /// The scope which hold this variable.
            /// </summary>
            IScope HostScope { get; }

            /// <summary>
            /// Scheme to Get the variable.
            /// </summary>
            ISTNodeTranslateScheme OverrideVarGetScheme { get; }

            /// <summary>
            /// Scheme to Set the variable.
            /// </summary>
            ISTNodeTranslateScheme OverrideVarSetScheme { get; }

            /// <summary>
            /// Scheme to Ref the variable.
            /// </summary>
            ISTNodeTranslateScheme OverrideVarRefScheme { get; }

        }

        /// <summary>
        /// Find variable in the host's Info-Chain
        /// </summary>
        /// <param name="InName"></param>
        /// <returns></returns>
        IVariable FindVariable(string InName);

        ///// <summary>
        ///// Scopes where elements should be found in.
        ///// </summary>
        //IEnumerable<Info> ScopeChain { get; }
        ///// ~ Replaced by FindVariable call.


        ///// <summary>
        ///// The scheme instance to present the 'host' object (like this in C++/Javascript).
        ///// 
        ///// In some situition, we need to use this host when accessing members of method/constructor's host type.
        ///// For example, in Js-constructors:
        /////     class Person {
        /////         constructor(name)
        /////         {
        /////             this.name = name; // Here we must have a 'this.'.
        /////         }
        /////     }
        ///// 
        ///// </summary>
        //ISTNodeTranslateSchemeInstance HostSchemeInstance { get; }
        ///// ~ Replaced by EnvVar

    }


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
            : IScope
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
            : IVariable
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
            public IScope HostScope { get; }
            public ISTNodeTranslateScheme OverrideVarGetScheme { get; }
            public ISTNodeTranslateScheme OverrideVarSetScheme { get; }
            public ISTNodeTranslateScheme OverrideVarRefScheme { get; }
            // ~ End IVariable interfaces.
        }

        public Info HostInfo { get; }
        public IEnumerable<Scope> ScopeChain { get; }

        public IVariable FindVariable(string InName)
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
