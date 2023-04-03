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
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            string codesWithoutTags = "";
            if (!ParseHelper.CheckAndRemoveStartCode(InCodesWithoutIndent, "@", out codesWithoutTags))
            {
                return null;
            }

            // Parse line-end attributes from codes.
            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(InCodesWithoutIndent, ref tokens);
            TokenList tl = new TokenList(tokens);

            return ParseHelper.TryParseLineEndBlocks(tl, (attrs, comments) =>
            {
                return new AttributesSector(InReader.CurrentCodeLine, attrs, comments);
            });
        }
    }


}
