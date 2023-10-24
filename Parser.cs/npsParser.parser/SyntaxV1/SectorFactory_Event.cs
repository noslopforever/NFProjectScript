using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// Try parse sector as an event-def sector or a event-attachment sector.
    /// 
    /// Events have no return values, so ">> EventName" is same with ">>EventName".
    /// 
    /// event-def sector: {ID} + {(} ...
    ///     ~{EventName}({Param}...)
    ///     >>{EventName}({Param}...)
    /// 
    /// event-attachment sector: {ID} + {+=} or {ID}
    ///     >>{EventName} += {Expr}
    ///     ~{EventName} += {Expr}
    ///     
    ///     ~{EventName}
    ///         {Expr} # Body or Multiline-body
    ///     >>{EventName}
    ///         {Expr} # Body or Multiline-body
    ///     
    /// </summary>
    public class SectorFactory_Event
        : SectorFactory
    {
        protected override Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent)
        {
            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCodes(InCodesWithoutIndent, out codesWithoutTags, "~", ">>"))
            { return null; }

            string comments = "";
            var tokens = TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, out comments);
            var tl = new TokenList(tokens);

            // Try parse Event definitions.
            var defParser = new analysis.ASTParser_StatementDefEvent();
            var funcDef = defParser.Parse(tl);
            if (funcDef != null)
            {
                var sector = ElementSector.NewEventSector(InCodeLine, funcDef);
                sector._SetComment(comments);

                // Try parse line-end attributes.
                ParseHelper.TryParseLineEndBlocks(tl, sector._SetAttributes);

                // if not end, there is an unexpected token
                ParseHelper.CheckFinishedAndThrow(tl, InCodeLine);

                return sector;
            }

            return null;
        }

    }

}
