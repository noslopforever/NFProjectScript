
namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// expr node: identifier.
    /// </summary>
    public class STNodeGetVar : STNodeBase
    {
        internal STNodeGetVar()
        {
        }

        public STNodeGetVar(string InIdName, bool InLeftHand = false)
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