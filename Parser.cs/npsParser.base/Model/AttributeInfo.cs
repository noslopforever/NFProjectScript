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
        public AttributeInfo(Info InParentInfo, string InHeader, string InName, ISyntaxTreeNode InInitExpr = null)
            : base(InParentInfo, InHeader, InName)
        {
            InitSyntaxTree = InInitExpr;
        }

        /// <summary>
        /// Init syntax of this attribute.
        /// </summary>
        public ISyntaxTreeNode InitSyntaxTree { get; }

    }

}
