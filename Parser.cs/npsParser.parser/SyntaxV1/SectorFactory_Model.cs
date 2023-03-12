using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// Try parse codes into a sector which defines a model/class.
    /// </summary>
    public class SectorFactory_Model
        : SectorFactory
    {
        // implement abstract methods
        //
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            // Try parse codes as a Model-Type or Attribute:
            //     {BaseType} {ModelName}
            //     [{AttributeName} = {Expr}]
            List<token.Token> tokens = new List<token.Token>();
            token.TokenParser_CommonNps.Instance.ParseLine(InCodesWithoutIndent, ref tokens);

            if (tokens.Count == 2)
            {
                if (tokens[0].TokenType == token.ETokenType.ID
                    && tokens[1].TokenType == token.ETokenType.ID
                    )
                {
                    string baseTypeName = tokens[0].Code;
                    string typeName = tokens[1].Code;
                    return new ModelSector(baseTypeName, typeName);
                }
            }
            throw new NotImplementedException();
        }
    }

}
