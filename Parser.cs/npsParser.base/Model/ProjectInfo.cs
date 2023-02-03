using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript
{

    /// <summary>
    /// Roots of all infos in a project.
    /// </summary>
    public class ProjectInfo : Info
    {
        // obtain this ctor to boost the serializer so it can use Activator.CreateInstance in a simple way.
        internal ProjectInfo(Info InParentInfo, string InHeader, string InName)
            : base(InParentInfo, InHeader, InName)
        { }

        public ProjectInfo(string InName)
            : base(null, "", InName)
        {
        }
        public ProjectInfo(string InHeader, string InName)
            : base(null, InHeader, InName)
        {
        }

    }


}
