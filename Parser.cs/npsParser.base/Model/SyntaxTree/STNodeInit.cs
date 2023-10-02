namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// Init, describe a variable init expression.
    /// 
    /// model mdl
    ///     - member = 100
    /// Here, the expr's purpose is to 'init' the member to 100, not 'set' the member to 100.
    /// So we need STNodeInit to describe this situition.
    /// </summary>
    public class STNodeInit
        : STNodeBase
    {
        public STNodeInit()
            : base()
        {
        }

        public STNodeInit(Info InInfoToBeInit, ISyntaxTreeNode InRHS)
            : base()
        {
            InfoToBeInit = InInfoToBeInit;
            RHS = InRHS;
        }

        /// <summary>
        /// The element to be initialized.
        /// </summary>
        [Serialization.SerializableInfo]
        public Info InfoToBeInit { get; private set; }

        /// <summary>
        /// Right hand value
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

    }

}
