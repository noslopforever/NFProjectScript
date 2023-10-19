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

        protected override Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent)
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

            Exception tryExcp = null;

            // Try parse as StartType member define:
            // -{Type} {Name}
            try
            {
                // Try Handle StartType
                var tl = new TokenList(tokens);
                ASTParser_BlockType blockTypeParser = new ASTParser_BlockType();
                var startTypeSig = blockTypeParser.Parse(tl);

                // If there is {Name} after {Type}
                var startTypeDefParser = new ASTParser_StatementDefMember(startTypeSig);
                var elemDef = startTypeDefParser.Parse(tl);
                if (elemDef != null)
                {
                    Sector sector = ElementSector.NewMemberSector(InCodeLine, elemDef);

                    // Try parse line-end attributes.
                    ParseHelper.TryParseLineEndBlocks(tl, (attrs, comments) =>
                    {
                        sector._SetAttributes(attrs);
                        if (comments != null)
                        {
                            sector._SetComment(comments.CommentText);
                        }
                    });

                    // if not end, there is an unexpected token
                    ParseHelper.CheckFinishedAndThrow(tl, InCodeLine);
                    return sector;
                }
            }
            catch (Exception ex)
            {
                tryExcp = ex;
            }

            // If fail, Seek back to the start and try parse Non-StartType member define:
            // - {Name} or -{Name}
            {
                var tl = new TokenList(tokens);
                var defParser = new ASTParser_StatementDefMember(null);
                var elemDef = defParser.Parse(tl);
                if (elemDef != null)
                {
                    var sector = ElementSector.NewMemberSector(InCodeLine, elemDef);
                    // Parse line-end blocks
                    ParseHelper.TryParseLineEndBlocks(tl, (attrs, comments) =>
                    {
                        sector._SetAttributes(attrs);
                        if (comments != null)
                        {
                            sector._SetComment(comments.CommentText);
                        }
                    });

                    // if not end, there is an unexpected token
                    ParseHelper.CheckFinishedAndThrow(tl, InCodeLine);

                    return sector;
                }
            }

            // Throw exception
            throw new ParserException(ParserErrorType.Factory_UnrecognizedElement);
        }

    }

}
