﻿using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// To parse tokens into ST-nodes.
    /// </summary>
    class ASTParser_Expression
        : ASTParser_Base<syntaxtree.STNodeBase>
    {
        /// <summary>
        /// Operator parsers sorted by prorities (from LOW to HIGH).
        /// </summary>
        static ASTParser_ExprBase GDefaultOpExprParsers =
            new ASTParser_ExprOperator(token.ETokenType.Assign, "="
            , new ASTParser_ExprOperator("|"
            , new ASTParser_ExprOperator("&"
            , new ASTParser_ExprOperator(new string[] { "==", "!=" }
            , new ASTParser_ExprOperator(new string[] { "<", "<=", ">", ">=" }
            , new ASTParser_ExprOperator(new string[] { "+", "-" }
            , new ASTParser_ExprOperator(new string[] { "*", "/", "%" }
            , new ASTParser_ExprUnary(new string[] { "~", "+", "-", "!" }
            , new ASTParser_ExprAccess(new ASTParser_ExprTerm())
            ))))))));
        
        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            return GDefaultOpExprParsers.Parse(InTokenList);
        }
    }

}