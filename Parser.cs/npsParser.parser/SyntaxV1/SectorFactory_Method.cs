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
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            // all method sectors start with a '+'.
            if (!InCodesWithoutIndent.StartsWith("+"))
            { return null; }

            string codesWithoutTags = InCodesWithoutIndent.Substring(1);

            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutTags, ref tokens);

            // Handle empty-type functions
            // + {Name}({Param})
            //
            if (codesWithoutTags.StartsWith(" "))
            {
                var defParser = new analysis.ASTParser_StatementDef(
                    analysis.ASTParser_StatementDef.EDefType.Function
                    , analysis.ASTParser_StatementDef.ESyntaxType.EmptyStartType
                    );
                var tl = new TokenList(tokens);
                var funcDef = defParser.Parse(tl) as analysis.STNode_FunctionDef;
                if (funcDef != null)
                {
                    return new FunctionSector(tokens.ToArray(), funcDef);
                }
            }
            else
            {
                // Try use DefParser in Pre-Type mode to handle handle function-defs with return values:
                // +{ReturnType} {Name} |= {Expr}|
                // +{ReturnType} {Name}({Param}...)} |= {Expr}|
                //
                try
                {
                    var preTypeParser = new analysis.ASTParser_StatementDef(
                        analysis.ASTParser_StatementDef.EDefType.Function
                        , analysis.ASTParser_StatementDef.ESyntaxType.ParseStartType
                        );
                    var tl = new TokenList(tokens);
                    var funcDef = preTypeParser.Parse(tl) as analysis.STNode_FunctionDef;
                    if (funcDef != null)
                    {
                        return new FunctionSector(tokens.ToArray(), funcDef);
                    }
                }
                catch
                {
                }

                // Try use DefParser in Post-Type mode to handle function-defs with non return values
                // +{Name} |= {Expr}|
                // +{Name}({Param}...)} |= {Expr}|
                // +{ Name}:{ReturnType} |= {Expr}|
                // +{ Name}({Param}...)}:{ReturnType} |= {Expr}|
                // 
                {
                    var postTypeParser = new analysis.ASTParser_StatementDef(
                        analysis.ASTParser_StatementDef.EDefType.Function
                        , analysis.ASTParser_StatementDef.ESyntaxType.ParsePostType
                        );
                    var tl = new TokenList(tokens);
                    var funcDef = postTypeParser.Parse(tl) as analysis.STNode_FunctionDef;
                    if (funcDef != null)
                    {
                        return new FunctionSector(tokens.ToArray(), funcDef);
                    }
                }

            }

            return null;
        }

    }

}
