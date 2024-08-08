using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.translator
{
    /// <summary>
    /// Base class for all contexts implemented in the project.
    /// Provides functionality for accessing parent context and retrieving context values.
    /// </summary>
    public abstract class TranslatingContextBase : ITranslatingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatingContextBase"/> class.
        /// </summary>
        /// <param name="InParentContext">The parent context.</param>
        public TranslatingContextBase(ITranslatingContext InPrevContext)
        {
            PreviousContext = InPrevContext;
        }

        // Begin ITranslatingContext interfaces

        /// <inheritdoc />
        public ITranslatingContext PreviousContext { get; }
        /// <inheritdoc />
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
                // TODO: Log the exception.
            }

            OutValue = null;
            return false;
        }
        /// <inheritdoc />
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
    /// Context for the translating Info.
    /// </summary>
    public class TranslatingInfoContext : TranslatingContextBase, ITranslatingInfoContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatingInfoContext"/> class.
        /// </summary>
        /// <param name="InParentContext">The parent context.</param>
        /// <param name="InBoundInfo">The info to translate.</param>
        public TranslatingInfoContext(ITranslatingContext InParentContext, Info InBoundInfo)
            : base(InParentContext)
        {
            TranslatingInfo = InBoundInfo;
        }

        /// <summary>
        /// Gets the info being translated.
        /// </summary>
        public Info TranslatingInfo { get; }

        /// <inheritdoc />
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
                // TODO: Log the exception.
            }

            return base.TryGetContextValue(InKey, out OutValue);
        }
    }

    /// <summary>
    /// Context for the translating expression-node (syntax tree node).
    /// </summary>
    public class TranslatingExprContext : TranslatingContextBase, ITranslatingExprContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatingExprContext"/> class.
        /// </summary>
        /// <param name="InParentContext">The parent context.</param>
        /// <param name="InSTNode">The syntax tree node to translate.</param>
        public TranslatingExprContext(ITranslatingContext InParentContext, ISyntaxTreeNode InSTNode)
            : base(InParentContext)
        {
            TranslatingExprNode = InSTNode;
        }

        /// <summary>
        /// Gets the syntax tree node being translated.
        /// </summary>
        public ISyntaxTreeNode TranslatingExprNode { get; }

        /// <inheritdoc />
        public override bool TryGetContextValue(string InKey, out object OutValue)
        {
            try
            {
                var prop = TranslatingExprNode.GetType().GetProperty(InKey);
                if (prop != null)
                {
                    OutValue = prop.GetValue(TranslatingExprNode);
                    return true;
                }
            }
            catch
            {
                // TODO: Log the exception.
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
