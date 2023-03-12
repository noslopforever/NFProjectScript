using nf.protoscript.syntaxtree;

namespace nf.protoscript
{
    /// <summary>
    /// Element which indicates global-objects, child-elements of an object or archetype.
    /// 
    /// Common conceptions between 'Element' and 'Member':
    ///     - 
    /// 
    /// Differences between 'Element' and 'Member':
    ///     - Relationship to the host:
    ///         - Elements can be attached to the host but can dynamically detached and attach to another host.
    ///         - Members are hardly bound with the host like a class's member or method. 
    /// 
    /// </summary>
    public class ElementInfo
        : Info
    {
        // obtain this ctor to boost the serializer so it can use Activator.CreateInstance in a simple way.
        internal ElementInfo(Info InParentInfo, string InHeader, string InName)
            : base(InParentInfo, InHeader, InName)
        { }

        public ElementInfo(Info InParentInfo, string InHeader, string InName, TypeInfo InType, ISyntaxTreeNode InInitExpr)
            : base(InParentInfo, InHeader, InName)
        {
            ElementType = InType;
            InitSyntax = InInitExpr;
        }


        /// <summary>
        /// Type of the element.
        /// </summary>
        [Serialization.SerializableInfo]
        public TypeInfo ElementType { get; private set; }

        /// <summary>
        /// This element's init-expression.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode InitSyntax { get; private set; }

    }


}
