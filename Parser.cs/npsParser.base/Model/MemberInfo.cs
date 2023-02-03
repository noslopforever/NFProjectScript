using System;
using System.Collections.Generic;
using System.Text;
using nf.protoscript.syntaxtree;

namespace nf.protoscript
{

    /// <summary>
    /// Member, always be sub info of a Type or Archetype.
    /// </summary>
    public class MemberInfo : Info
    {
        // obtain this ctor to boost the serializer so it can use Activator.CreateInstance in a simple way.
        internal MemberInfo(Info InParentInfo, string InHeader, string InName)
            : base(InParentInfo, InHeader, InName)
        { }

        public MemberInfo(Info InParentInfo, string InHeader, string InName, TypeInfo InType, ISyntaxTreeNode InInitExpr)
            : base(InParentInfo, InHeader, InName)
        {
            Archetype = InType;
            InitSyntax = InInitExpr;
        }

        /// <summary>
        /// Archetype of the member.
        /// </summary>
        [Serialization.SerializableInfo]
        public TypeInfo Archetype { get; private set; }

        /// <summary>
        /// This member's init-expression.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode InitSyntax { get; private set; }

    }

}
