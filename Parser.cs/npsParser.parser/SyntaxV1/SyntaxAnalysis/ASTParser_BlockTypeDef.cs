using nf.protoscript.parser.token;


namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Parser to parse type-define:
    /// 
    /// HP:integer
    ///    ^-----^
    ///    
    /// </summary>
    internal class ASTParser_BlockTypeDef
        : ASTParser_Base<STNode_TypeSignature>
    {
        public override STNode_TypeSignature Parse(TokenList InTokenList)
        {
            if (!InTokenList.CheckToken(ETokenType.Colon))
            {
                return null;
            }

            InTokenList.Consume();

            var typeParser = new ASTParser_BlockType();
            return typeParser.Parse(InTokenList);
        }
    }
}