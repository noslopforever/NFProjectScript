namespace nf.protoscript.translator
{

    /// <summary>
    /// Context which preserves states for a translating process.
    /// 
    /// Translating Project holds translating Types, translating Types holds translating properties/methods,
    /// So the contexts will always establish a context-tree.
    /// </summary>
    public interface ITranslatingContext
    {
        /// <summary>
        /// The parent context of this context.
        /// </summary>
        ITranslatingContext ParentContext { get; }

        ///// <summary>
        ///// Which project is being translated when handling the current context.
        ///// </summary>
        //ProjectInfo HostProjectInfo { get; }

        ///// <summary>
        ///// Which Type is being translated when handling the current context. Nullable.
        ///// </summary>
        //TypeInfo HostTypeInfo { get; }

        ///// <summary>
        ///// Which element is being translated when handling the current context. Nullable.
        ///// </summary>
        //ElementInfo HostElementInfo { get; }

        /// <summary>
        /// Get value string from this context by InKey.
        /// </summary>
        /// <param name="InKey"></param>
        /// <returns></returns>
        string GetContextValueString(string InKey);

    }

    /// <summary>
    /// Context for an Info which is being translated.
    /// </summary>
    public interface ITranslatingInfoContext
        : ITranslatingContext
    {

        /// <summary>
        /// The translating Info.
        /// </summary>
        Info TranslatingInfo { get; }

    }





}
