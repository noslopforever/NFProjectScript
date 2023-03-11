using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// The model sector
    /// </summary>
    public class ModelSector
        : Sector
    {
        public ModelSector(string InBaseTypeName, string InTypeName)
            : base(null)
        {
            BaseTypeName = InBaseTypeName;
            TypeName = InTypeName;
        }

        /// <summary>
        /// The Object's type-name.
        /// </summary>
        public string BaseTypeName { get; }

        /// <summary>
        /// The Object's name.
        /// </summary>
        public string TypeName { get; }

        // implement abstract methods
        //
        public override void TryCollectTypes(ProjectInfo InProjectInfo)
        {
            // TODO BaseType is not valid when collecting Types. Could we find a better way?
            _TypeProvided = new TypeInfo(InProjectInfo, "model", TypeName);
        }

        public override Info CollectInfos(ProjectInfo InProjectInfo, Info InParentInfo)
        {
            System.Diagnostics.Debug.Assert(InProjectInfo == InParentInfo);

            TypeInfo baseTypeInfo = InfoHelper.FindType(InProjectInfo, BaseTypeName);
            _TypeProvided.__Internal_SetBaseType(baseTypeInfo);
            return _TypeProvided;
        }

        /// <summary>
        /// Type Provided by this sector
        /// </summary>
        TypeInfo _TypeProvided = null;

    }

    /// <summary>
    /// Try parse codes into a sector which defines a model/class.
    /// </summary>
    public class SectorFactory_Model
        : SectorFactory
    {
        // implement abstract methods
        //
        protected override Sector ParseImpl(ICodeContentReader InReader, string InCodesWithoutIndent)
        {
            // Try parse codes as a Model-Type or Attribute:
            //     {BaseType} {ModelName}
            //     [{AttributeName} = {Expr}]
            List<token.Token> tokens = new List<token.Token>();
            token.TokenParser_CommonNps.Instance.ParseLine(InCodesWithoutIndent, ref tokens);

            if (tokens.Count == 2)
            {
                if (tokens[0].TokenType == token.ETokenType.ID
                    && tokens[1].TokenType == token.ETokenType.ID
                    )
                {
                    string baseTypeName = tokens[0].Code;
                    string typeName = tokens[1].Code;
                    return new ModelSector(baseTypeName, typeName);
                }
            }
            throw new NotImplementedException();
        }
    }

}
