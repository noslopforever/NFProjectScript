using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// Try parse codes into a sector which describes a global-object/singleton.
    /// </summary>
    public class SectorFactory_SingletonOrGlobal
        : SectorFactory
    {

        // implement abstract methods
        //
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            if (InCodesWithoutIndent[0] != '$')
            {
                return null;
            }
            string codesWithoutDollar = InCodesWithoutIndent.Substring(1);

            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(codesWithoutDollar, ref tokens);
            if (tokens.Count == 1)
            {
                if (tokens[0].TokenType == ETokenType.ID)
                {
                    string singletonName = tokens[0].Code;
                    var sector = new SingletonSector(InReader.CurrentCodeLine, singletonName);
                    return sector;
                }
                else
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
            }
            else if (tokens.Count == 2)
            {
                if (tokens[0].TokenType == ETokenType.ID
                    && tokens[1].TokenType == ETokenType.ID
                    )
                {
                    string typename = tokens[0].Code;
                    string objectName = tokens[1].Code;
                    var sector = new GlobalSector(InReader.CurrentCodeLine, typename, objectName);
                    return sector;
                }
                else
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
            }

            // TODO log error
            return null;
        }

    }

}
