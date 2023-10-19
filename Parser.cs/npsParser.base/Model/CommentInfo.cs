namespace nf.protoscript
{
    /// <summary>
    /// Comment, be used to add descriptions to its parent info.
    /// </summary>
    public class CommentInfo : Info
    {
        internal CommentInfo(Info InParentInfo, string InHeader, string InName)
           : base(InParentInfo, InHeader, InName)
        { }

        public CommentInfo(Info InParentInfo, string InComment)
            : base(InParentInfo, "comment", "")
        {
        }

        /// <summary>
        /// Comment string.
        /// </summary>
        [Serialization.SerializableInfo]
        public string Comment { get; private set; }

    }

}
