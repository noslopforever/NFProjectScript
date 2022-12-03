
namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// expr node: identifier.
    /// </summary>
    public class STNodeVariable : STNodeBase
    {
        public STNodeVariable(string InIdName)
            : base("id")
        {
            IDName = InIdName;
        }

        /// <summary>
        /// The identifier's name: variable's name, member's name, and so on...
        /// </summary>
        public string IDName { get; }

        ///// <summary>
        ///// The scope of this variable.
        ///// </summary>
        //public Info Scope { get; }

    }
}