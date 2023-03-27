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

            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCode(InCodesWithoutIndent, "-", out codesWithoutTags))
            { return null; }


            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);

            // Use common DefParser to handle anonymous-type or post-type definitions.
            // - {Name} |= {Expr}|
            //
            if (codesWithoutTags.StartsWith(" "))
            {
                var defParser = new analysis.ASTParser_StatementDef(
                    analysis.ASTParser_StatementDef.EDefType.Element
                    , analysis.ASTParser_StatementDef.ESyntaxType.EmptyStartType
                    );
                var tl = new TokenList(tokens);
                var elemDef = defParser.Parse(tl) as analysis.STNode_ElementDef;
                if (elemDef != null)
                {
                    return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                }
            }
            else
            {
                try
                {
                    // Try use DefParser in Pre-Type mode to handle pre-type definitions:
                    // -{Type} {Name}
                    //
                    var preTypeParser = new analysis.ASTParser_StatementDef(
                        analysis.ASTParser_StatementDef.EDefType.Element
                        , analysis.ASTParser_StatementDef.ESyntaxType.ParseStartType
                        );

                    var tl = new TokenList(tokens);
                    var elemDef = preTypeParser.Parse(tl) as analysis.STNode_ElementDef;
                    if (elemDef != null)
                    {
                        return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                    }
                }
                catch
                {

                }

                // Try use DefParser in Post-Type mode to handle member overrides
                // -{Name} |= {Expr}|
                // -{Name}:{Type}|= {Expr}|
                // 
                {
                    var postTypeParser = new analysis.ASTParser_StatementDef(
                        analysis.ASTParser_StatementDef.EDefType.Element
                        , analysis.ASTParser_StatementDef.ESyntaxType.ParsePostType
                        );
                    var tl = new TokenList(tokens);
                    var elemDef = postTypeParser.Parse(tl) as analysis.STNode_ElementDef;
                    if (elemDef != null)
                    {
                        return ElementSector.NewMemberSector(tokens.ToArray(), elemDef);
                    }
                }

            }

            return null;
        }

    }

}
