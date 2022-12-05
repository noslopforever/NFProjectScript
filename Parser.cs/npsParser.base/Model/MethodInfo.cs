using System;
using System.Text;
using nf.protoscript.syntaxtree;

namespace nf.protoscript
{

    /// <summary>
    /// Infos to describe a method of a type.
    /// -   Contains execution expressions.
    /// </summary>
    public class MethodInfo : MemberInfo
    {
        public MethodInfo(Info InParentInfo, string InHeader, string InName, DelegateTypeInfo InType, STNodeSequence InExecSequence)
            : base(InParentInfo, InHeader, InName, InType, InExecSequence)
        {
        }

        /// <summary>
        /// Execution expressions.
        /// </summary>
        public STNodeSequence ExecSequence { get { return InitSyntax as STNodeSequence; } }

    }

}
