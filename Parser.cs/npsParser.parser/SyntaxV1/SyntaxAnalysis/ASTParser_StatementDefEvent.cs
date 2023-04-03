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
            // >> OnClick(MousePt, Ray, Hit)       += cout(MousePt)     @uicommand            # comment
            //    ^------------------------^       ^--------------^     ^----------^          ^-------------^
            //    BlockFunctionDef    [o]TypeSig   [o]EventAttach      [o]LineEndAttributes   [o]LineEndComments
            //
            if (!InTokenList.CheckToken(ETokenType.ID))
            {
                return null;
            }

            // Parse function def
            STNode_FunctionDef result = new ASTParser_BlockFunctionDef().Parse(InTokenList);
            if (result == null)
            {
                // TODO log error
                throw new NotImplementedException();
            }

            // Parse inline function body (expression statement)
            StaticParseAST(new ASTParser_BlockInlineEventAttach(), InTokenList,
                expr => result._Internal_SetInitExpr(expr)
                );

            // Parse line-end blocks
            ParseHelper.TryParseLineEndBlocks(InTokenList, (attrs, comments) =>
            {
                result._Internal_AddAttributes(attrs);
                result._Internal_AddComments(comments);
            });

            // if not end, there is an unexpected token
            if (!InTokenList.IsEnd)
            {
                // TODO log error
                throw new NotImplementedException();
                return null;
            }

            return result;
        }
    }

}