
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
            LeftHandValue = InLeftHand;
        }

        /// <summary>
        /// The identifier's name: variable's name, member's name, and so on...
        /// </summary>
        public string IDName { get; }

        /// <summary>
        /// Is this var a left-value, which should be assigned and modified.
        /// 
        /// a = b - a is a left hand value, and b is a right hand value.
        /// a.b = c - a and b are left hand values, and c is a right hand value.
        /// </summary>
        public bool LeftHandValue { get; }

        ///// <summary>
        ///// The scope of this variable.
        ///// </summary>
        //public Info Scope { get; }

    }
}