using nf.protoscript.syntaxtree;
using System;

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
            SettedElementType = InType;
            InitSyntax = InInitExpr;
        }

        /// <summary>
        /// Type of the element.
        /// </summary>
        public TypeInfo ElementType
        {
            get
            {
                // 1, If have init-syntax, try return the init-syntax's PredictType.
                //    e.g. unit t = new Tank(), here the type of T should be the Tank inherited from Unit.
                if (InitSyntaxPredictType != CommonTypeInfos.Unknown)
                {
                    return InitSyntaxPredictType;
                }

                // 2, If setted manually, return the setted type.
                if (SettedElementType != CommonTypeInfos.Unknown)
                {
                    return SettedElementType;
                }

                // 3, If override from anthoer element, try return the overriding element's type.
                if (OverrideElement != null)
                {
                    return OverrideElement.ElementType;
                }

                // Still unknown
                return CommonTypeInfos.Unknown;
            }
        }

        /// <summary>
        /// Type of the element. Set manually by parsing nps scripts.
        /// </summary>
        [Serialization.SerializableInfo]
        public TypeInfo SettedElementType { get; private set; } = CommonTypeInfos.Unknown;

        /// <summary>
        /// This element's init-expression.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode InitSyntax { get; private set; }

        /// <summary>
        /// return the type predicted by the init-syntax.
        /// </summary>
        public TypeInfo InitSyntaxPredictType
        {
            get
            {
                if (InitSyntax != null)
                {
                    // TODO InitSyntax may return null if the init-syntax is a non-type statement like "return;".
                    // We need to guard against this situation, because what we often need here is to treat it as a "function" type.
                    return InitSyntax.GetPredictType(this);
                }
                return CommonTypeInfos.Unknown;
            }
        }

        /// <summary>
        /// Which Element has been overloaded by this element.
        /// </summary>
        [Serialization.SerializableInfo]
        public ElementInfo OverrideElement { get; internal set; }


        public static void Unsafe_SetOverrideElement(ElementInfo InTargetElem, ElementInfo InOverrideSource)
        {
            InTargetElem.OverrideElement = InOverrideSource;
        }

    }


}
