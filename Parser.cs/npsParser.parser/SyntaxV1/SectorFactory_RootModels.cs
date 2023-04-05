using System;
using System.Collections.Generic;
using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// Try parse codes into a sector which defines a model/class.
    /// </summary>
    public class SectorFactory_RootModels
        : SectorFactory
    {
        // implement abstract methods
        //
        protected override Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent)
        {
            // $ID:     Singleton
            // ID:      non-base class
            // $ID ID:  Global object.
            // ID $ID:  Special object template.
            // ID ID:   Model definition.
            // $ID $ID: <ERROR>
            //

            List<Token> tokens = new List<Token>();
            TokenParser_CommonNps.Instance.ParseLine(InCodesWithoutIndent, ref tokens);

            TokenList tl = new TokenList(tokens);
            var sector = _ParseModelSector(InCodeLine, tl);
            
            // If parsed a sector, try parse line-end attributes and check line-end.
            if (sector != null)
            {
                // Try parse line-end attributes.
                ParseHelper.TryParseLineEndBlocks(tl, (attrs, comments) =>
                {
                    sector._SetAttributes(attrs);
                    sector._SetComment(comments);
                });

                ParseHelper.CheckFinishedAndThrow(tl, InCodeLine);
            }

            return sector;
        }

        /// <summary>
        /// Parse the model sector.
        /// </summary>
        /// <param name="InReader"></param>
        /// <param name="InTokenList"></param>
        /// <returns></returns>
        private Sector _ParseModelSector(CodeLine InCodeLine, TokenList InTokenList)
        {
            if (!InTokenList.CheckToken(ETokenType.ID))
            {
                return null;
            }
            if (InTokenList.CheckNextToken(ETokenType.ID))
            {
                // Read the first token.
                string str0 = InTokenList.Consume().Code;
                // Read the second token.
                string str1 = InTokenList.Consume().Code;

                // $ID ID:  Global object.
                // ID $ID:  Special object template.
                // ID ID:   Model definition.
                // $ID $ID: <ERROR>

                bool dollar0 = str0.StartsWith('$');
                bool dollar1 = str1.StartsWith('$');

                // $ID ID:  Global object.
                if (dollar0 && !dollar1)
                {
                    // Global object
                    string typename = str0.Substring(1);
                    string objectName = str1;
                    var sector = new GlobalSector(InCodeLine, typename, objectName);
                    return sector;
                }
                // ID $ID:  Special object template.
                else if (!dollar0 && dollar1)
                {
                    // Special object template
                    // TODO impl.
                    throw new NotImplementedException();
                }
                // ID ID:   Model definition.
                else if (!dollar0 && !dollar1)
                {
                    string baseTypeName = str0;
                    string typeName = str1;
                    var model = new ModelSector(InCodeLine, baseTypeName, typeName);
                    return model;
                }
            }
            // $ID:     Singleton
            // ID:      Template
            else
            {
                string str0 = InTokenList.Consume().Code;
                bool dollar0 = str0.StartsWith('$');

                // $ID:     Singleton
                if (dollar0)
                {
                    string singletonName = str0;

                    var sector = new SingletonSector(InCodeLine, singletonName);
                    return sector;
                }
                // ID:      non-base class
                else
                {
                    string baseTypeName = "";
                    string typeName = str0;
                    var model = new ModelSector(InCodeLine, baseTypeName, typeName);
                    return model;
                }
            } // End else (InTokenList.CheckNextToken(ID))

            return null;
        }

    }

}
