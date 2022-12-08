using System;
using System.Text;
using nf.protoscript.syntaxtree;

namespace nf.protoscript
{

    /// <summary>
    /// Infos to describe a method of a type.
    /// -   Contains execution expressions.
    /// </summary>
    public class MethodInfo : Info
    {
        public MethodInfo(Info InParentInfo, string InHeader, string InName, DelegateTypeInfo InMethodSignature, STNodeSequence InExecSequence)
            : base(InParentInfo, InHeader, InName)
        {
            MethodSignature = InMethodSignature;
            ExecSequence = InExecSequence;
        }

        /// <summary>
        /// The method's signature.
        /// </summary>
        public DelegateTypeInfo MethodSignature { get; }

        /// <summary>
        /// Execution expressions.
        /// </summary>
        public STNodeSequence ExecSequence { get; }

    }

}
