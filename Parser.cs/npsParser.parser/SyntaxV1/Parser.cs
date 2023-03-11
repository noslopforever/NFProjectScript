using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// A parser to parse nps codes.
    /// </summary>
    public class Parser
    {
        public Parser()
        {
        }

        public enum ESystemDefault
        {
            On,
        }
        /// <summary>
        /// We should use new Parser(Parser.ESystemDefault.On) to create a Parser with system default settings.
        /// </summary>
        public Parser(ESystemDefault InUseSystemDefault)
        {
            // Register system factories
            _RootFactories.Add(new SectorFactory_SingletonOrGlobal());
            //_RootFactories.Add(new SectorFactory_Block());
            _RootFactories.Add(new SectorFactory_Model());

            //_NonRootFactories.Add(new SectorFactory_Block());
            //_NonRootFactories.Add(new SectorFactory_Links());
            //_NonRootFactories.Add(new SectorFactory_Expression());
            //_NonRootFactories.Add(new SectorFactory_Element());
        }

        /// <summary>
        /// Sectors parsed.
        /// </summary>
        private List<Sector> _Sectors = null;

        /// <summary>
        /// Root sectors.
        /// </summary>
        private List<Sector> _RootSectors = null;

        /// <summary>
        /// States to parse root sectors.
        /// </summary>
        private List<SectorFactory> _RootFactories = new List<SectorFactory>();

        /// <summary>
        /// States to parse non-root sectors.
        /// </summary>
        private List<SectorFactory> _NonRootFactories = new List<SectorFactory>();
 
        /// <summary>
        /// Parse a nps file into sectors.
        /// </summary>
        /// <param name="InReader"></param>
        public void Parse(ProjectInfo InProjectInfo, ICodeContentReader InReader)
        {
            _Sectors = new List<Sector>();
            _RootSectors = new List<Sector>();

            // Parse the file.
            while (!InReader.IsEndOfFile)
            {
                // ## Reforge codes.
                CodeLine codeLn = InReader.CurrentCodeLine;

                int indent = 0;
                string codesTrimmed = ParseHelper.TrimCodes(codeLn.Content, out indent);

                // empty line: continue.
                if (codesTrimmed == "")
                { continue; }

                // ## Let factories determine how to parse the element.
                Sector sector = null;
                // If indent == 0, use root-factories.
                // else, use non-root-factories
                var factories = _RootFactories;
                if (indent != 0)
                { factories = _NonRootFactories; }
 
                // Try select a factory which can recognize the codes.
                foreach (var secFactory in factories)
                {
                    sector = secFactory.Parse(InReader, codesTrimmed);
                    if (sector != null)
                    { break; }
                }

                // Unrecognized sector, error out.
                if (sector == null)
                {
                    throw new NotImplementedException();
                }

                // Try attach the sector to its parent.
                // If no-parent, attach it to the root.
                Sector parentSector = FindLastOuterSector(indent);
                if (parentSector != null)
                { parentSector._AddSubSector(sector); }
                else
                { _RootSectors.Add(sector); }

                // save indent and push sector.
                sector.Indent = indent;
                _Sectors.Add(sector);

                // Move to the next line.
                InReader.GoNextLine();
            }

            // ## Gather all types from these sectors
            foreach (var sector in _Sectors)
            {
                sector.TryCollectTypes(InProjectInfo);
            }

            // ## Collect all infos recursively.
            void _CollectSectorInfosRecursively(Sector InSec, Info InParentInfo)
            {
                Info thisSecInfo = InSec.CollectInfos(InProjectInfo, InParentInfo);
                foreach (var subSec in InSec.SubSectors)
                {
                    _CollectSectorInfosRecursively(subSec, thisSecInfo);
                }
            }
            foreach (var sector in _RootSectors)
            {
                _CollectSectorInfosRecursively(sector, InProjectInfo);
                sector.CollectInfos(InProjectInfo, InProjectInfo);
            }

        }

        /// <summary>
        /// Find the last sector whose indent is less than the InCurrentIndent.
        /// </summary>
        /// <param name="InCurrentIndent"></param>
        /// <returns></returns>
        Sector FindLastOuterSector(int InCurrentIndent)
        {
            for (int i = _Sectors.Count - 1; i >= 0; i++)
            {
                if (_Sectors[i].Indent < InCurrentIndent)
                { return _Sectors[i]; }
            }

            return null;
        }



    }

}
