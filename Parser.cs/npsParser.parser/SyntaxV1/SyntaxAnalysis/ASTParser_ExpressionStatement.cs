using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    using KeywordParserDict = Dictionary<string, ASTParser_Base<syntaxtree.STNodeBase>>;

    /// <summary>
    /// Parse tokens as expression statements.
    /// 
    /// Like return/if/while/for.
    ///     or common expressions.
    /// </summary>
    class ASTParser_ExpressionStatement
        : ASTParser_Base<syntaxtree.STNodeBase>
    {
        static KeywordParserDict GKeyworldParsers = new KeywordParserDict()
        {
            //{"if", new ASTParser_StatementIf() },
            //{"elif", new ASTParser_StatementElif() },
            //{"else", new ASTParser_StatementElse() },
            //{"switch", new ASTParser_StatementSwitch() },
            //{"do", new ASTParser_StatementDo() },
            //{"while", new ASTParser_StatementWhile() },
            //{"for", new ASTParser_StatementFor() },
            //{"foreach", new ASTParser_StatementForeach() },
            //{"continue", new ASTParser_StatementContinue() },
            //{"break", new ASTParser_StatementBreak() },
            {"return", new ASTParser_StatementReturn() },
            //{"new", new ASTParser_StatementNew() },
            //{"cout", new ASTParser_StatementCout() },
        };

    public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
    {
        // Handle keywords (if/do/while/for/foreach/local)
        if (InTokenList.CheckToken(ETokenType.ID))
        {
            var idToken = InTokenList.CurrentToken;
            string idStrLower = idToken.Code.ToLower();

            ASTParser_Base<syntaxtree.STNodeBase> parser = null;
            if (GKeyworldParsers.TryGetValue(idStrLower, out parser))
            {
                return parser.Parse(InTokenList);
            }
        }

        // Dispatch common expressions.
        ASTParser_Expression exprParser = new ASTParser_Expression();
        return exprParser.Parse(InTokenList);
    }
}

}