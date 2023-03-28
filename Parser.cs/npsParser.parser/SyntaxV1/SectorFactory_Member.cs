using System;
using System.Collections.Generic;
using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// Try parse sector as a member
    /// </summary>
    public class SectorFactory_Member
        : SectorFactory
    {

        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            //
            //-{Type} {Name} |= {Expr}|
            //- {Name} |= {Expr}|
            //-{Name}:{Type} |= {Expr}|
            //

            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCode(InCodesWithoutIndent, "-", out codesWithoutTags))
            { return null; }

            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);

            // Try Handle StartType
            var tl = new TokenList(tokens);
            ASTParser_BlockType blockTypeParser = new ASTParser_BlockType();
            var startTypeSig = blockTypeParser.Parse(tl);

            // Try parse as StartType member define:
            // -{Type} {Name}
            try
            {
                // If there is {Name} after {Type}
                if (tl.CheckToken(ETokenType.ID))
                {
                    var startTypeDefParser = new ASTParser_StatementDefMember(startTypeSig, true);
                    var elemDef = startTypeDefParser.Parse(tl);
                    if (elemDef != null)
                    {
                        return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                    }
                }
            }
            catch
            {
            }

            // If fail, Seek back to the start and try parse Non-StartType member define:
            // - {Name} or -{Name}
            {
                tl.Seek(0);
                var defParser = new ASTParser_StatementDefMember(null, true);
                var elemDef = defParser.Parse(tl);
                if (elemDef != null)
                {
                    return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                }
            }

            // TODO log error
            throw new NotImplementedException();
            return null;
        }

    }

}
