using nf.protoscript.parser.token;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// SectorFactory_Attributes to parse and generate AttributesSector.
    /// </summary>
    class SectorFactory_Attributes
        : SectorFactory
    {
        protected override Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent)
        {
            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCode(InCodesWithoutIndent, "@", out codesWithoutTags))
            {
                return null;
            }

            // Parse line-end attributes from codes.
            string comments = "";
            List<Token> tokens = TokenParser_CommonNps.Instance.ParseLine(InCodesWithoutIndent, out comments);
            TokenList tl = new TokenList(tokens);

            return ParseHelper.TryParseLineEndBlocks(tl, (attrs) =>
            {
                return new AttributesSector(InCodeLine, attrs);
            });
        }
    }


}
