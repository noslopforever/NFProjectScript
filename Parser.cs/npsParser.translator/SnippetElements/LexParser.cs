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
                if (InTokens[InStartIndex].TokenType == "Operator" || InTokens[InStartIndex].Code == ".")
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
                else if (InTokens[InStartIndex].TokenType == "Seperator" && InTokens[InStartIndex].Code == "(")
                {
                    InStartIndex++;
                    handled = true;
                    var exprListParser = new ASTParser_ExprList(
                        new Token("Seperator", ","),
                        new Token("Seperator", ")")
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
        /// <param name="InSeperateToken">The token that separates expressions.</param>
        /// <param name="InEndToken">The token that marks the end of the list.</param>
        public ASTParser_ExprList(Token InSeperateToken, Token InEndToken)
        {
            _seperatorToken = InSeperateToken;
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
                if (InTokens[InStartIndex].TokenType == _seperatorToken.TokenType &&
                    InTokens[InStartIndex].Code == _seperatorToken.Code)
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

        private readonly IToken _seperatorToken;

        private readonly IToken _endToken;

    }

    /// <summary>
    /// Parser for expressions.
    /// </summary>
    public class ASTParser_Expression : ASTParserBase
    {
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
            // TODO: Implement priority-sorted operator parsers.
            var exprParser = new ASTParser_ExprAccessOrCall();
            var exprNode = exprParser.Parse(InTokens, ref InStartIndex);
            return exprNode;
        }
    }
}