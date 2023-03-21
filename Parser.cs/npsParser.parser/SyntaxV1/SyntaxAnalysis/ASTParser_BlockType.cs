using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;


namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Type block parser to parse tokens to a type-signature.
    /// </summary>
    internal class ASTParser_BlockType
        : ASTParser_Base<STNode_TypeSignature>
    {
        public override STNode_TypeSignature Parse(TokenList InTokenList)
        {
            if (!InTokenList.CheckToken(ETokenType.ID))
            {
                // TODO log error
                throw new NotImplementedException();
                return null;
            }

            var typeCodeToken = InTokenList.CurrentToken;
            InTokenList.Consume();
            // TODO impl collections/ delegate-types
            //throw new NotImplementedException();

            return new STNode_TypeSignature(typeCodeToken.Code);
        }
    }
}