namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// The model sector
    /// </summary>
    public class ModelSector
        : Sector
    {
        public ModelSector(CodeLine InCodeLn, string InBaseTypeName, string InTypeName)
            : base(InCodeLn)
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

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            TypeInfo baseTypeInfo = InfoHelper.FindType(InProjectInfo, BaseTypeName);
            _TypeProvided.__Internal_SetBaseType(baseTypeInfo);
            return _TypeProvided;
        }

        /// <summary>
        /// Type Provided by this sector
        /// </summary>
        TypeInfo _TypeProvided = null;

    }


}
