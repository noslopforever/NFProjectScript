using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parses tokens into an expression syntax tree nodes.
    /// </summary>
    class ASTParser_Expression
        : ASTParser_Base<ISyntaxTreeNode>
    {
        /// <summary>
        /// Operator parsers sorted by priorities (from HIGH to LOW).
        /// Each parser handles specific operators and contains a lower-priority parser.
        /// This nested structure ensures that operators are parsed according to the correct precedence.
        /// </summary>

        static ASTParser_ExprBase GDefaultOpExprParsers =
            // Assignment operators (=, +=, -=, *=, /=, %=, &=, |=)
            new ASTParser_ExprAssign(new string[] { "=", "+=", "-=", "*=", "/=", "%=", "&=", "|=" }

            // Logical OR (|)
            , new ASTParser_ExprOperator(new OpCodeWithDef("|", EOpFunction.Or)

                // Logical AND (&)
                , new ASTParser_ExprOperator(new OpCodeWithDef("&", EOpFunction.And)

                    // Equality comparisons (==, !=)
                    , new ASTParser_ExprOperator(new OpCodeWithDef[] { new OpCodeWithDef("==", EOpFunction.Equal), new OpCodeWithDef("!=", EOpFunction.NotEqual) }

                        // Relational comparisons (<, <=, >, >=)
                        , new ASTParser_ExprOperator(new OpCodeWithDef[] { new OpCodeWithDef("<", EOpFunction.LessThan), new OpCodeWithDef("<=", EOpFunction.LessThanOrEqual), new OpCodeWithDef(">", EOpFunction.GreaterThan), new OpCodeWithDef(">=", EOpFunction.GreaterThanOrEqual) }

                            // Addition and subtraction (+, -)
                            , new ASTParser_ExprOperator(new OpCodeWithDef[] { new OpCodeWithDef("+", EOpFunction.Add), new OpCodeWithDef("-", EOpFunction.Substract) }

                                // Multiplication, division, and modulus (*, /, %)
                                , new ASTParser_ExprOperator(new OpCodeWithDef[] { new OpCodeWithDef("*", EOpFunction.Multiply), new OpCodeWithDef("/", EOpFunction.Divide), new OpCodeWithDef("%", EOpFunction.Mod) }

                                    // Unary operators (~, +, -, !)
                                    , new ASTParser_ExprUnary(new OpCodeWithDef[] { new OpCodeWithDef("~", EOpFunction.BitwiseNot), new OpCodeWithDef("+", EOpFunction.Positive), new OpCodeWithDef("-", EOpFunction.Negative), new OpCodeWithDef("!", EOpFunction.Not) }

                                        // member access, collecting access, and function call operations.
                                        , new ASTParser_ExprAccessOrCall(

                                            // Basic terms (identifiers, numbers, strings, etc.)
                                            new ASTParser_ExprTerm()
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );


        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            return GDefaultOpExprParsers.Parse(InTokens, ref RefStartIndex);
        }
    }

}