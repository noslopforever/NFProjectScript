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

        /// <summary>
        /// The Host's object-name.
        /// In a method or constructor, the name should be 'this', 'self'.
        /// 
        /// In some situition, we need to use this host-name when accessing members of method/constructor's host type.
        /// For example, in Js-constructors:
        ///     class Person {
        ///         constructor(name)
        ///         {
        ///             this.name = name; // Here we must have a 'this.'.
        ///         }
        ///     }
        /// 
        /// </summary>
        string HostName { get; }

    }


    /// <summary>
    /// Default implementation of ExprTranslateContext
    /// </summary>
    public class ExprTranslateContextDefault
        : IExprTranslateContext
    {
        public ExprTranslateContextDefault(Info InHostInfo, string InHostName)
        {
            HostInfo = InHostInfo;
            HostName = InHostName;
        }

        public Info HostInfo { get; }

        public string HostName { get; }

    }


}
