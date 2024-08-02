using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Base class for Abstract Syntax Tree (AST) parsers.
    /// </summary>
    public abstract class ASTParserBase
    {
        /// <summary>
        /// Parses a syntax tree node from a list of tokens.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="InStartIndex">The current index in the token list.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public abstract ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex);
    }

    /// <summary>
    /// Common base class for AST parsers.
    /// </summary>
    public class ASTParserCommon : ASTParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParserCommon"/> class.
        /// </summary>
        public ASTParserCommon()
        {
        }

        /// <summary>
        /// Represents a handler for parsing conditions.
        /// </summary>
        private class Handler
        {
            /// <summary>
            /// Condition to check before parsing.
            /// </summary>
            public Func<IReadOnlyList<IToken>, int, bool> _condition;

            /// <summary>
            /// The next parser to use if the condition is met.
            /// </summary>
            public ASTParserCommon _nextParser;
        }

        /// <summary>
        /// Parses a syntax tree node from a list of tokens.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="InStartIndex">The current index in the token list.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a parser for binary expressions.
    /// </summary>
    class ASTParser_ExprOperator : ASTParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a collection of operators and a reference to the next parser.
        /// </summary>
        /// <param name="InOps">The collection of operators.</param>
        /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
        public ASTParser_ExprOperator(OpCodeWithDef[] InOps, ASTParserBase InNextExprParser)
        {
            Ops = InOps;
            _nextParser = InNextExprParser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a single operator and a reference to the next parser.
        /// </summary>
        /// <param name="InOp">The operator definition.</param>
        /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
        public ASTParser_ExprOperator(OpCodeWithDef InOp, ASTParserBase InNextExprParser)
        {
            Ops = new OpCodeWithDef[] { InOp };
            _nextParser = InNextExprParser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a token type, a single operator, and a reference to the next parser.
        /// </summary>
        /// <param name="InTokenType">The token type expected for the operators.</param>
        /// <param name="InOp">The operator definition.</param>
        /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
        public ASTParser_ExprOperator(string InTokenType, OpCodeWithDef InOp, ASTParserBase InNextExprParser)
        {
            TokenType = InTokenType;
            Ops = new OpCodeWithDef[] { InOp };
            _nextParser = InNextExprParser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a token type, a collection of operators, and a reference to the next parser.
        /// </summary>
        /// <param name="InTokenType">The token type expected for the operators.</param>
        /// <param name="InOps">The collection of operators.</param>
        /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
        public ASTParser_ExprOperator(string InTokenType, OpCodeWithDef[] InOps, ASTParserBase InNextExprParser)
        {
            TokenType = InTokenType;
            Ops = InOps;
            _nextParser = InNextExprParser;
        }

        /// <summary>
        /// Gets the operators handled by this parser.
        /// </summary>
        public OpCodeWithDef[] Ops { get; private set; }

        /// <summary>
        /// Gets the token type for the operators.
        /// </summary>
        public string TokenType { get; private set; } = "Operator";

        /// <summary>
        /// Parses an expression using the current parser.
        /// </summary>
        /// <param name="InTokens">The list of tokens representing the input.</param>
        /// <param name="InStartIndex">The starting index of the tokens to parse.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
        {
            // Try parse the left-hand side (LHS) of the expression.
            var lhs = _nextParser.Parse(InTokens, ref InStartIndex);

            // Parse and consume the operator(s).
            OpDefinition opDef = null;
            while (InStartIndex < InTokens.Count
                && string.Equals(InTokens[InStartIndex].TokenType, TokenType, StringComparison.OrdinalIgnoreCase)
                && (opDef = OpCodeWithDef.FindDefByCode(Ops, InTokens[InStartIndex].Code)) != null)
            {
                // Save, consume, then step to the next token.
                var opToken = InTokens[InStartIndex];
                InStartIndex++;

                // All operators must have a right-hand side (RHS).
                var rhs = _nextParser.Parse(InTokens, ref InStartIndex);

                // Create a binary operation node.
                lhs = new STNodeBinaryOp(opDef, lhs, rhs);
            }

            return lhs;
        }

        /// <summary>
        /// Reference to the next parser in the chain for lower precedence expressions.
        /// </summary>
        private readonly ASTParserBase _nextParser;
    }

    /// <summary>
    /// Represents a parser for unary expressions.
    /// </summary>
    class ASTParser_ExprUnary : ASTParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprUnary"/> class with a collection of unary operators and a reference to the next parser.
        /// </summary>
        /// <param name="InUnaryOps">The collection of unary operator definitions.</param>
        /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
        public ASTParser_ExprUnary(OpCodeWithDef[] InUnaryOps, ASTParserBase InNextExprParser)
        {
            UnaryOps = InUnaryOps;
            _nextParser = InNextExprParser;
        }

        /// <summary>
        /// Gets the unary operators handled by this parser.
        /// </summary>
        public OpCodeWithDef[] UnaryOps { get; private set; }

        /// <summary>
        /// Gets the token type for the unary operators.
        /// </summary>
        public string TokenType { get; private set; } = "Operator";

        /// <summary>
        /// Parses an expression using the current parser.
        /// </summary>
        /// <param name="InTokens">The list of tokens representing the input.</param>
        /// <param name="InStartIndex">The starting index of the tokens to parse.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
        {
            // Try parse and consume unary operator(s).
            List<OpDefinition> unaryTokens = new List<OpDefinition>();
            OpDefinition opDef = null;
            while (InStartIndex < InTokens.Count
                && string.Equals(InTokens[InStartIndex].TokenType, TokenType, StringComparison.OrdinalIgnoreCase)
                && (opDef = OpCodeWithDef.FindDefByCode(UnaryOps, InTokens[InStartIndex].Code)) != null)
            {
                var opToken = InTokens[InStartIndex];
                InStartIndex++;

                unaryTokens.Add(opDef);
            }

            // Parse the right-hand side (RHS) of the expression.
            var rhs = _nextParser.Parse(InTokens, ref InStartIndex);

            // Construct unary operation nodes from the last operator to the first.
            var lastNode = rhs;
            for (int i = unaryTokens.Count - 1; i >= 0; i--)
            {
                lastNode = new STNodeUnaryOp(unaryTokens[i], lastNode);
            }

            return lastNode;
        }

        /// <summary>
        /// Reference to the next parser in the chain for lower precedence expressions.
        /// </summary>
        private readonly ASTParserBase _nextParser;
    }

    /// <summary>
    /// Parser for expressions that involve access (e.g., member access) or calls (e.g., function calls).
    /// </summary>
    public class ASTParser_ExprAccessOrCall : ASTParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprAccessOrCall"/> class.
        /// </summary>
        public ASTParser_ExprAccessOrCall()
        {
        }

        /// <summary>
        /// Parses an expression that can be either an access (member access) or a call (function call).
        /// </summary>
        /// <param name="InTokens">The list of tokens representing the input to parse.</param>
        /// <param name="InStartIndex">The current index in the token list from which to start parsing.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
        {
            // Parse the left-hand side as a term.
            var termParser = new ASTParser_Term(new ASTParser_Expression());
            var lhs = termParser.Parse(InTokens, ref InStartIndex);

            // Loop to handle various scenarios such as:
            // function pointers returned by functions: getFuncPtr(0)(1)
            // 2D arrays: arr2d[0][1]
            // function array: funcArray[0]()
            // function results used as arrays: funcReturnArray()[0]
            while (InStartIndex < InTokens.Count)
            {
                bool handled = false;

                // Handle member access: <Term> . <Term>
                if (InTokens[InStartIndex].TokenType == "Operator"
                    && InTokens[InStartIndex].Code == "."
                    )
                {
                    InStartIndex++;
                    handled = true;
                    var nextTerm = termParser.Parse(InTokens, ref InStartIndex);
                    if (nextTerm is STNodeVar)
                    {
                        var memberAccess = new STNodeMemberAccess(lhs, (nextTerm as STNodeVar).IDName);
                        lhs = memberAccess;
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected token");
                    }
                }

                // Handle function call: <Term> (EXPRs)
                else if (InTokens[InStartIndex].TokenType == "Separator"
                    && InTokens[InStartIndex].Code == "("
                    )
                {
                    InStartIndex++;
                    handled = true;
                    var exprListParser = new ASTParser_ExprList(
                        new Token("Separator", ","),
                        new Token("Separator", ")")
                    );
                    var nodeSeq = exprListParser.Parse(InTokens, ref InStartIndex) as STNodeSequence;
                    var call = new STNodeCall(lhs, nodeSeq.NodeList);
                    lhs = call;
                }

                // If the current token was not handled, stop parsing.
                if (!handled)
                {
                    break;
                }
            }

            return lhs;
        }
    }
    
    /// <summary>
    /// Parser for terms.
    /// </summary>
    public class ASTParser_Term : ASTParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_Term"/> class.
        /// </summary>
        /// <param name="InExpressionParser">The parser for expressions.</param>
        public ASTParser_Term(ASTParserBase InExpressionParser)
        {
            _expressionParser = InExpressionParser;
        }

        /// <summary>
        /// Parses a syntax tree node for a term.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="InStartIndex">The current index in the token list.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
        {
            var curToken = InTokens[InStartIndex];
            switch (curToken.TokenType)
            {
                // var: Identity
                case "Identity":
                    InStartIndex++;
                    var stNodeVar = new STNodeVar(curToken.Code);
                    return stNodeVar;
                // Number: 100, 200
                case "Number":
                    InStartIndex++;
                    var stNodeConstantInt = new STNodeConstant(int.Parse(curToken.Code));
                    return stNodeConstantInt;
                // Floating Number: 0.05
                case "FloatingNumber":
                    InStartIndex++;
                    var stNodeConstantFloat = new STNodeConstant(double.Parse(curToken.Code));
                    return stNodeConstantFloat;
                // Constant String: "This is a string."
                case "String":
                    InStartIndex++;
                    string codeWithoutQuote = curToken.Code.Substring(1, curToken.Code.Length - 2);
                    var stNodeConstantString = new STNodeConstant(codeWithoutQuote);
                    return stNodeConstantString;
                default:
                    return null;
            }
        }

        private readonly ASTParserBase _expressionParser;
    }

    /// <summary>
    /// Parser for lists of expressions.
    /// </summary>
    public class ASTParser_ExprList : ASTParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprList"/> class.
        /// </summary>
        public ASTParser_ExprList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprList"/> class.
        /// </summary>
        /// <param name="InSeparateToken">The token that separates expressions.</param>
        /// <param name="InEndToken">The token that marks the end of the list.</param>
        public ASTParser_ExprList(Token InSeparateToken, Token InEndToken)
        {
            _separatorToken = InSeparateToken;
            _endToken = InEndToken;
        }

        /// <summary>
        /// Parses a syntax tree node for a sequence of expressions.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="InStartIndex">The current index in the token list.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
        {
            var exprParser = new ASTParser_Expression();
            var stNodeList = new List<ISyntaxTreeNode>();

            while (InStartIndex < InTokens.Count)
            {
                // (a, b, c,)
                //  ^  ^  ^
                var stnode = exprParser.Parse(InTokens, ref InStartIndex);
                if (stnode != null)
                {
                    stNodeList.Add(stnode);
                }

                // (a, b, c)
                //   ^  ^
                if (InTokens[InStartIndex].TokenType == _separatorToken.TokenType &&
                    InTokens[InStartIndex].Code == _separatorToken.Code)
                {
                    InStartIndex++;
                    continue;
                }
                // (a, b, c)
                //         ^
                else if (InTokens[InStartIndex].TokenType == _endToken.TokenType &&
                         InTokens[InStartIndex].Code == _endToken.Code)
                {
                    InStartIndex++;
                    return new STNodeSequence(stNodeList.ToArray());
                }
                else
                {
                    throw new ArgumentException("Unexpected Token");
                }
            }

            throw new ArgumentException("Unexpected End");
        }

        private readonly IToken _separatorToken;

        private readonly IToken _endToken;

    }

    /// <summary>
    /// Parser for expressions.
    /// </summary>
    public class ASTParser_Expression : ASTParserBase
    {
        /// <summary>
        /// Operator parsers sorted by prorities (from HIGH to LOW, more inner, more lower).
        /// TODO assign
        /// </summary>
        static ASTParserBase GDefaultOpExprParsers =
            new ASTParser_ExprOperator(new OpCodeWithDef("|", EOpFunction.Or)
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
                                        , new ASTParser_ExprAccessOrCall()
                                    )
                                )
                            )
                        )
                    )
                )
            );

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_Expression"/> class.
        /// </summary>
        public ASTParser_Expression()
        {
        }

        /// <summary>
        /// Parses a syntax tree node for an expression.
        /// </summary>
        /// <param name="InTokens">The list of tokens to parse.</param>
        /// <param name="InStartIndex">The current index in the token list.</param>
        /// <returns>The parsed syntax tree node.</returns>
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
        {
            var exprNode = GDefaultOpExprParsers.Parse(InTokens, ref InStartIndex);
            return exprNode;
        }
    }
}