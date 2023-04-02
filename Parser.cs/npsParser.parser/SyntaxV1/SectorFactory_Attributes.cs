using nf.protoscript.parser.syntax1.analysis;
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

            ASTParser_BlockLineEndAttributes leAttrsParser = new ASTParser_BlockLineEndAttributes();
            STNode_AttributeDefs attrs = leAttrsParser.Parse(tl);

            ASTParser_BlockLineEndComments leCmtParser = new ASTParser_BlockLineEndComments();
            STNode_Comment comment = leCmtParser.Parse(tl);

            return new AttributesSector(InReader.CurrentCodeLine, attrs, comment);
        }
    }


}
