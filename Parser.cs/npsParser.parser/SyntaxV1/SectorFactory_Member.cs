using System;
using System.Collections.Generic;
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
            if (!InCodesWithoutIndent.StartsWith("-"))
            { return null; }

            string codesWithoutTags = InCodesWithoutIndent.Substring(1);

            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);
            var tl = new TokenList(tokens);

            // Use common DefParser to handle anonymous-type or post-type definitions.
            // - {Name} |= {Expr}|
            // -{Name}:{Type}|= {Expr}|
            //
            if (codesWithoutTags.StartsWith(" ")
                || (tokens[0].TokenType == ETokenType.ID
                    && tokens[1].TokenType == ETokenType.Colon
                    )
                )
            {
                var defParser = new analysis.ASTParser_StatementDef()
                {
                    OnlyCheckMember = true
                };
                var elemDef = defParser.Parse(tl) as analysis.STNode_ElementDef;
                if (elemDef != null)
                {
                    return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                }
            }
            else
            {
                // Try use InfoDefParser to handle pre-type definitions
                // -{Type} {Name}
                //
                var infoDefParser = new analysis.ASTParser_StatementInfoDef()
                {
                    OnlyCheckMember = true
                };

                var elemDef = infoDefParser.Parse(tl) as analysis.STNode_ElementDef;
                if (elemDef != null)
                {
                    return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                }

                // Try use DefParser to handle member overrides
                // -{Name} |= {Expr}|
                // 
                var defParser = new analysis.ASTParser_StatementDef()
                {
                    OnlyCheckMember = true
                };
                elemDef = defParser.Parse(tl) as analysis.STNode_ElementDef;
                if (elemDef != null)
                {
                    return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                }
            }

            return null;
        }

    }

}
