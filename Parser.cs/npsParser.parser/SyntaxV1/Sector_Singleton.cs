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
            _singletonType = new TypeInfo(InProjectInfo, "model", SingletonName);
        }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            var singleton = new ElementInfo(InProjectInfo, "singleton", SingletonName, _singletonType, null);
            return _singletonType;
        }

    }

}
