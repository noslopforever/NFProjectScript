using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Provides common methods for parsing expressions in NPS (NF ProtoScript).
    /// </summary>
    public static class NPSExpressionParser
    {

        /// <summary>
        /// Parses an expression from a given code string.
        /// </summary>
        /// <param name="InCode">The code string to parse.</param>
        /// <returns>The parsed expression as an instance of <see cref="ISyntaxTreeNode"/>.</returns>
        public static ISyntaxTreeNode ParseExpression(string InCode)
        {
            // Tokenize the input code using the common NPS token parser.
            var tokens = NPSTokenizer.Instance.Tokenize(InCode);

            // Initialize an expression parser to parse the token list into an abstract syntax tree.
            ASTParser_Expression exprParser = new ASTParser_Expression();

            // Parse the token list into an expression node and return it.
            int startIndex = 0;
            var expr = exprParser.Parse(tokens, ref startIndex);
            return expr;
        }

    }

}
