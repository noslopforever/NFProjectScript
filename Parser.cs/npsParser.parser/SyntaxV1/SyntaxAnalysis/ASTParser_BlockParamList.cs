using System;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse parameter-list.
    /// 
    /// -n [Pure] getHP(InParam0, InParam1) = return 100
    ///                ^------------------^
    /// 
    /// </summary>
    class ASTParser_BlockParamList
        : ASTParser_Base<STNode_ElementDefs>
    {
        public override STNode_ElementDefs Parse(TokenList InTokenList)
        {
            STNode_ElementDefs paramDefs = new STNode_ElementDefs();

            // Handle parameter lists if have.
            // -n getSth(InParam0, InParam1)
            //          ^------------------^
            //
            if (InTokenList.CheckToken(ETokenType.OpenParen))
            {
                // consume the 'open-paren'.
                InTokenList.Consume();

                while (true)
                {
                    // -n getSth(InParam0, InParam1)
                    //                             ^
                    if (InTokenList.CheckToken(ETokenType.CloseParen))
                    { break; }

                    // If matches the end of the TL, mark failed and break.
                    if (InTokenList.IsEnd)
                    {
                        paramDefs = null;
                        break;
                    }

                    ASTParser_StatementDefMember paramDefParser = new ASTParser_StatementDefMember(null, false);
                    var paramDef = paramDefParser.Parse(InTokenList);
                    if (paramDef == null)
                    {
                        // If param parse failed, clear all params and break the loop.
                        paramDefs = null;
                        break;
                    }
                    paramDefs.Add(paramDef);

                    // -n getSth(InParam0, InParam1)
                    //                   ^         ^
                    InTokenList.EnsureOrConsumeTo(new ETokenType[] { ETokenType.Comma, ETokenType.CloseParen });
                    if (InTokenList.CheckToken(ETokenType.Comma))
                    { InTokenList.Consume(); }
                }

                InTokenList.EnsureOrConsumeTo(ETokenType.CloseParen);
                // consume the 'close-paren'.
                InTokenList.Consume();

                if (paramDefs == null)
                {
                    // TODO log error
                    throw new NotImplementedException();
                }
            }

            return paramDefs;
        }
    }
}