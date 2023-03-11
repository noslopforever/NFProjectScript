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
        public Sector Parse(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            Sector result = ParseImpl(InReader, InCodesWithoutIndent);
            return result;
        }

        protected abstract Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent);

    }

}
