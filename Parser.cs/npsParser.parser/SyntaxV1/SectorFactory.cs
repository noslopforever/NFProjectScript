namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// Factory to parse codes with different formats into sectors.
    /// </summary>
    public abstract class SectorFactory
    {
        /// <summary>
        /// Parse codes into sectors.
        /// </summary>
        /// <param name="InReader"></param>
        /// <param name="InIndent"></param>
        /// <param name="InCodesWithoutIndent"></param>
        /// <returns></returns>
        public Sector Parse(CodeLine InCodeLine, string InCodesWithoutIndent)
        {
            Sector result = ParseImpl(InCodeLine, InCodesWithoutIndent);
            return result;
        }

        protected abstract Sector ParseImpl(CodeLine InCodeLine, string InCodesWithoutIndent);

    }

}
