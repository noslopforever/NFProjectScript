using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Base class for parsing tokens into syntax tree nodes (ST-Nodes).
    /// This abstract class serves as the foundation for various expression parsers.
    /// Each derived class will implement specific parsing logic for different types of expressions.
    /// </summary>
    internal abstract class ASTParser_ExprBase : ASTParser_Base<ISyntaxTreeNode>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ASTParser_ExprBase"/> class.
        /// </summary>
        /// <param name="InLowerPriorityParser">The parser with a lower priority. See <see cref="LowerPriorityParser"/> for more informations. </param>
        public ASTParser_ExprBase(ASTParser_Base<ISyntaxTreeNode> InLowerPriorityParser)
        {
            LowerPriorityParser = InLowerPriorityParser;
        }

        /// <summary>
        /// Gets the parser with a lower priority that is used to parse the left-hand side (LHS) and right-hand side (RHS) of the expression.
        /// When parsing an expression, this parser may need to delegate the parsing of LHS and RHS to the lower priority parser.
        /// </summary>
        public ASTParser_Base<ISyntaxTreeNode> LowerPriorityParser { get; private set; }

    }
}