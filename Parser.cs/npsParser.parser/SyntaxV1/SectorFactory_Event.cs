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
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCodes(InCodesWithoutIndent, out codesWithoutTags, "~", ">>"))
            { return null; }

            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);

            // Try parse Event definitions.
            var defParser = new analysis.ASTParser_StatementDefEvent();
            var tl = new TokenList(tokens);
            var funcDef = defParser.Parse(tl);
            if (funcDef != null)
            {
                return ElementSector.NewEventSector(tokens.ToArray(), funcDef);
            }

            return null;
        }

    }

}
