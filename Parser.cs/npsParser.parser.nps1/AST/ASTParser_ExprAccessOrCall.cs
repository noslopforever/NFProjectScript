using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// A parser for handling expression access, collecting access, and function call operations.
    /// This class is responsible for parsing expressions that involve accessing members of an object,
    /// calling functions, or accessing elements in a collection.
    /// </summary>
    /// <example>
    /// A.B (member access)
    /// Sqrt(100) (function call)
    /// Items[0, 1] (collection access)
    /// A.B.foo(C.D.E, F)[0, G.H].I (complex expression with multiple accesses and calls)
    /// </example>
    class ASTParser_ExprAccessOrCall
        : ASTParser_ExprBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="ASTParser_ExprAccessOrCall"/> class.
        /// </summary>
        /// <param name="InLowerPriorityParser">The parser with a lower priority. See <see cref="LowerPriorityParser"/> for more informations. </param>
        public ASTParser_ExprAccessOrCall(ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
            : base(InLowerPriorityParser)
        {
        }

        /// <inheritdoc />
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Attempt to parse the left-hand side (LHS) of the expression.
            var lhs = LowerPriorityParser.Parse(InTokens, ref RefStartIndex);

            // Continuously parse SUB (member access), CALL (function call), and COLL (collection access) operations
            // until no more of these operations are found.
            while (true)
            {
                // Handle SUB (member access): <Term> . <Term>
                if (InTokens[RefStartIndex].Check(CommonTokenTypes.Operator, "."))
                {
                    // Consume the current '.' and move to the next token.
                    RefStartIndex++;

                    // Try to parse the next term using the LowerPriorityParser.
                    var curToken = InTokens[RefStartIndex];
                    var nextTerm = LowerPriorityParser.Parse(InTokens, ref RefStartIndex);

                    // The next term should be parsed as an STNodeVar, which represents a variable or member name.
                    // This is the only expected and correct case for member access.
                    if (nextTerm is STNodeVar)
                    {
                        // Create a member access node and update LHS to point to the new node.
                        var memberAccess = new syntaxtree.STNodeMemberAccess(lhs, (nextTerm as syntaxtree.STNodeVar).IDName);
                        lhs = memberAccess;
                    }
                    else
                    {
                        // Throw an exception if the next term is not an ID token or cannot be parsed into a STNodeVar.
                        throw new ParserException(
                            ParserErrorType.AST_UnexpectedToken
                            , curToken
                            , CommonTokenTypes.ID
                            );
                    }
                }
                // Handle CALL (function call):  <Term> (EXPRs)
                else if (InTokens[RefStartIndex].Check(CommonTokenTypes.OpenParen))
                {
                    // Parse the argument list inside the parentheses.
                    var exprListParser = new ASTParser_BlockExpressionList(CommonTokenTypes.OpenParen, CommonTokenTypes.CloseParen);
                    var stnodeSeq = exprListParser.Parse(InTokens, ref RefStartIndex);
                    if (stnodeSeq == null)
                    {
                        return null;
                    }

                    // Create a function call node and update LHS to point to the new node.
                    var call = new syntaxtree.STNodeCall(lhs, stnodeSeq.NodeList);
                    lhs = call;
                }
                // Handle COLL (Collection access): <Term> [EXPRs]
                else if (InTokens[RefStartIndex].Check(CommonTokenTypes.OpenBracket))
                {
                    // Parse the index list inside the brackets.
                    var exprListParser = new ASTParser_BlockExpressionList(CommonTokenTypes.OpenBracket, CommonTokenTypes.CloseBracket);
                    var stnodeSeq = exprListParser.Parse(InTokens, ref RefStartIndex);
                    if (stnodeSeq == null)
                    {
                        return null;
                    }

                    // Create a collection access node and update LHS to point to the new node.
                    var accessColl = new syntaxtree.STNodeCollectionAccess(lhs, stnodeSeq.NodeList);
                    lhs = accessColl;
                }
                else
                {
                    // Break the loop if no more operations are found.
                    break;
                }
            }

            return lhs;
        }

    }


}