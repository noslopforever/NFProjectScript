using System;
using System.Collections.Generic;
using System.Text;
using nf.protoscript.syntaxtree;

namespace nf.protoscript
{

    /// <summary>
    /// Attribute, be used to add some details to the parent-Info.
    /// </summary>
    public class AttributeInfo : Info
    {
        // obtain this ctor to boost the serializer so it can use Activator.CreateInstance in a simple way.
        internal AttributeInfo(Info InParentInfo, string InHeader, string InName)
            : base(InParentInfo, InHeader, InName)
        { }

        public AttributeInfo(Info InParentInfo, string InHeader, string InName, ISyntaxTreeNode InInitExpr = null)
            : base(InParentInfo, InHeader, InName)
        {
            InitSyntaxTree = InInitExpr;
        }

        /// <summary>
        /// Init syntax of this attribute.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode InitSyntaxTree { get; private set; }

    }

}
