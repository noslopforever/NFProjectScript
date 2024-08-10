using nf.protoscript.parser.syntax1;
using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.parser.SyntaxV1
{

    /// <summary>
    /// Provides common methods for parsing expressions in NPS (NF ProtoScript).
    /// </summary>
    public static class ExpressionParser_CommonNps
    {

        /// <summary>
        /// Parses an expression from a given code string.
        /// </summary>
        /// <param name="InCode">The code string to parse.</param>
        /// <returns>The parsed expression as an instance of <see cref="ISyntaxTreeNode"/>.</returns>
        public static ISyntaxTreeNode ParseExpression(string InCode)
        {
            // Initialize an empty string to hold any comments extracted during tokenization.
            string comments = "";

            // Tokenize the input code using the common NPS token parser.
            var tokens = TokenParser_CommonNps.Instance.ParseLine(InCode, out comments);
            TokenList tl = new TokenList(tokens);

            // Initialize an expression parser to parse the token list into an abstract syntax tree.
            ASTParser_Expression exprStmtParser = new ASTParser_Expression();

            // Parse the token list into an expression node and return it.
            var expr = exprStmtParser.Parse(tl);
            return expr;
        }

    }



    ///// <summary>
    ///// Represents a token in the parsing process.
    ///// </summary>
    //public interface IToken
    //{
    //    /// <summary>
    //    /// Gets the type of the token, such as Identity, Number, FloatingNumber, Operator, etc.
    //    /// </summary>
    //    string TokenType { get; }

    //    /// <summary>
    //    /// Gets the string representation of the token.
    //    /// </summary>
    //    string Code { get; }

    //    /// <summary>
    //    /// Gets debugging information including the filename, line, and column of the token.
    //    /// </summary>
    //    string DebugOutput { get; }
    //}

    ///// <summary>
    ///// Default implementation of a token.
    ///// </summary>
    //public class Token : IToken
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="Token"/> class.
    //    /// </summary>
    //    /// <param name="InType">The type of the token.</param>
    //    /// <param name="InCode">The string representation of the token.</param>
    //    /// <param name="InDebugText">Optional debugging information about the token.</param>
    //    public Token(string InType, string InCode, string InDebugText = "")
    //    {
    //        TokenType = InType;
    //        Code = InCode;
    //        DebugOutput = InDebugText;
    //    }

    //    // Begin IToken implementation
    //    public string TokenType { get; }
    //    public string Code { get; }
    //    public string DebugOutput { get; }
    //    // End IToken implementation
    //}

    ///// <summary>
    ///// Interface for parsing a token from a string.
    ///// </summary>
    //public interface ITokenParser
    //{
    //    /// <summary>
    //    /// Parses a token from the specified code starting at the given index.
    //    /// </summary>
    //    /// <param name="InCode">The input code to parse.</param>
    //    /// <param name="InStartIndex">The starting index in the input code.</param>
    //    /// <returns>The parsed token or null if no token is found.</returns>
    //    IToken Parse(string InCode, int InStartIndex);
    //}

    ///// <summary>
    ///// Token parser that uses regular expressions to identify tokens.
    ///// </summary>
    //public class TokenParserRegex : ITokenParser
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="TokenParserRegex"/> class.
    //    /// </summary>
    //    /// <param name="InRegexPattern">The regular expression pattern to match tokens.</param>
    //    /// <param name="InTokenType">The type of token to return when a match is found.</param>
    //    public TokenParserRegex(string InRegexPattern, string InTokenType)
    //    {
    //        _regex = new Regex(InRegexPattern, RegexOptions.Compiled);
    //        _tokenType = InTokenType;
    //    }

    //    // Begin ITokenParser implementation
    //    public IToken Parse(string InCode, int InStartIndex)
    //    {
    //        var match = _regex.Match(InCode, InStartIndex);
    //        if (!match.Success || match.Index != InStartIndex)
    //        {
    //            return null;
    //        }

    //        var result = new Token(_tokenType, match.Value, $"{match.Index}:{match.Index + match.Length}");
    //        return result;
    //    }
    //    // End ITokenParser implementation

    //    private readonly Regex _regex;
    //    private readonly string _tokenType;

    //}

    ////public class TokenParserStartEndChar
    ////    : ITokenParser
    ////{
    ////    public TokenParserStartEndChar(char[] InStartChar, char[] InEndChar)
    ////    {
    ////    }
    ////    public TokenParserStartEndChar(Func<char, bool> InStartCharChecker, Func<char, bool> InEndCharChecker)
    ////    {
    ////    }
    ////    public TokenParserStartEndChar(Func<char, bool> InEndCharChecker, char[] InEndChar)
    ////    {
    ////    }
    ////    public TokenParserStartEndChar(char[] InStartChar, Func<char, bool> InEndCharChecker)
    ////    {
    ////    }
    ////    Func<char, bool> StartCharChecker { get; }
    ////    Func<char, bool> EndCharChecker { get; }
    ////    public IToken Parse(string InCode, int InStartIndex, int InEndIndex)
    ////    {
    ////        throw new NotImplementedException();
    ////    }
    ////}

    ///// <summary>
    ///// A tokenizer that parses a string into a list of tokens using a collection of parsers.
    ///// </summary>
    //public class Tokenizer
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="Tokenizer"/> class.
    //    /// </summary>
    //    /// <param name="InParsers">An array of parsers to use for tokenization.</param>
    //    public Tokenizer(params ITokenParser[] InParsers)
    //    {
    //        Parsers = InParsers;
    //    }

    //    /// <summary>
    //    /// Gets the parsers used to analyze each token.
    //    /// </summary>
    //    private ITokenParser[] Parsers { get; }

    //    /// <summary>
    //    /// Tokenizes the input code into a list of tokens.
    //    /// </summary>
    //    /// <param name="InCode">The input code to tokenize.</param>
    //    /// <returns>A read-only list of tokens.</returns>
    //    /// <exception cref="ArgumentException">Thrown when an unknown character is encountered.</exception>
    //    public IReadOnlyList<IToken> Tokenize(string InCode)
    //    {
    //        var tokens = new List<IToken>();
    //        int index = 0;

    //        while (index < InCode.Length)
    //        {
    //            bool tokenFound = false;

    //            // Attempt to parse the next token using each parser.
    //            foreach (var parser in Parsers)
    //            {
    //                try
    //                {
    //                    var token = parser.Parse(InCode, index);
    //                    if (token == null)
    //                    {
    //                        continue;
    //                    }
    //                    if (!string.IsNullOrEmpty(token.TokenType))
    //                    {
    //                        tokens.Add(token);
    //                    }
    //                    index += token.Code.Length;
    //                    tokenFound = true;
    //                    break;
    //                }
    //                catch (ArgumentException)
    //                {
    //                    // Skip this parser and try the next one.
    //                }
    //            }

    //            if (!tokenFound)
    //            {
    //                throw new ArgumentException($"Unknown character at position {index}: '{InCode[index]}'", nameof(InCode));
    //            }
    //        }

    //        return tokens.AsReadOnly();
    //    }
    //}


    ///// <summary>
    ///// Base class for Abstract Syntax Tree (AST) parsers.
    ///// </summary>
    //public abstract class ASTParserBase
    //{
    //    /// <summary>
    //    /// Parses a syntax tree node from a list of tokens.
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens to parse.</param>
    //    /// <param name="InStartIndex">The current index in the token list.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public abstract ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex);
    //}

    ///// <summary>
    ///// Common base class for AST parsers.
    ///// </summary>
    //public class ASTParserCommon : ASTParserBase
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParserCommon"/> class.
    //    /// </summary>
    //    public ASTParserCommon()
    //    {
    //    }

    //    /// <summary>
    //    /// Represents a handler for parsing conditions.
    //    /// </summary>
    //    private class Handler
    //    {
    //        /// <summary>
    //        /// Condition to check before parsing.
    //        /// </summary>
    //        public Func<IReadOnlyList<IToken>, int, bool> _condition;

    //        /// <summary>
    //        /// The next parser to use if the condition is met.
    //        /// </summary>
    //        public ASTParserCommon _nextParser;
    //    }

    //    /// <summary>
    //    /// Parses a syntax tree node from a list of tokens.
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens to parse.</param>
    //    /// <param name="InStartIndex">The current index in the token list.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    ///// <summary>
    ///// Represents a parser for binary expressions.
    ///// </summary>
    //class ASTParser_ExprOperator : ASTParserBase
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a collection of operators and a reference to the next parser.
    //    /// </summary>
    //    /// <param name="InOps">The collection of operators.</param>
    //    /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
    //    public ASTParser_ExprOperator(OpCodeWithDef[] InOps, ASTParserBase InNextExprParser)
    //    {
    //        Ops = InOps;
    //        _nextParser = InNextExprParser;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a single operator and a reference to the next parser.
    //    /// </summary>
    //    /// <param name="InOp">The operator definition.</param>
    //    /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
    //    public ASTParser_ExprOperator(OpCodeWithDef InOp, ASTParserBase InNextExprParser)
    //    {
    //        Ops = new OpCodeWithDef[] { InOp };
    //        _nextParser = InNextExprParser;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a token type, a single operator, and a reference to the next parser.
    //    /// </summary>
    //    /// <param name="InTokenType">The token type expected for the operators.</param>
    //    /// <param name="InOp">The operator definition.</param>
    //    /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
    //    public ASTParser_ExprOperator(string InTokenType, OpCodeWithDef InOp, ASTParserBase InNextExprParser)
    //    {
    //        TokenType = InTokenType;
    //        Ops = new OpCodeWithDef[] { InOp };
    //        _nextParser = InNextExprParser;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprOperator"/> class with a token type, a collection of operators, and a reference to the next parser.
    //    /// </summary>
    //    /// <param name="InTokenType">The token type expected for the operators.</param>
    //    /// <param name="InOps">The collection of operators.</param>
    //    /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
    //    public ASTParser_ExprOperator(string InTokenType, OpCodeWithDef[] InOps, ASTParserBase InNextExprParser)
    //    {
    //        TokenType = InTokenType;
    //        Ops = InOps;
    //        _nextParser = InNextExprParser;
    //    }

    //    /// <summary>
    //    /// Gets the operators handled by this parser.
    //    /// </summary>
    //    public OpCodeWithDef[] Ops { get; private set; }

    //    /// <summary>
    //    /// Gets the token type for the operators.
    //    /// </summary>
    //    public string TokenType { get; private set; } = "Operator";

    //    /// <summary>
    //    /// Parses an expression using the current parser.
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens representing the input.</param>
    //    /// <param name="InStartIndex">The starting index of the tokens to parse.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
    //    {
    //        // Try parse the left-hand side (LHS) of the expression.
    //        var lhs = _nextParser.Parse(InTokens, ref InStartIndex);

    //        // Parse and consume the operator(s).
    //        OpDefinition opDef = null;
    //        while (InStartIndex < InTokens.Count
    //            && string.Equals(InTokens[InStartIndex].TokenType, TokenType, StringComparison.OrdinalIgnoreCase)
    //            && (opDef = OpCodeWithDef.FindDefByCode(Ops, InTokens[InStartIndex].Code)) != null)
    //        {
    //            // Save, consume, then step to the next token.
    //            var opToken = InTokens[InStartIndex];
    //            InStartIndex++;

    //            // All operators must have a right-hand side (RHS).
    //            var rhs = _nextParser.Parse(InTokens, ref InStartIndex);

    //            // Create a binary operation node.
    //            lhs = new STNodeBinaryOp(opDef, lhs, rhs);
    //        }

    //        return lhs;
    //    }

    //    /// <summary>
    //    /// Reference to the next parser in the chain for lower precedence expressions.
    //    /// </summary>
    //    private readonly ASTParserBase _nextParser;
    //}

    ///// <summary>
    ///// Represents a parser for unary expressions.
    ///// </summary>
    //class ASTParser_ExprUnary : ASTParserBase
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprUnary"/> class with a collection of unary operators and a reference to the next parser.
    //    /// </summary>
    //    /// <param name="InUnaryOps">The collection of unary operator definitions.</param>
    //    /// <param name="InNextExprParser">The next parser in the chain for lower precedence expressions.</param>
    //    public ASTParser_ExprUnary(OpCodeWithDef[] InUnaryOps, ASTParserBase InNextExprParser)
    //    {
    //        UnaryOps = InUnaryOps;
    //        _nextParser = InNextExprParser;
    //    }

    //    /// <summary>
    //    /// Gets the unary operators handled by this parser.
    //    /// </summary>
    //    public OpCodeWithDef[] UnaryOps { get; private set; }

    //    /// <summary>
    //    /// Gets the token type for the unary operators.
    //    /// </summary>
    //    public string TokenType { get; private set; } = "Operator";

    //    /// <summary>
    //    /// Parses an expression using the current parser.
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens representing the input.</param>
    //    /// <param name="InStartIndex">The starting index of the tokens to parse.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
    //    {
    //        // Try parse and consume unary operator(s).
    //        List<OpDefinition> unaryTokens = new List<OpDefinition>();
    //        OpDefinition opDef = null;
    //        while (InStartIndex < InTokens.Count
    //            && string.Equals(InTokens[InStartIndex].TokenType, TokenType, StringComparison.OrdinalIgnoreCase)
    //            && (opDef = OpCodeWithDef.FindDefByCode(UnaryOps, InTokens[InStartIndex].Code)) != null)
    //        {
    //            var opToken = InTokens[InStartIndex];
    //            InStartIndex++;

    //            unaryTokens.Add(opDef);
    //        }

    //        // Parse the right-hand side (RHS) of the expression.
    //        var rhs = _nextParser.Parse(InTokens, ref InStartIndex);

    //        // Construct unary operation nodes from the last operator to the first.
    //        var lastNode = rhs;
    //        for (int i = unaryTokens.Count - 1; i >= 0; i--)
    //        {
    //            lastNode = new STNodeUnaryOp(unaryTokens[i], lastNode);
    //        }

    //        return lastNode;
    //    }

    //    /// <summary>
    //    /// Reference to the next parser in the chain for lower precedence expressions.
    //    /// </summary>
    //    private readonly ASTParserBase _nextParser;
    //}

    ///// <summary>
    ///// Parser for expressions that involve access (e.g., member access) or calls (e.g., function calls).
    ///// </summary>
    //public class ASTParser_ExprAccessOrCall : ASTParserBase
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprAccessOrCall"/> class.
    //    /// </summary>
    //    public ASTParser_ExprAccessOrCall()
    //    {
    //    }

    //    /// <summary>
    //    /// Parses an expression that can be either an access (member access) or a call (function call).
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens representing the input to parse.</param>
    //    /// <param name="InStartIndex">The current index in the token list from which to start parsing.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
    //    {
    //        // Parse the left-hand side as a term.
    //        var termParser = new ASTParser_Term(new ASTParser_Expression());
    //        var lhs = termParser.Parse(InTokens, ref InStartIndex);

    //        // Loop to handle various scenarios such as:
    //        // function pointers returned by functions: getFuncPtr(0)(1)
    //        // 2D arrays: arr2d[0][1]
    //        // function array: funcArray[0]()
    //        // function results used as arrays: funcReturnArray()[0]
    //        while (InStartIndex < InTokens.Count)
    //        {
    //            bool handled = false;

    //            // Handle member access: <Term> . <Term>
    //            if (InTokens[InStartIndex].TokenType == "Operator"
    //                && InTokens[InStartIndex].Code == "."
    //                )
    //            {
    //                InStartIndex++;
    //                handled = true;
    //                var nextTerm = termParser.Parse(InTokens, ref InStartIndex);
    //                if (nextTerm is STNodeVar)
    //                {
    //                    var memberAccess = new STNodeMemberAccess(lhs, (nextTerm as STNodeVar).IDName);
    //                    lhs = memberAccess;
    //                }
    //                else
    //                {
    //                    throw new ArgumentException("Unexpected token");
    //                }
    //            }

    //            // Handle function call: <Term> (EXPRs)
    //            else if (InTokens[InStartIndex].TokenType == "Separator"
    //                && InTokens[InStartIndex].Code == "("
    //                )
    //            {
    //                InStartIndex++;
    //                handled = true;
    //                var exprListParser = new ASTParser_ExprList(
    //                    new Token("Separator", ","),
    //                    new Token("Separator", ")")
    //                );
    //                var nodeSeq = exprListParser.Parse(InTokens, ref InStartIndex) as STNodeSequence;
    //                var call = new STNodeCall(lhs, nodeSeq.NodeList);
    //                lhs = call;
    //            }

    //            // If the current token was not handled, stop parsing.
    //            if (!handled)
    //            {
    //                break;
    //            }
    //        }

    //        return lhs;
    //    }
    //}

    ///// <summary>
    ///// Parser for terms.
    ///// </summary>
    //public class ASTParser_Term : ASTParserBase
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_Term"/> class.
    //    /// </summary>
    //    /// <param name="InExpressionParser">The parser for expressions.</param>
    //    public ASTParser_Term(ASTParserBase InExpressionParser)
    //    {
    //        _expressionParser = InExpressionParser;
    //    }

    //    /// <summary>
    //    /// Parses a syntax tree node for a term.
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens to parse.</param>
    //    /// <param name="InStartIndex">The current index in the token list.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
    //    {
    //        var curToken = InTokens[InStartIndex];
    //        switch (curToken.TokenType)
    //        {
    //            // var: Identity
    //            case "Identity":
    //                InStartIndex++;
    //                var stNodeVar = new STNodeVar(curToken.Code);
    //                return stNodeVar;
    //            // Number: 100, 200
    //            case "Number":
    //                InStartIndex++;
    //                var stNodeConstantInt = new STNodeConstant(int.Parse(curToken.Code));
    //                return stNodeConstantInt;
    //            // Floating Number: 0.05
    //            case "FloatingNumber":
    //                InStartIndex++;
    //                var stNodeConstantFloat = new STNodeConstant(double.Parse(curToken.Code));
    //                return stNodeConstantFloat;
    //            // Constant String: "This is a string."
    //            case "String":
    //                InStartIndex++;
    //                string codeWithoutQuote = curToken.Code.Substring(1, curToken.Code.Length - 2);
    //                var stNodeConstantString = new STNodeConstant(codeWithoutQuote);
    //                return stNodeConstantString;
    //            default:
    //                return null;
    //        }
    //    }

    //    private readonly ASTParserBase _expressionParser;
    //}

    ///// <summary>
    ///// Parser for lists of expressions.
    ///// </summary>
    //public class ASTParser_ExprList : ASTParserBase
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprList"/> class.
    //    /// </summary>
    //    public ASTParser_ExprList()
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_ExprList"/> class.
    //    /// </summary>
    //    /// <param name="InSeparateToken">The token that separates expressions.</param>
    //    /// <param name="InEndToken">The token that marks the end of the list.</param>
    //    public ASTParser_ExprList(Token InSeparateToken, Token InEndToken)
    //    {
    //        _separatorToken = InSeparateToken;
    //        _endToken = InEndToken;
    //    }

    //    /// <summary>
    //    /// Parses a syntax tree node for a sequence of expressions.
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens to parse.</param>
    //    /// <param name="InStartIndex">The current index in the token list.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
    //    {
    //        var exprParser = new ASTParser_Expression();
    //        var stNodeList = new List<ISyntaxTreeNode>();

    //        while (InStartIndex < InTokens.Count)
    //        {
    //            // (a, b, c,)
    //            //  ^  ^  ^
    //            var stnode = exprParser.Parse(InTokens, ref InStartIndex);
    //            if (stnode != null)
    //            {
    //                stNodeList.Add(stnode);
    //            }

    //            // (a, b, c)
    //            //   ^  ^
    //            if (InTokens[InStartIndex].TokenType == _separatorToken.TokenType &&
    //                InTokens[InStartIndex].Code == _separatorToken.Code)
    //            {
    //                InStartIndex++;
    //                continue;
    //            }
    //            // (a, b, c)
    //            //         ^
    //            else if (InTokens[InStartIndex].TokenType == _endToken.TokenType &&
    //                     InTokens[InStartIndex].Code == _endToken.Code)
    //            {
    //                InStartIndex++;
    //                return new STNodeSequence(stNodeList.ToArray());
    //            }
    //            else
    //            {
    //                throw new ArgumentException("Unexpected Token");
    //            }
    //        }

    //        throw new ArgumentException("Unexpected End");
    //    }

    //    private readonly IToken _separatorToken;

    //    private readonly IToken _endToken;

    //}

    ///// <summary>
    ///// Parser for expressions.
    ///// </summary>
    //public class ASTParser_Expression : ASTParserBase
    //{
    //    /// <summary>
    //    /// Operator parsers sorted by prorities (from HIGH to LOW, more inner, more lower).
    //    /// TODO assign
    //    /// </summary>
    //    static ASTParserBase GDefaultOpExprParsers =
    //        new ASTParser_ExprOperator(new OpCodeWithDef("|", EOpFunction.Or)
    //            , new ASTParser_ExprOperator(new OpCodeWithDef("&", EOpFunction.And)
    //                , new ASTParser_ExprOperator(new OpCodeWithDef[]
    //                {
    //                        new OpCodeWithDef("==", EOpFunction.Equal)
    //                        , new OpCodeWithDef("!=", EOpFunction.NotEqual)
    //                }
    //                    , new ASTParser_ExprOperator(new OpCodeWithDef[]
    //                    {
    //                            new OpCodeWithDef("<", EOpFunction.LessThan)
    //                            , new OpCodeWithDef("<=", EOpFunction.LessThanOrEqual)
    //                            , new OpCodeWithDef(">", EOpFunction.GreaterThan)
    //                            , new OpCodeWithDef(">=", EOpFunction.GreaterThanOrEqual)
    //                    }
    //                        , new ASTParser_ExprOperator(new OpCodeWithDef[]
    //                        {
    //                                new OpCodeWithDef("+", EOpFunction.Add)
    //                                , new OpCodeWithDef("-", EOpFunction.Substract)
    //                        }
    //                            , new ASTParser_ExprOperator(new OpCodeWithDef[]
    //                            {
    //                                    new OpCodeWithDef("*", EOpFunction.Multiply)
    //                                    , new OpCodeWithDef("/", EOpFunction.Divide)
    //                                    , new OpCodeWithDef("%", EOpFunction.Mod)
    //                            }
    //                                , new ASTParser_ExprUnary(new OpCodeWithDef[]
    //                                {
    //                                        new OpCodeWithDef("~", EOpFunction.BitwiseNot)
    //                                        , new OpCodeWithDef("+", EOpFunction.Positive)
    //                                        , new OpCodeWithDef("-", EOpFunction.Negative)
    //                                        , new OpCodeWithDef("!", EOpFunction.Not)
    //                                }
    //                                    , new ASTParser_ExprAccessOrCall()
    //                                )
    //                            )
    //                        )
    //                    )
    //                )
    //            )
    //        );

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ASTParser_Expression"/> class.
    //    /// </summary>
    //    public ASTParser_Expression()
    //    {
    //    }

    //    /// <summary>
    //    /// Parses a syntax tree node for an expression.
    //    /// </summary>
    //    /// <param name="InTokens">The list of tokens to parse.</param>
    //    /// <param name="InStartIndex">The current index in the token list.</param>
    //    /// <returns>The parsed syntax tree node.</returns>
    //    public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int InStartIndex)
    //    {
    //        var exprNode = GDefaultOpExprParsers.Parse(InTokens, ref InStartIndex);
    //        return exprNode;
    //    }
    //}

    ///// <summary>
    ///// Provides an expression parser used in ElementExpr class.
    ///// </summary>
    //internal static class ElementExprParser
    //{

    //    /// <summary>
    //    /// Initializes the tokenizer used by the expression parser.
    //    /// </summary>
    //    static ElementExprParser()
    //    {
    //        _tokenizer = new Tokenizer(
    //            new TokenParserRegex(@"[$a-zA-Z_][a-zA-Z0-9_$]*", "Identity")
    //            , new TokenParserRegex(@"\d+", "Number")
    //            , new TokenParserRegex(@"\d*\.\d+", "FloatingNumber")
    //            , new TokenParserRegex("\"(\\\\\"|[^\"])*\"", "String")
    //            , new TokenParserRegex("'([^'])*'", "String")
    //            , new TokenParserRegex(@"[\(\)\[\],]", "Separator")
    //            , new TokenParserRegex("[!<>]=?|==", "Operator")
    //            , new TokenParserRegex(@"[\+\-\*/%=\&|.]", "Operator")
    //            , new TokenParserRegex(@"[ \t\r]+", "") // Skip whitespace
    //        );
    //    }

    //    /// <summary>
    //    /// Parses the provided code and generates a syntax tree node.
    //    /// </summary>
    //    /// <param name="InCode">The code to parse.</param>
    //    /// <returns>The generated syntax tree node.</returns>
    //    public static ISyntaxTreeNode ParseCode(string InCode)
    //    {
    //        // Tokenize the code.
    //        var tokens = _tokenizer.Tokenize(InCode);

    //        // Use the expression parser to parse the tokens.
    //        var exprParser = new ASTParser_Expression();
    //        int startIndex = 0;
    //        var stNode = exprParser.Parse(tokens, ref startIndex);

    //        return stNode;
    //    }

    //    /// <summary>
    //    /// The tokenizer used by this parser.
    //    /// </summary>
    //    private static readonly Tokenizer _tokenizer;
    //}

}
