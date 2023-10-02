namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// The singleton sector.
    /// </summary>
    public class SingletonSector
        : Sector
    {
        public SingletonSector(CodeLine InCodeLn, string InSingletonName)
            : base(InCodeLn)
        {
            SingletonName = InSingletonName;
        }

        /// <summary>
        /// The Singleton's Name
        /// </summary>
        public string SingletonName { get; }

        TypeInfo _singletonType = null;

        // implement abstract methods
        //
        public override void TryCollectTypes(ProjectInfo InProjectInfo)
        {
            string typeName = SingletonName.Remove(0, 1);
            _singletonType = new TypeInfo(InProjectInfo, "model", typeName);
        }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            var singleton = new ElementInfo(InProjectInfo, "singleton", SingletonName, _singletonType, null);
            return _singletonType;
        }

    }

}
