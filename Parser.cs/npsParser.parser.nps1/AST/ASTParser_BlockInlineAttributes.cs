using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse inline attributes.
    /// 
    /// Example:
    /// -n [Min=0][Max=100] HP
    ///    ^--------------^
    ///    
    /// </summary>
    internal class ASTParser_BlockInlineAttributes
        : ASTParser_Base<STNode_AttributeDefs>
    {
        /// <inheritdoc />
        public override STNode_AttributeDefs Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        public static STNode_AttributeDefs StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Try checking for and consuming the '[' token.
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.OpenBracket))
            { return null; }

            // TODO dispatch Parser_BlockAttribute
            throw new System.NotImplementedException();
        }

    }

}