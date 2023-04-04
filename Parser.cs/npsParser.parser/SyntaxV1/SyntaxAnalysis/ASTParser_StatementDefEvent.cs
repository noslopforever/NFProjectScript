using nf.protoscript.parser.token;
using System;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Try parse tokens as a event-def statement.
    /// 
    /// >> OnNotify += cout("notify triggerred")
    /// >> OnClick(MousePt, Ray, Hit) += cout(MousePt)
    /// 
    /// </summary>
    class ASTParser_StatementDefEvent
        : ASTParser_Base<STNode_FunctionDef>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ASTParser_StatementDefEvent()
        {
        }

        public override STNode_FunctionDef Parse(TokenList InTokenList)
        {
            // >> OnClick(MousePt, Ray, Hit)       += cout(MousePt)
            //    ^------------------------^       ^--------------^
            //    BlockFunctionDef    [o]TypeSig   [o]EventAttach
            //
            if (!InTokenList.CheckToken(ETokenType.ID))
            {
                return null;
            }

            // Parse function def
            STNode_FunctionDef result = new ASTParser_BlockFunctionDef().Parse(InTokenList);

            // Parse inline function body (expression statement)
            StaticParseAST(new ASTParser_BlockInlineEventAttach(), InTokenList,
                expr => result._Internal_SetInitExpr(expr)
                );

            return result;
        }
    }

}