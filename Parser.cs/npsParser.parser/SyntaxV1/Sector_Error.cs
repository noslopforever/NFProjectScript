using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Hold an error sector.
    /// </summary>
    public class ErrorSector
        : Sector
    {
        public ErrorSector(CodeLine InCodeLn)
            : base(InCodeLn)
        {
        }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            return null;
        }
    }

}
