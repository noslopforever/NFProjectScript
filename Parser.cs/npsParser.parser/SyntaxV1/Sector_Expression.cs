using nf.protoscript.parser.token;
using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Expression statement sector
    /// 
    /// -n Foo()
    ///     > tag call: output("hello world")
    ///     > return 0
    /// </summary>
    public class ExpressionSector
        : Sector
    {
        public ExpressionSector(Token[] InTokens, int InLineIndex, syntaxtree.STNodeBase InExpr, string InTag = "")
            : base(InTokens)
        {
            LineIndex = InLineIndex;
            Tag = InTag;
            Expr = InExpr;
        }

        /// <summary>
        /// Line index of the expression
        /// </summary>
        public int LineIndex { get; }

        /// <summary>
        /// Tag of the statement.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Expression statement stored in the sector.
        /// </summary>
        public syntaxtree.STNodeBase Expr { get; }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            // expression sectors will be handled by it parents.
            return null;
        }

    }

}
