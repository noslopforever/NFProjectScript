namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Sector contains only comments (start with # )
    /// </summary>
    public class CommentSector
        : Sector
    {
        public CommentSector(CodeLine InCodeLn, string InComment)
            : base(InCodeLn)
        {
            _SetComment(InComment);
        }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            // register comments into the parent sector.
            if (InParentSector == null)
            {
                // No parent sector (root), register comment to the ProjectInfo.
                var cmtInfo = new CommentInfo(InProjectInfo, Comment);
                return null;
            }
            else
            {
                // register comment to the host sector.
                var cmtInfo = new CommentInfo(InParentSector.CollectedInfo, Comment);
                return null;
            }
        }

    }

}
