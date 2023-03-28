using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Base class of all ASTParsers.
    /// </summary>
    abstract class ASTParser_Base
    {

        /// <summary>
        /// Helper function to simplify call sub-parsers.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="InParser"></param>
        /// <param name="InTokenList"></param>
        /// <param name="InSuccessAction"></param>
        /// <param name="InNullAction"></param>
        /// <param name="InExAction"></param>
        public static void StaticParseAST<U>(ASTParser_Base<U> InParser, TokenList InTokenList, Action<U> InSuccessAction, Action InNullAction = null, Action<Exception> InExAction = null)
            where U : syntaxtree.ISyntaxTreeNode
        {
            U result = default(U);
            try
            {
                result = InParser.Parse(InTokenList);
            }
            catch (Exception ex)
            {
                if (InExAction != null) { InExAction(ex); }
                else { throw ex; }
            }

            if (result != null)
            {
                InSuccessAction(result);
            }
            else if (InNullAction != null)
            {
                InNullAction();
            }
        }

    }

    /// <summary>
    /// Base of all abstract-syntax-tree parsers.
    /// </summary>
    abstract class ASTParser_Base<T>
        : ASTParser_Base
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