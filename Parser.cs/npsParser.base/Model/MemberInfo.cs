using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript
{

    /// <summary>
    /// Member, always be sub info of a Type or Archetype.
    /// </summary>
    public class MemberInfo : Info
    {
        public MemberInfo(Info InParentInfo, string InHeader, string InName, TypeInfo InType, IExpression InInitExpr)
            : base(InParentInfo, InHeader, InName)
        {
            Archetype = InType;
            InitExpression = InInitExpr;
        }

        /// <summary>
        /// Archetype of the member.
        /// </summary>
        public TypeInfo Archetype { get; private set; }

        /// <summary>
        /// This member's init-expression.
        /// </summary>
        public IExpression InitExpression { get; private set; }

    }

}
