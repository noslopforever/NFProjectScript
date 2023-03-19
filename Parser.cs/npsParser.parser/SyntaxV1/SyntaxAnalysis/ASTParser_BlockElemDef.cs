using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse elements.
    /// 
    /// -n [Min=0][Max=100] HP
    ///                     ^^
    ///                     
    /// </summary>
    internal class ASTParser_BlockElemDef
        : ASTParser_Base<STNode_ElementDef>
    {
        public override STNode_ElementDef Parse(TokenList InTokenList)
        {
            // Consume if the current token is ID.
            if (!InTokenList.CheckToken(ETokenType.ID))
            {
                return null;
            }

            var elemNameToken = InTokenList.CurrentToken;
            InTokenList.Consume();

            // Use the ID as the name of the result ElementDef.
            var result = new STNode_ElementDef(elemNameToken.Code);
            return result;
        }
    }
}