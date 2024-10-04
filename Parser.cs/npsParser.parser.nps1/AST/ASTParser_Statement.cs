using nf.protoscript.syntaxtree;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    using KeywordParserDict = Dictionary<string, ASTParser_Base>;

    /// <summary>
    /// Parser to parse tokens as a single statement.
    /// 
    /// This can include control flow statements (e.g., return, if, while, for) or common expressions.
    /// </summary>
    class ASTParser_Statement
        : ASTParser_Base<ISyntaxTreeNode>
    {
        // A dictionary that maps keywords to their respective parsers.
        static KeywordParserDict GKeyworldParsers = new KeywordParserDict()
        {
            {"if", new ASTParser_StatementIf() },
            {"elif", new ASTParser_StatementElif() },
            {"else", new ASTParser_StatementElse() },
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

        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return StaticParse(InTokens, ref RefStartIndex);
        }

        /// <summary>
        /// Static method to parse the tokens into a statement.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="RefStartIndex">A reference to the current index in the token list. Updated as tokens are consumed.</param>
        /// <returns>The parsed statement node, or null if parsing fails.</returns>
        public static ISyntaxTreeNode StaticParse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Handle keywords (e.g., if, do, while, for, foreach, local)
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                var idToken = InTokens[RefStartIndex];
                string idStrLower = idToken.Code.ToLower();

                // Check if the keyword is in the dictionary and get the corresponding parser.
                ASTParser_Base parser = null;
                if (GKeyworldParsers.TryGetValue(idStrLower, out parser))
                {
                    // Parse the statement using the appropriate parser.
                    return parser.ParseTokens(InTokens, ref RefStartIndex);
                }
            }

            // If no keyword is found, dispatch to the expression parser to handle common expressions.
            ASTParser_Expression exprParser = new ASTParser_Expression();
            return exprParser.Parse(InTokens, ref RefStartIndex);
        }
    }

}