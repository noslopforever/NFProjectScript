using System;
using System.Collections.Generic;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser for "TERM" expressions. Always be the leaf of a handler-tree.
    /// </summary>
    class ASTParser_ExprTerm
        : ASTParser_Base<ISyntaxTreeNode>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprTerm"/> class.
        /// Since this is a leaf parser, it does not have a lower priority parser.
        /// </summary>
        public ASTParser_ExprTerm()
        {
        }

        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Handle different types of terms:
            // < Number > | < ID > | (EXPR) | [EXPRs] | { EXPR_STATEMENT }
            // EXPR (EXPRS) | EXPR [EXPRS]
            //
            // - Identifier (ID)
            // - Integer or Floating-point number
            // - String literal
            // - Parenthesized expression (EXPR)
            // - Expression list in brackets [EXPRs]
            // - Dictionary in braces { EXPR_STATEMENT }

            // <ID>
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                // Create a variable node with the ID token's string as the variable name.
                IToken idToken = InTokens[RefStartIndex];
                var idNode = new STNodeVar(idToken.Code);

                // Consume the current ID token.
                RefStartIndex++;

                return idNode;
            }
            // <Number> (Integer)
            else if (InTokens[RefStartIndex].Check(CommonTokenTypes.Integer))
            {
                // Create a constant node with the parsed integer value from the token.
                STNodeConstant stConst = new STNodeConstant(int.Parse(InTokens[RefStartIndex].Code));

                // Consume the current integer token.
                RefStartIndex++;

                return stConst;
            }
            // <Number> (Floating-point)
            else if (InTokens[RefStartIndex].Check(CommonTokenTypes.Floating))
            {
                // Create a constant node with the parsed double value from the token.
                STNodeConstant stConst = new STNodeConstant(double.Parse(InTokens[RefStartIndex].Code));

                // Consume the current floating-point token.
                RefStartIndex++;

                return stConst;
            }
            // <String>
            else if (InTokens[RefStartIndex].Check(CommonTokenTypes.String))
            {
                // Remove the surrounding quotes and create a constant node with the unquoted string.
                string codeWithoutQuote = InTokens[RefStartIndex].Code.Substring(1, InTokens[RefStartIndex].Code.Length - 2);
                STNodeConstant stConst = new STNodeConstant(codeWithoutQuote);

                // Consume the current string token.
                RefStartIndex++;
                return stConst;
            }

            // Parenthesized expression: (EXPR)
            // Example: c * (a + b)
            //              ^-----^
            else if (InTokens[RefStartIndex].Check(CommonTokenTypes.OpenParen))
            {
                // Consume the open-paren '('
                RefStartIndex++;

                // Parse the inner expression.
                ASTParser_Expression exprParser = new ASTParser_Expression();
                var subExpr = exprParser.Parse(InTokens, ref RefStartIndex);

                // Try to check for and consume the close-paren ')'.
                if (!InTokens[RefStartIndex].Check(CommonTokenTypes.CloseParen))
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokens[RefStartIndex]
                        , CommonTokenTypes.CloseParen.ToString()
                        );
                }
                // Consume the close-paren ')'
                RefStartIndex++;

                return subExpr;
            }
            // Expression list in brackets: [ EXPRs ]
            else if (InTokens[RefStartIndex].Check(CommonTokenTypes.OpenBracket))
            {
                // Parse an expression list enclosed in square brackets.
                ASTParser_BlockExpressionList exprListParser = new ASTParser_BlockExpressionList(CommonTokenTypes.OpenBracket, CommonTokenTypes.CloseBracket);
                STNodeSequence exprList = exprListParser.Parse(InTokens, ref RefStartIndex);

                return exprList;
            }
            // Constant dictionary in braces: { STATEMENT }
            else if (InTokens[RefStartIndex].Check(CommonTokenTypes.OpenBrace))
            {
                // TODO impl
                throw new NotImplementedException();
                //InTokens[RefStartIndex].Consume();

                //List<syntaxtree.STNodeBaseAssign> assignments = new List<syntaxtree.STNodeBaseAssign>();
                //ASTParser.ParseAssignments(InTokens[RefStartIndex], assignments, CommonTokenTypes.CloseBrace);

                //syntaxtree.STNodeBaseConstDict dict = new syntaxtree.STNodeBaseConstDict();
                //dict.SetAssignments(assignments);
                //if (!InTokens[RefStartIndex].EnsureOrConsumeTo(CommonTokenTypes.CloseBrace))
                //{
                //}
                //return dict;
            }

            // If none of the above cases match, throw an exception for unexpected token.
            throw new ParserException(
                ParserErrorType.AST_UnexpectedTermToken
                , InTokens[RefStartIndex]
                );
        }


    }


}