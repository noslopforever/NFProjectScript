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
            if (InTokenList.CheckToken(ETokenType.ID))
            {
                // Handle function name
                // -n getSth(InParam0, InParam1)
                //    ^----^
                //
                var funcNameToken = InTokenList.CurrentToken;
                InTokenList.Consume();

                // Handle parameter lists if have.
                // -n getSth(InParam0, InParam1)
                //          ^------------------^
                //
                var paramsParser = new ASTParser_BlockParamList();
                var paramDefStartToken = InTokenList.CurrentToken;
                var paramDefs = paramsParser.Parse(InTokenList);

                // return the result function.
                STNode_FunctionDef funcDef = new STNode_FunctionDef(funcNameToken.Code, paramDefs);
                return funcDef;
            }

            return null;
        }

    }
}