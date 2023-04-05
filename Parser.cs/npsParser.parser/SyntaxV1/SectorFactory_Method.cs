using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Try parse sector as a method.
    /// </summary>
    public class SectorFactory_Method
        : SectorFactory
    {
        protected override Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent)
        {
            // all method sectors start with a '+'.
            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCode(InCodesWithoutIndent, "+", out codesWithoutTags))
            { return null; }

            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);

            // Try parse as StartType member define:
            // -{Type} {Name}
            try
            {
                // Try Handle StartType
                var tl = new TokenList(tokens);
                ASTParser_BlockType blockTypeParser = new ASTParser_BlockType();
                var startTypeSig = blockTypeParser.Parse(tl);

                // If there is {Name} after {Type}
                var startTypeDefParser = new ASTParser_StatementDefFunction(startTypeSig);
                var funcDef = startTypeDefParser.Parse(tl);
                if (funcDef != null)
                {
                    var sector = ElementSector.NewMethodSector(InCodeLine, funcDef);
                    // Parse line-end blocks
                    ParseHelper.TryParseLineEndBlocks(tl, (attrs, comments) =>
                    {
                        sector._SetAttributes(attrs);
                        sector._SetComment(comments);
                    });

                    // if not end, there is an unexpected token
                    ParseHelper.CheckFinishedAndThrow(tl, InCodeLine);

                    return sector;
                }
            }
            catch
            {
            }

            // If fail, Seek back to the start and try parse Non-StartType member define:
            // + {Name} or +{Name}
            {
                var tl = new TokenList(tokens);
                var defParser = new ASTParser_StatementDefFunction(null);
                var funcDef = defParser.Parse(tl);
                if (funcDef != null)
                {
                    var sector = ElementSector.NewMethodSector(InCodeLine, funcDef);
                    // Parse line-end blocks
                    ParseHelper.TryParseLineEndBlocks(tl, (attrs, comments) =>
                    {
                        sector._SetAttributes(attrs);
                        sector._SetComment(comments);
                    });

                    // if not end, there is an unexpected token
                    ParseHelper.CheckFinishedAndThrow(tl, InCodeLine);

                    return sector;
                }
            }

            // Throw exception
            throw new ParserException(ParserErrorType.Factory_UnrecognizedMethod);
        }

    }

}
