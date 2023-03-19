using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Base of all abstract-syntax-tree parsers.
    /// </summary>
    abstract class ASTParser_Base<T>
        where T : syntaxtree.ISyntaxTreeNode
    {

        /// <summary>
        /// Parse tokens into results.
        /// </summary>
        /// <param name="InTokenList"></param>
        /// <returns></returns>
        public abstract T Parse(TokenList InTokenList);

    }

}