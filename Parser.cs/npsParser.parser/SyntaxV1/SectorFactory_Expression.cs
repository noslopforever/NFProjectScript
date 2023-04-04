using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// SectorFactory_Expression to parse and generate ExpressionSector.
    /// </summary>
    class SectorFactory_Expression
        : SectorFactory
    {
        protected override Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent)
        {
            // Only handle "> EXPR_STATEMENT"
            string codesWithoutDefTag = "";
            if (!ParseHelper.CheckAndRemoveStartCode(InCodesWithoutIndent, ">", out codesWithoutDefTag))
            {
                return null;
            }

            string codesWithoutTags = InCodesWithoutIndent.Substring(1);

            // Parse expression statement from codes.
            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);
            TokenList tl = new TokenList(tokens, InCodeLine);

            // Parse tag and consume tag tokens.
            string tag = "";
            if (tl.CheckToken(ETokenType.ID)
                && tl.CheckNextToken(ETokenType.Colon)
                )
            {
                tag = tl.CurrentToken.Code;
                tl.Consume();
                tl.Consume();
            }

            // Try parse expression statement and save it to an ExpressionSector.
            ASTParser_ExpressionStatement exprStmtParser = new ASTParser_ExpressionStatement();
            var expr = exprStmtParser.Parse(tl);

            var secExpr = new ExpressionSector(InCodeLine, expr, tag);
            return secExpr;
        }
    }

}
