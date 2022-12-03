using System;
using System.Collections.Generic;
using System.Text;
using nf.protoscript.syntaxtree;

namespace nf.protoscript
{

    /// <summary>
    /// Infos to describe a Function.
    /// -   Contains Input and Output parameters. (Sub MemberTypes of this type)
    /// -   Contains execution expressions.
    /// </summary>
    public class FunctionInfo : TypeInfo
    {
        public FunctionInfo(Info InParentInfo, string InHeader, string InName) : base(InParentInfo, InHeader, InName)
        {
        }

        /// <summary>
        /// Execution expressions.
        /// </summary>
        IReadOnlyList<ISyntaxTreeNode> ExecSyntaxes;

    }


}
