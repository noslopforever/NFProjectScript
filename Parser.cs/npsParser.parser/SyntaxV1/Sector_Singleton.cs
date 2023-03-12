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

}
