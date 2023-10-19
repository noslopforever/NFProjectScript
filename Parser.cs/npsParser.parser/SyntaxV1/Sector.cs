using System;
using System.Collections.Generic;
using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Nps file is splitted by sectors. A sector was built with one or several lines.
    /// </summary>
    public abstract class Sector
    {
        public Sector(CodeLine InCodeLn)
        {
            CodeLn = InCodeLn;
        }

        /// <summary>
        /// Indent of the sector
        /// </summary>
        public int Indent { get; internal set; }

        /// <summary>
        /// The codeline which has been used to generate the sector.
        /// </summary>
        public CodeLine CodeLn { get; }

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
        /// Attributes registered to the sector.
        /// </summary>
        internal STNode_AttributeDefs Attributes { get; private set; }

        /// <summary>
        /// Comments registered to the sector.
        /// </summary>
        internal string Comment { get; private set; } = "";

        /// <summary>
        /// Does this sector contains TypeInfo?
        /// </summary>
        /// <returns></returns>
        public virtual bool ContainsTypeInfo()
        {
            return false;
        }

        internal void _SetAttributes(STNode_AttributeDefs InAttrs)
        {
            Attributes = InAttrs;
        }

        internal void _SetComment(string InComments)
        {
            Comment = InComments;
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

            // Register attributes
            if (Attributes != null)
            {
                foreach (var attrDef in Attributes)
                {
                    var attrInfo = new AttributeInfo(CollectedInfo, attrDef.DefName, attrDef.DefName, attrDef.InitExpression);
                }
            }

            // Register comments
            if (Comment != "")
            {
                var cmtInfo = new CommentInfo(CollectedInfo, Comment);
            }

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
