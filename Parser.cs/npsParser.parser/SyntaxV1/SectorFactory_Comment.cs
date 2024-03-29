﻿using nf.protoscript.parser.token;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Try parse a sector with only comments (start with #)
    /// </summary>
    public class SectorFactory_Comment
        : SectorFactory
    {
        protected override Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent)
        {
            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCodes(InCodesWithoutIndent, out codesWithoutTags, "#"))
            { return null; }

            var cmtSector = new CommentSector(InCodeLine, InCodesWithoutIndent);
            return cmtSector;
        }
    }

}
