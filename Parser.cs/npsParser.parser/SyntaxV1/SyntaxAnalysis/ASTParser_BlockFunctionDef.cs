using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Parser to parse functions.
    /// 
    /// -n [Pure] getHP() = return 100
    ///           ^-----^
    /// 
    /// </summary>
    internal class ASTParser_BlockFunctionDef
        : ASTParser_Base<STNode_FunctionDef>
    {

        public override STNode_FunctionDef Parse(TokenList InTokenList)
        {
            if (InTokenList.CheckToken(ETokenType.ID) && InTokenList.CheckToken(ETokenType.OpenParen))
            {
                var funcNameToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                // TODO dispatch Parser_BlockFunctionParams
                throw new System.NotImplementedException();
            }

            return null;
        }

    }
}