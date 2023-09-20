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


}
