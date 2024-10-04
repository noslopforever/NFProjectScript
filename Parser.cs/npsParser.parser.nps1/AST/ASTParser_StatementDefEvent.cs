using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse tokens as a event definition statement.
    /// 
    /// Examples:
    /// >> OnNotify += cout("notify triggerred")
    /// >> OnClick(MousePt, Ray, Hit) += cout(MousePt)
    /// 
    /// </summary>
    class ASTParser_StatementDefEvent
        : ASTParser_Base<STNode_FunctionDef>
    {

        /// <inheritdoc />
        public override STNode_FunctionDef Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // >> OnClick(MousePt, Ray, Hit)       += cout(MousePt)
            //    ^------------------------^       ^--------------^
            //    BlockFunctionDef                 [o]EventAttach

            // Check if the current token is an Identifier.
            if (!InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                return null;
            }

            // Parse the function definition part.
            STNode_FunctionDef result = ASTParser_BlockDefFunction.StaticParse(InTokens, ref RefStartIndex);

            // Try to parse the inline function body (EventAttach).
            var expr = ASTParser_BlockInlineEventAttach.StaticParse(InTokens, ref RefStartIndex);
            if (expr != null)
            {
                // Set the inline function body as the initial expression.
                result._Internal_SetInitExpr(expr);
            }

            return result;
        }
    }

}