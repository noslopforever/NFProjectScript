using nf.protoscript.syntaxtree;

namespace nf.protoscript.translator
{

    /// <summary>
    /// Represents a context used to store state information during the translation process.
    /// 
    /// This context helps avoid the need to handle different objects directly during translation.
    /// All translatable objects should provide their own implementation of this interface,
    /// and these implementations should be registered within the translator to ensure that
    /// <see cref="InfoTranslatorAbstract.CreateContext"/> can return the appropriate context for each object.
    /// </summary>
    public interface ITranslatingContext
    {
        /// <summary>
        /// Gets the parent context of this context.
        /// </summary>
        ITranslatingContext ParentContext { get; }

        /// <summary>
        /// Attempts to retrieve a context variable by its key.
        /// </summary>
        /// <param name="InKey">The key of the context variable to retrieve.</param>
        /// <param name="OutValue">The retrieved context variable value, if found.</param>
        /// <returns><c>true</c> if the context variable was found; otherwise, <c>false</c>.</returns>
        bool TryGetContextValue(string InKey, out object OutValue);

        /// <summary>
        /// Retrieves a context variable as a string by its key.
        /// </summary>
        /// <param name="InKey">The key of the context variable to retrieve.</param>
        /// <returns>The string representation of the context variable, or a default message if not found.</returns>
        string GetContextValueString(string InKey);

    }

    /// <summary>
    /// Defines the contract for a context that represents an Info being translated.
    /// </summary>
    public interface ITranslatingInfoContext : ITranslatingContext
    {
        /// <summary>
        /// Gets the Info being translated.
        /// </summary>
        Info TranslatingInfo { get; }
    }

    /// <summary>
    /// Defines the contract for a context that represents a translating expression node.
    /// </summary>
    public interface ITranslatingExprContext : ITranslatingContext
    {
        /// <summary>
        /// Gets the expression node being translated.
        /// </summary>
        ISyntaxTreeNode TranslatingExprNode { get; }
    }
}
