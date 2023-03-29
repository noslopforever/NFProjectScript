using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Parser to parse inline-event-attachments
    /// 
    /// >> OnClick += cout(mousePt)
    ///            ^--------------^
    /// 
    /// </summary>
    internal class ASTParser_BlockInlineEventAttach
        : ASTParser_Base<syntaxtree.STNodeBase>
    {
        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            if (!InTokenList.CheckToken(ETokenType.Assign, "+="))
            {
                return null;
            }

            // Consume '+='
            InTokenList.Consume();

            // Take next tokens as an expression-statements.
            var exprStmtParser = new ASTParser_ExpressionStatement();
            var expr = exprStmtParser.Parse(InTokenList);
            return expr;
        }
    }
}