using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse attributes.
    /// 
    /// -n [Min=0][Max=100] HP
    ///    ^--------------^
    ///    
    /// </summary>
    internal class ASTParser_BlockAttributes
        : ASTParser_Base<STNode_AttributeDefs>
    {
        public override STNode_AttributeDefs Parse(TokenList InTokenList)
        {
            if (!InTokenList.CheckToken(ETokenType.OpenBracket))
            { return null; }

            // TODO dispatch Parser_BlockAttribute
            throw new System.NotImplementedException();
        }
    }

}