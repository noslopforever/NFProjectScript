using nf.protoscript.parser.token;
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
        void Parse(ICodeContentReader InReader)
        {
            _Sectors = new List<Sector>();

            // Parser states.
            while (!InReader.IsEndOfFile)
            {
                // ## Reforge codes.
                CodeLine codeLn = InReader.CurrentCodeLine;

                int indent = 0;
                string codesTrimmed = ParseHelper.TrimCodes(codeLn.Content, out indent);

                // empty line: continue.
                if (codesTrimmed == "")
                { continue; }

                // ## Let states determine how to parse the element.


                Sector sector = null;
                // If indent == 0, try parse the line with root-factories.
                if (indent == 0)
                {
                    // select the factory who can recognize sub elements.
                    foreach (var secFactory in _RootFactories)
                    {
                        sector = secFactory.Parse(InReader, codesTrimmed);
                        if (sector != null)
                        { break; }
                    }
                }
                // else, try parse the line with sub-factories
                else
                {
                    foreach (var secFactory in _NonRootFactories)
                    {
                        sector = secFactory.Parse(InReader, codesTrimmed);
                        if (sector != null)
                        { break; }
                    }
                }

                if (sector == null)
                {
                    // Unrecognized sector
                    throw new NotImplementedException();
                }


                // Find the parent sector, and attach to it.
                Sector parentSector = FindLastOuterSector(indent);
                parentSector._AddSubSector(sector);
                // save indent and push sector.
                sector.Indent = indent;
                _Sectors.Add(sector);

                // Move to the next line.
                InReader.GoNextLine();
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
