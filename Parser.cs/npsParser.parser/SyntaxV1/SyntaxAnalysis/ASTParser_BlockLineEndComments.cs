using nf.protoscript.parser.token;
using System;

namespace nf.protoscript.parser.syntax1.analysis
{
    class ASTParser_BlockLineEndComments
        : ASTParser_Base<STNode_Comment>
    {
        public override STNode_Comment Parse(TokenList InTokenList)
        {
            // Try parse line-end comments finally.
            if (InTokenList.CheckToken(ETokenType.Sharp))
            {
                // TODO impl
                throw new NotImplementedException();
                return null;
            }

            throw new System.NotImplementedException();
        }
    }

}