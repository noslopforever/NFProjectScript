using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// SectorFactory_Expression to parse and generate SectorExpression.
    /// </summary>
    class SectorFactory_Expression
        : SectorFactory
    {
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            // Only handle "> EXPR_STATEMENT"
            if (!InCodesWithoutIndent.StartsWith(">")
                || InCodesWithoutIndent.StartsWith(">>"))
            {
                return null;
            }

            string codesWithoutTags = InCodesWithoutIndent.Substring(1);

            // Parse expression statement from codes.
            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);
            TokenList tl = new TokenList(tokens);

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

            var secExpr = new ExpressionSector(tokens.ToArray(), InReader.CurrentCodeLine.LineNumber, expr, tag);
            return secExpr;
        }
    }

}
