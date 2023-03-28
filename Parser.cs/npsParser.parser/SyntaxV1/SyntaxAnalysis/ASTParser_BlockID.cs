using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Parser to parse an ID to a STNodeGetVar
    /// </summary>
    class ASTParser_BlockID
        : ASTParser_Base<syntaxtree.STNodeGetVar>
    {
        public override syntaxtree.STNodeGetVar Parse(TokenList InTokenList)
        {
            if (InTokenList.CheckToken(ETokenType.ID))
            {
                var tokenId = InTokenList.Consume();
                return new syntaxtree.STNodeGetVar(tokenId.Code);
            }
            return null;
        }
    }

}