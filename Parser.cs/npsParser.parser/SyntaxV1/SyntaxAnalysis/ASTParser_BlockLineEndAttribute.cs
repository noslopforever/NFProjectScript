using nf.protoscript.parser.token;
using System;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse attributes which appears after a def but before eol:
    /// unit Arrow @massunit @fakeunit
    ///            ^-------^
    /// </summary>
    class ASTParser_BlockLineEndAttribute
        : ASTParser_Base<STNode_AttributeDef>
    {
        public override STNode_AttributeDef Parse(TokenList InTokenList)
        {
            throw new System.NotImplementedException();
        }
    }


    /// <summary>
    /// Parser to parse all line-end attributes:
    /// unit Arrow @massunit @fakeunit
    ///            ^-----------------^
    /// </summary>
    class ASTParser_BlockLineEndAttributes
        : ASTParser_Base<STNode_AttributeDefs>
    {
        public override STNode_AttributeDefs Parse(TokenList InTokenList)
        {
            if (InTokenList.CheckToken(ETokenType.At))
            {
                // TODO impl
                throw new NotImplementedException();
                return null;
            }

            throw new System.NotImplementedException();
        }
    }

}