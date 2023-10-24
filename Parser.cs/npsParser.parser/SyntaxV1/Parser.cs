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

        /// <summary>
        /// Create a Parser with system default settings.
        /// </summary>
        public static Parser CreateDefault()
        {
            Parser parser = new Parser();
            // Register system factories
            //parser._RootFactories.Add(new SectorFactory_Block());
            parser._RootFactories.Add(new SectorFactory_Comment());
            parser._RootFactories.Add(new SectorFactory_RootModels());

            //parser._NonRootFactories.Add(new SectorFactory_Block());
            //parser._NonRootFactories.Add(new SectorFactory_Links());
            parser._NonRootFactories.Add(new SectorFactory_Comment());
            parser._NonRootFactories.Add(new SectorFactory_Event());
            parser._NonRootFactories.Add(new SectorFactory_Attributes());
            parser._NonRootFactories.Add(new SectorFactory_Expression());
            parser._NonRootFactories.Add(new SectorFactory_Member());
            parser._NonRootFactories.Add(new SectorFactory_Method());
            return parser;
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
            do
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
                    try
                    {
                        sector = secFactory.Parse(codeLn, codesTrimmed);
                        if (sector != null)
                        {
                            break;
                        }
                    }
                    catch (ParserException pex)
                    {
                        // TODO find error message by UniqueID in different locales.
                        string msg = pex.GetLocalizedMessage();

                        // line site string.
                        string lineSiteStr = codeLn.SiteString;
                        if (pex.ErrorToken != null)
                        {
                            int columnIndex = codeLn.Content.TrimEnd().Length - pex.ErrorToken.LengthToTheEnd;
                            lineSiteStr += $"[{columnIndex}]";
                        }

                        // write to log
                        Logger.Instance.Log(ELoggerType.Error, "Parser"
                            , pex.ErrorType.UniqueID
                            , $"{lineSiteStr} : {msg}"
                            );
                    }
                    catch (Exception ex)
                    {
                        // write to log
                        Logger.Instance.Log(ELoggerType.Error, "Parser"
                            , -1
                            , $"{codeLn.SiteString} : Unexpected error occurred. {ex.Message}"
                            );
                    }
                }

                // Unrecognized sector, error out and register a 'Error' sector.
                if (sector == null)
                {
                    Logger.Instance.Log(ELoggerType.Error, "Parser"
                        , ParserErrorType.Parser_UnrecognizedSector.UniqueID
                            , $"{codeLn.SiteString} : Unexpected error occurred."
                        );
                    sector = new ErrorSector(codeLn);
                    continue;
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
            } while (InReader.GoNextLine());

            // ## Analyze sectors
            foreach (var sector in _Sectors)
            {
                sector.AnalyzeSector();
            }

            // ## Gather all types from these sectors
            foreach (var sector in _Sectors)
            {
                sector.TryCollectTypes(InProjectInfo);
            }

            // ## Collect all infos recursively.
            void _CollectSectorInfosRecursively(Sector InSec, ProjectInfo InProjectInfo, Sector InParentSector)
            {
                InSec.CollectInfos(InProjectInfo, InParentSector);
                foreach (var subSec in InSec.SubSectors)
                {
                    _CollectSectorInfosRecursively(subSec, InProjectInfo, InSec);
                }
            }
            foreach (var sector in _RootSectors)
            {
                _CollectSectorInfosRecursively(sector, InProjectInfo, null);
            }

        }

        /// <summary>
        /// Find the last sector whose indent is less than the InCurrentIndent.
        /// </summary>
        /// <param name="InCurrentIndent"></param>
        /// <returns></returns>
        Sector FindLastOuterSector(int InCurrentIndent)
        {
            for (int i = _Sectors.Count - 1; i >= 0; i--)
            {
                if (_Sectors[i].Indent < InCurrentIndent)
                { return _Sectors[i]; }
            }

            return null;
        }



    }

}
