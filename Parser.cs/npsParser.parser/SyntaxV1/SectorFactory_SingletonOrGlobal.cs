using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{

    /// <summary>
    /// The singleton sector.
    /// </summary>
    public class SingletonSector
        : Sector
    {
        public SingletonSector(string InSingletonName)
            : base(null)
        {
            SingletonName = InSingletonName;
        }

        /// <summary>
        /// The Singleton's Name
        /// </summary>
        public string SingletonName { get; }

        // implement abstract methods
        //
        public override void TryCollectTypes(ProjectInfo InProjectInfo)
        {
            TypeInfo singletonType = new TypeInfo(InProjectInfo, "model", SingletonName);
        }

        public override Info CollectInfos(ProjectInfo InProjectInfo, Info InParentInfo)
        {
            System.Diagnostics.Debug.Assert(InProjectInfo == InParentInfo);

            TypeInfo typeInfo = InfoHelper.FindType(InProjectInfo, SingletonName);
            ElementInfo singleton = new ElementInfo(InProjectInfo, "singleton", SingletonName, typeInfo, null);
            return singleton;
        }

    }

    /// <summary>
    /// The global sector
    /// </summary>
    public class GlobalSector
        : Sector
    {
        public GlobalSector(string InTypeName, string InObjectName)
            : base(null)
        {
            TypeName = InTypeName;
            ObjectName = InObjectName;
        }

        /// <summary>
        /// The Object's type-name.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// The Object's name.
        /// </summary>
        public string ObjectName { get; }

        // implement abstract methods
        //
        public override void TryCollectTypes(ProjectInfo InProjectInfo)
        {
            TypeInfo singletonType = new TypeInfo(InProjectInfo, "model", TypeName);
        }

        public override Info CollectInfos(ProjectInfo InProjectInfo, Info InParentInfo)
        {
            System.Diagnostics.Debug.Assert(InProjectInfo == InParentInfo);

            TypeInfo typeInfo = InfoHelper.FindType(InProjectInfo, TypeName);
            ElementInfo globalObject = new ElementInfo(InProjectInfo, "object", ObjectName, typeInfo, null);
            return globalObject;
        }

    }

    /// <summary>
    /// Try parse codes into a sector which describes a global-object/singleton.
    /// </summary>
    public class SectorFactory_SingletonOrGlobal
        : SectorFactory
    {

        // implement abstract methods
        //
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            if (InCodesWithoutIndent[0] != '$')
            {
                return null;
            }
            string codesWithoutDollar = InCodesWithoutIndent.Substring(1);

            List<token.Token> tokens = new List<token.Token>();
            token.TokenParser_CommonNps.Instance.ParseLine(codesWithoutDollar, ref tokens);
            if (tokens.Count == 1)
            {
                if (tokens[0].TokenType == token.ETokenType.ID)
                {
                    string singletonName = tokens[0].Code;
                    var sector = new SingletonSector(singletonName);
                    return sector;
                }
                else
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
            }
            else if (tokens.Count == 2)
            {
                if (tokens[0].TokenType == token.ETokenType.ID
                    && tokens[1].TokenType == token.ETokenType.ID
                    )
                {
                    string typename = tokens[0].Code;
                    string objectName = tokens[1].Code;
                    var sector = new GlobalSector(typename, objectName);
                    return sector;
                }
                else
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
            }

            // TODO log error
            return null;
        }

    }

}
