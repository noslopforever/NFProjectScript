
namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// expr node: identifier.
    /// </summary>
    public class STNodeVar
        : STNodeBase
    {
        internal STNodeVar()
        {
        }

        public STNodeVar(string InIdName)
        {
            IDName = InIdName;
        }

        /// <summary>
        /// The identifier's name: variable's name, member's name, and so on...
        /// </summary>
        [Serialization.SerializableInfo]
        public string IDName { get; private set; }

    }
}