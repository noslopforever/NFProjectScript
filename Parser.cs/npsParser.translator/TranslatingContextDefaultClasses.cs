using System;

namespace nf.protoscript.translator
{
    /// <summary>
    /// Base of all Contexts implemented in the project.
    /// </summary>
    public abstract class TranslatingContextBase
       : ITranslatingContext
    {
        public TranslatingContextBase(ITranslatingContext InParentContext)
        {
            ParentContext = InParentContext;
        }

        public virtual ITranslatingContext ParentContext { get; }

        public virtual bool TryGetContextValue(string InKey, out object OutValue)
        {
            try
            {
                var keyProp = GetType().GetProperty(InKey);
                if (keyProp != null)
                {
                    OutValue = keyProp.GetValue(this);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // TODO log error
            }

            if (ParentContext != null)
            {
                return ParentContext.TryGetContextValue(InKey, out OutValue);
            }

            OutValue = null;
            return false;
        }

        public string GetContextValueString(string InKey)
        {
            if (TryGetContextValue(InKey, out var val))
            {
                if (val == null)
                {
                    return "null";
                }
                return val.ToString();
            }
            return $"<<NULL VAR for {InKey}>>";
        }

    }

    /// <summary>
    /// Context for the translating Info
    /// </summary>
    public class TranslatingInfoContext
        : TranslatingContextBase
        , ITranslatingInfoContext
    {
        public TranslatingInfoContext(ITranslatingContext InParentContext, Info InBoundInfo)
            : base(InParentContext)
        {
            TranslatingInfo = InBoundInfo;
        }

        // Begin ITranslatingInfoContext interfaces
        public Info TranslatingInfo { get; }
        // ~ End ITranslatingInfoContext interfaces

        public override bool TryGetContextValue(string InKey, out object OutValue)
        {
            try
            {
                var prop = TranslatingInfo.GetType().GetProperty(InKey);
                if (prop != null)
                {
                    OutValue = prop.GetValue(TranslatingInfo);
                    return true;
                }
            }
            catch
            {
                // TODO log error.
            }

            return base.TryGetContextValue(InKey, out OutValue);
        }

    }

    ///// <summary>
    ///// Context for the translating project.
    ///// </summary>
    //public class TranslatingProjectContext
    //        : TranslatingInfoContext
    //{
    //    public TranslatingProjectContext(ProjectInfo InProjInfo)
    //        : base(null, InProjInfo)
    //    {
    //        HostProjectInfo = InProjInfo;
    //    }

    //    public TranslatingProjectContext(ITranslatingContext InParentContext, ProjectInfo InProjInfo)
    //        : base(InParentContext, InProjInfo)
    //    {
    //        HostProjectInfo = InProjInfo;
    //    }

    //    // Begin ITranslatingContext interfaces
    //    public override ProjectInfo HostProjectInfo { get; }
    //    public override TypeInfo HostTypeInfo => null;
    //    public override ElementInfo HostElementInfo => null;
    //    // ~ End ITranslatingContext interfaces

    //}

    ///// <summary>
    ///// Context for the translating TypeInfo
    ///// </summary>
    //public class TranslatingTypeContext
    //    : TranslatingInfoContext
    //{
    //    public TranslatingTypeContext(ITranslatingContext InParentContext, TypeInfo InTypeInfo)
    //        : base(InParentContext, InTypeInfo)
    //    {
    //        TranslatingType = InTypeInfo;
    //    }

    //    // Begin ITranslatingContext interfaces
    //    public override TypeInfo HostTypeInfo => TranslatingType;
    //    public override ElementInfo HostElementInfo => null;
    //    // ~ End ITranslatingContext interfaces

    //    /// <summary>
    //    /// Type being translated.
    //    /// </summary>
    //    public TypeInfo TranslatingType { get; }

    //    /// <summary>
    //    /// Type name of the host.
    //    /// </summary>
    //    public string TranslatingTypeName { get { return TranslatingType.Name; } }

    //}

    ///// <summary>
    ///// Context for the translating ElementInfo
    ///// </summary>
    //public class TranslatingElementContext
    //    : TranslatingInfoContext
    //{
    //    public TranslatingElementContext(ITranslatingContext InParentContext, ElementInfo InElementInfo)
    //        : base(InParentContext, InElementInfo)
    //    {
    //        TranslatingElement = InElementInfo;
    //    }

    //    // Begin ITranslatingContext interfaces
    //    public override ElementInfo HostElementInfo => TranslatingElement;
    //    // ~ End ITranslatingContext interfaces

    //    /// <summary>
    //    /// Element being translated.
    //    /// </summary>
    //    public ElementInfo TranslatingElement { get; }

    //    /// <summary>
    //    /// Name of the translating element.
    //    /// </summary>
    //    public string TranslatingElementName { get { return TranslatingElement.Name; } }

    //}





}
