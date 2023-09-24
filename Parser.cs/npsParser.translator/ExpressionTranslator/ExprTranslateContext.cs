using nf.protoscript.syntaxtree;
using System.Collections;
using System.IO;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Runtime context created by info-translators to describe the environment of the translating expression.
    /// </summary>
    /// <example>
    /// Member init-expression:
    /// model Person
    ///     age = 32
    /// 
    /// The translate context should be:
    ///     HostInfo: Person (a TypeInfo)
    ///     ScopeChain:
    ///             ScopeInfo       | ScopeName | ScopePresent
    ///         --------------------|-----------|--------------
    ///         Project             | global    | "::"
    ///         Person(TypeInfo)    | this      | "this->"
    /// </example>
    /// <example>
    /// Method expressions:
    /// model Person
    ///     + talk(Other)
    ///         > var = 32
    /// 
    /// The translate context should be:
    ///     HostInfo: Person.talk (a MethodInfo)
    ///     ScopeChain:
    ///             ScopeInfo       | ScopeName | ScopePresent
    ///         --------------------|-----------|--------------
    ///         Project             | global    | "::"
    ///         Person(TypeInfo)    | this      | "this->"
    ///         talk (MethodInfo)   | method    | ""
    /// </example>
    public interface IExprTranslateContext
    {

        /// <summary>
        /// Info which hold this expression.
        /// 
        /// e.g.
        ///     Method expressions:                             The host MethodInfo.
        ///     Member init expressions (ctor expressions):     The host TypeInfo.
        ///     Lambda expressions:                             The host TypeInfo where the lambda is defined in.
        ///     Global function expressions:                    The host ProjectInfo which holds the global varaible or function.
        /// 
        /// </summary>
        Info HostInfo { get; }

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

        }

        /// <summary>
        /// Find variable in the host's Info-Chain
        /// </summary>
        /// <param name="InName"></param>
        /// <returns></returns>
        IVariable FindVariable(string InName);


        /// <summary>
        /// Add Temporary variable to this context.
        /// </summary>
        /// <param name="InNodeToTranslate"></param>
        /// <param name="InTempVarKey"></param>
        /// <param name="InTempVarInitCodes"></param>
        /// <returns></returns>
        IVariable AddTempVar(ISyntaxTreeNode InNodeToTranslate, string InTempVarKey);

    }


}
