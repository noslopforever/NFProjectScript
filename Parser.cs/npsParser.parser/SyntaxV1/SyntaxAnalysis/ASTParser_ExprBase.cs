using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// To parse tokens into ST-Nodes
    /// </summary>
    abstract class ASTParser_ExprBase
        : ASTParser_Base<syntaxtree.STNodeBase>
    {
        public ASTParser_ExprBase(ASTParser_ExprBase InNextExprParser)
        {
            NextParser = InNextExprParser;
        }

        /// <summary>
        /// The parser which has lower priority than this parser.
        /// </summary>
        public ASTParser_ExprBase NextParser { get; private set; }

    }

}