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
        // obtain this ctor to boost the serializer so it can use Activator.CreateInstance in a simple way.
        internal MethodInfo(Info InParentInfo, string InHeader, string InName)
           : base(InParentInfo, InHeader, InName)
        { }

        public MethodInfo(Info InParentInfo, string InHeader, string InName, DelegateTypeInfo InMethodSignature, STNodeSequence InExecSequence)
            : base(InParentInfo, InHeader, InName)
        {
            MethodSignature = InMethodSignature;
            ExecSequence = InExecSequence;
        }

        /// <summary>
        /// The method's signature.
        /// </summary>
        [Serialization.SerializableInfo]
        public DelegateTypeInfo MethodSignature { get; private set; }

        /// <summary>
        /// Execution expressions.
        /// </summary>
        [Serialization.SerializableInfo]
        public STNodeSequence ExecSequence { get; private set; }

    }

}
