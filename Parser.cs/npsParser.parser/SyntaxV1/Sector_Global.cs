namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// The global sector
    /// </summary>
    public class GlobalSector
        : Sector
    {
        public GlobalSector(CodeLine InCodeLn, string InTypeName, string InObjectName)
            : base(InCodeLn)
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

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            TypeInfo typeInfo = InfoHelper.FindType(InProjectInfo, TypeName);
            ElementInfo globalObject = new ElementInfo(InProjectInfo, "object", ObjectName, typeInfo, null);
            return globalObject;
        }

    }

}
