using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Parse tokens to if statement.
    /// </summary>
    internal class ASTParser_StatementIf
        : ASTParser_Base<STNodeIf>
    {
        public override STNodeIf Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID , "if"))
            {
                if (InTokens[RefStartIndex].Code.ToLower() != "if")
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokens[RefStartIndex]
                        , "ID:If"
                        );
                }

                // Consume the "if" keyword.
                RefStartIndex ++;

                // Parse other codes as an boolean expression.
                ASTParser_Expression exprParser = new ASTParser_Expression();
                var expr = exprParser.Parse(InTokens, ref RefStartIndex);

                // Create If Node and try gather sub-expressions and else expressions.
                var ifNode = new syntaxtree.STNodeIf(expr);

                // Try parse next tokens as attributes + comments
                ASTParser_BlockLineEndAttributes inlineAttrParser = new ASTParser_BlockLineEndAttributes();

                throw new NotImplementedException();
                //ASTParser_StatementElif elifParser = new ASTParser_StatementElif(ifNode);
                //InContext.PendingParseSubLines(
                //    subSeqs => ifNode.SetThenSequence(subSeqs)
                //    );
                //InContext.PendingParseNextLine(elifParser
                //    , 
                //    );

            }

            return null;
        }
    }

    /// <summary>
    /// Parse tokens to if statement.
    /// </summary>
    internal class ASTParser_StatementElif
        : ASTParser_Base<ISyntaxTreeNode>
    {
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                if (InTokens[RefStartIndex].Code.ToLower() != "elif")
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokens[RefStartIndex]
                        , "ID:If"
                        );
                }
                // Consume the "elif" keyword.
                ++RefStartIndex;

                // Parse other codes as an boolean expression.
                ASTParser_Expression exprParser = new ASTParser_Expression();
                var expr = exprParser.Parse(InTokens, ref RefStartIndex);

                // TODO register to the exist STNodeIf
                throw new NotImplementedException();
            }

            return null;
        }
    }

    /// <summary>
    /// Parse tokens to if statement.
    /// </summary>
    internal class ASTParser_StatementElse
        : ASTParser_Base<ISyntaxTreeNode>
    {
        public override ISyntaxTreeNode Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            if (InTokens[RefStartIndex].Check(CommonTokenTypes.ID))
            {
                if (InTokens[RefStartIndex].Code.ToLower() != "if")
                {
                    throw new ParserException(
                        ParserErrorType.AST_UnexpectedToken
                        , InTokens[RefStartIndex]
                        , "ID:If"
                        );
                }
                // Consume the "if" keyword.
                ++RefStartIndex;

                // Parse other codes as an boolean expression.
                ASTParser_Expression exprParser = new ASTParser_Expression();
                var expr = exprParser.Parse(InTokens, ref RefStartIndex);

                // TODO register to the exist STNodeIf
                throw new NotImplementedException();
            }

            return null;
        }
    }

}