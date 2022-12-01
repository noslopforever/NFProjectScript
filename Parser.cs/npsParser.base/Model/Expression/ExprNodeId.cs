using nf.protoscript.expression;

namespace nf.protoscript.expression
{

    /// <summary>
    /// expr node: identifier.
    /// </summary>
    public class ExprNodeId : ExprNodeBase
    {
        public ExprNodeId(string InIdName)
            : base("id")
        {
            IDName = InIdName;
        }

        /// <summary>
        /// The identifier's name: variable's name, member's name, and so on...
        /// </summary>
        public string IDName { get; }

    }
}