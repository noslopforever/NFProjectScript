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
