using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Nps file is splitted by sectors. A sector was built with one or several lines.
    /// </summary>
    public abstract class Sector
    {
        public Sector(Token[] InTokens)
        {
            Tokens = InTokens;
        }

        /// <summary>
        /// Indent of the sector
        /// </summary>
        public int Indent { get; internal set; }

        /// <summary>
        /// Tokens of the sector.
        /// </summary>
        public IReadOnlyList<Token> Tokens { get; }

        /// <summary>
        /// Sub sectors of this sector.
        /// </summary>
        public IReadOnlyList<Sector> SubSectors { get { return _SubSectors; } }
        List<Sector> _SubSectors = new List<Sector>();

        /// <summary>
        /// Does this sector contains TypeInfo?
        /// </summary>
        /// <returns></returns>
        public virtual bool ContainsTypeInfo()
        {
            return false;
        }

        /// <summary>
        /// Add sub-sector to this.
        /// </summary>
        /// <param name="InSector"></param>
        internal void _AddSubSector(Sector InSector)
        {
            _SubSectors.Add(InSector);
        }

        /// <summary>
        /// Try collect Type infos provided by this sector.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <returns></returns>
        public virtual void TryCollectTypes(ProjectInfo InProjectInfo)
        {
        }

        /// <summary>
        /// Collect Infos contain in this sector.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InParentInfo"></param>
        public abstract Info CollectInfos(ProjectInfo InProjectInfo, Info InParentInfo);

    }

}
