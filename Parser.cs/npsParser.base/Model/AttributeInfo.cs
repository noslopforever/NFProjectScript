using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript
{

    /// <summary>
    /// Attribute, be used to add some details to the parent-Info.
    /// </summary>
    public class AttributeInfo : Info
    {
        public AttributeInfo(Info InParentInfo, string InHeader, string InName)
            : base(InParentInfo, InHeader, InName)
        {
        }

        /// <summary>
        /// Init Expression of this attribute.
        /// </summary>
        IExpression InitExpression;

    }

}
