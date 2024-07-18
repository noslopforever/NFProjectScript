using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{
    /// <summary>
    /// System Call: Query Info and call other scheme for it.
    /// </summary>
    public class ElementQueryInfo
        : InfoTranslateSnippet.IElement
    {
        public ElementQueryInfo(string InSystemCallName, string InSchemeName, params (string, InfoTranslateSnippet.IElement)[] InParams)
        {
            SystemCallName = InSystemCallName;
            SchemeName = InSchemeName;
        }

        /// <summary>
        /// Which call to be used to query the target info.
        /// </summary>
        public string SystemCallName { get; }

        /// <summary>
        /// Scheme to apply to the target info if found.
        /// </summary>
        public string SchemeName { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            throw new NotImplementedException();
            // TODO system call
            var targetInfo = null;
            // 
        }
    }

}