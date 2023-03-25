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
        /// Collected Info of this sector.
        /// </summary>
        public Info CollectedInfo { get; private set; }

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
        /// Get all sub sectors with type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetSubSectors<T>() where T: Sector
        {
            List<T> results = new List<T>();
            foreach (var sec in _SubSectors)
            {
                if (sec is T)
                {
                    results.Add(sec as T);
                }
            }
            return results;
        }

        /// <summary>
        /// Analyze sector.
        /// </summary>
        public virtual void AnalyzeSector()
        {
        }

        /// <summary>
        /// Try collect Type infos provided by this sector.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// 
        public virtual void TryCollectTypes(ProjectInfo InProjectInfo)
        {
        }

        /// <summary>
        /// Collect Info contains in this sector.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InParentSector"></param>
        public void CollectInfos(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            CollectedInfo = CollectInfosImpl(InProjectInfo, InParentSector);
        }

        /// <summary>
        /// Abstract method which implements the CollectInfos.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InParentSector"></param>
        /// 
        /// <returns></returns>
        protected abstract Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector);

    }

}
