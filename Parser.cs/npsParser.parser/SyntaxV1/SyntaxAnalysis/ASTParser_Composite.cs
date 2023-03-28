using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Base of the Parser which is composited by other parsers.
    /// </summary>
    class ASTParser_Composite<T>
        : ASTParser_Base<T>
        where T: syntaxtree.ISyntaxTreeNode
    {

        class RegEntry
        {
            public RegEntry(dynamic InParser, dynamic InSuccessAction, Func<bool> InFailAction)
            {
                Parser = InParser;
                SuccessAction = InSuccessAction;
                FailAction = InFailAction;
            }
            public dynamic Parser { get; }
            public dynamic SuccessAction { get; }
            public Func<bool> FailAction { get; }

            /// <summary>
            /// Call Parser.
            /// </summary>
            /// <param name="InTokenList"></param>
            /// <returns>
            /// If success, return true. or call FailAction to determine the return value.
            /// Return false to break the loop.
            /// </returns>
            public bool Invoke(TokenList InTokenList)
            {
                var result = Parser.Parse(InTokenList);
                if (result != null)
                {
                    SuccessAction(result);
                    return true;
                }

                if (FailAction != null)
                { return FailAction(); }

                return true;
            }
        }

        // Sub parsers sorted by order.
        private List<RegEntry> _SubParsers = new List<RegEntry>();

        // Result of the Parse method.
        protected T _Result = default(T);

        /// <summary>
        /// Add Composite Parsers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InParser"></param>
        /// <param name="InSuccessAction"></param>
        /// <param name="InFailAction"></param>
        public void AddSubParsers<U>(ASTParser_Base<U> InParser, Action<U> InSuccessAction, Func<bool> InFailAction = null)
            where U : syntaxtree.ISyntaxTreeNode
        {
            var entry = new RegEntry(InParser, InSuccessAction, InFailAction);
            _SubParsers.Add(entry);
        }

        public override T Parse(TokenList InTokenList)
        {
            for (int i = 0; i < _SubParsers.Count; i++)
            {
                if (!_SubParsers[i].Invoke(InTokenList))
                { break; }
            }
            return _Result;
        }

    }

}