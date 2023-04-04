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

        // implement abstract methods
        //
        public override void TryCollectTypes(ProjectInfo InProjectInfo)
        {
        }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            ElementInfo singleton = new ElementInfo(InProjectInfo, "singleton", SingletonName, null, null);
            return singleton;
        }

    }

}
