using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

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
            new ASTParser_ExprAssign(new string[] { "=", "+=", "-=", "*=", "/=", "%=", "&=", "|=" }
            , new ASTParser_ExprOperator(new OpCodeWithDef("|", EOpFunction.Or)
            , new ASTParser_ExprOperator(new OpCodeWithDef("&", EOpFunction.And)
            , new ASTParser_ExprOperator(new OpCodeWithDef[]
            {
                new OpCodeWithDef("==", EOpFunction.Equal)
                , new OpCodeWithDef("!=", EOpFunction.NotEqual)
            }
            , new ASTParser_ExprOperator(new OpCodeWithDef[]
            { 
                new OpCodeWithDef("<", EOpFunction.LessThan)
                , new OpCodeWithDef("<=", EOpFunction.LessThanOrEqual)
                , new OpCodeWithDef(">", EOpFunction.GreaterThan)
                , new OpCodeWithDef(">=", EOpFunction.GreaterThanOrEqual)
            }
            , new ASTParser_ExprOperator(new OpCodeWithDef[]
            {
                new OpCodeWithDef("+", EOpFunction.Add)
                , new OpCodeWithDef("-", EOpFunction.Substract)
            }
            , new ASTParser_ExprOperator(new OpCodeWithDef[]
            { 
                new OpCodeWithDef("*", EOpFunction.Multiply)
                , new OpCodeWithDef("/", EOpFunction.Divide)
                , new OpCodeWithDef("%", EOpFunction.Mod)
            }
            , new ASTParser_ExprUnary(new OpCodeWithDef[]
            { 
                new OpCodeWithDef("~", EOpFunction.BitwiseNot)
                , new OpCodeWithDef("+", EOpFunction.Positive)
                , new OpCodeWithDef("-", EOpFunction.Negative)
                , new OpCodeWithDef("!", EOpFunction.Not)
            }
            , new ASTParser_ExprAccessOrCall(new ASTParser_ExprTerm())
            ))))))));

        public override syntaxtree.STNodeBase Parse(TokenList InTokenList)
        {
            return GDefaultOpExprParsers.Parse(InTokenList);
        }
    }

}