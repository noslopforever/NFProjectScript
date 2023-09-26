using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression.DefaultSnippetElements
{

    /// <summary>
    /// A special 'NewLine' element.
    /// </summary>
    public sealed class ElementNewLine
        : STNodeTranslateSnippet.IElement
    {
        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// A snippet element which contains only constant-string.
    /// </summary>
    public class ElementConstString
        : STNodeTranslateSnippet.IElement
    {
        public string Value { get; }
        public ElementConstString(string InValue)
        {
            Value = InValue;
        }
        public override string ToString()
        {
            return Value;
        }

        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            return new string[] { Value };
        }
    }


    /// <summary>
    /// A snippet element to retrieve variable-names from STNodeVar/STNodeMemberAccess.
    /// </summary>
    public class ElementVarName
        : STNodeTranslateSnippet.IElement
    {
        public ElementVarName()
        {
        }

        public override string ToString()
        {
            return $"%{{VarName}}%";
        }

        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            ISyntaxTreeNode nodeToTranslate = InHolderSchemeInstance.NodeToTranslate;
            if (nodeToTranslate is STNodeVar)
            {
                return new string[] { (nodeToTranslate as STNodeVar).IDName };
            }
            else if (nodeToTranslate is STNodeMemberAccess)
            {
                return new string[] { (nodeToTranslate as STNodeMemberAccess).MemberID };
            }

            // TODO log error
            throw new InvalidCastException();
            return new string[] { "<<ERROR NODE TYPE>>" };
        }
    }


    /// <summary>
    /// A snippet element to retrieve constant value strings from STNodeConstant.
    /// </summary>
    public class ElementConstNodeValueString
        : STNodeTranslateSnippet.IElement
    {
        public override string ToString()
        {
            return $"%{{ValueString}}%";
        }

        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            ISyntaxTreeNode nodeToTranslate = InHolderSchemeInstance.NodeToTranslate;
            if (nodeToTranslate is STNodeConstant)
            {
                return new string[] { (nodeToTranslate as STNodeConstant).Value.ToString() };
            }

            // TODO log error
            throw new InvalidCastException();
            return new string[] { "<<ERROR NODE TYPE>>" };
        }
    }

    public class ElementReplaceSubNodeValue
        : STNodeTranslateSnippet.IElement
    {
        public ElementReplaceSubNodeValue(string InKey, string InStageName = "Present")
        {
            Key = InKey;
        }

        public string Key { get; }

        public string StageName { get; } = "Present";

        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            return new string[] { InHolderSchemeInstance.GetVarValue(Key, StageName) };
        }
    }



    /// <summary>
    /// Element which act as a new scheme with another snippet
    /// </summary>
    public abstract class ElementProxyAbstract
        : STNodeTranslateSnippet.IElement
    {
        public ElementProxyAbstract(STNodeTranslateSnippet InSnippet)
        {
            ProxySnippet = InSnippet;
        }

        /// <summary>
        /// Referenced snippet
        /// </summary>
        public STNodeTranslateSnippet ProxySnippet { get; }

        public virtual IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Get or create a scheme with only Present snippets.
            if (_cachedScheme == null)
            {
                _cachedScheme = new STNodeTranslateSchemeDefault(ProxySnippet);
            }

            // Create inner proxy SI for applying
            if (_cachedSchemeInstance == null)
            {
                _cachedSchemeInstance = _cachedScheme.CreateProxyInstance(InHolderSchemeInstance);
            }

            return _cachedSchemeInstance.GetResult("Present");
        }

        ISTNodeTranslateScheme _cachedScheme;
        ISTNodeTranslateSchemeInstance _cachedSchemeInstance;

    }


    /// <summary>
    /// Reference A temporary variable in current Context.
    /// </summary>
    public class ElementTempVar
        : STNodeTranslateSnippet.IElement
    {
        public ElementTempVar(string InKey)
        {
            Key = InKey;
        }

        /// <summary>
        /// Key name of the temp-var
        /// </summary>
        public string Key { get; }

        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            var context = InHolderSchemeInstance.TranslateContext;
            IExprTranslateContext.IVariable var = context.EnsureTempVar(InHolderSchemeInstance.NodeToTranslate, Key);
            return new string[] { var.Name };
        }

    }


    /// <summary>
    /// Element which calls another Scheme to provide the result.
    /// </summary>
    public class ElementCallOther
        : STNodeTranslateSnippet.IElement
    {
        public ElementCallOther(string InSchemeName)
        {
            SchemeName = InSchemeName;
            _varInitCodes = new List<VarInitCodeCache>();
        }

        public ElementCallOther(string InSchemeName, Dictionary<string, STNodeTranslateSnippet> InVarInitCodes)
        {
            SchemeName = InSchemeName;
            foreach (var kvp in InVarInitCodes)
            {
                _varInitCodes.Add(new VarInitCodeCache(kvp.Key, kvp.Value));
            }
        }

        /// <summary>
        /// Referenced scheme name.
        /// </summary>
        public string SchemeName { get; }

        public virtual IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Get or create a scheme with only Present snippets.
            if (_cachedScheme == null)
            {
                _cachedScheme = InHolderSchemeInstance.Translator.FindSchemeByName(SchemeName);
            }
            // Create inner proxy SI for applying
            if (_cachedSchemeInstance == null)
            {
                _cachedSchemeInstance = _cachedScheme.CreateProxyInstance(InHolderSchemeInstance);

                // Ensure and bind 'Parameter' SIs.
                foreach (var varInitCode in _varInitCodes)
                {
                    var paramSchemeInst = varInitCode.EnsureSchemeInstance(InHolderSchemeInstance);
                    _cachedSchemeInstance.AddPrerequisiteScheme(varInitCode.Key, paramSchemeInst);
                }
            }

            return _cachedSchemeInstance.GetResult("Present");
        }

        // The cache of the referenced scheme.
        ISTNodeTranslateScheme _cachedScheme;
        // The cache of the instance created by the referenced scheme.
        ISTNodeTranslateSchemeInstance _cachedSchemeInstance;

        /// <summary>
        /// Helper class to cache var-init codes.
        /// </summary>
        class VarInitCodeCache
        {
            public VarInitCodeCache(string InKey, STNodeTranslateSnippet InSnippet)
            {
                Key = InKey;
                _varSnippet = InSnippet;
            }

            /// <summary>
            /// Ensure and return the scheme instance bound with the Key
            /// </summary>
            /// <param name="InHolderSchemeInstance"></param>
            /// <returns></returns>
            public ISTNodeTranslateSchemeInstance EnsureSchemeInstance(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
            {
                if (_cachedScheme == null)
                {
                    _cachedScheme = new STNodeTranslateSchemeDefault(_varSnippet);
                }
                if (_cachedSchemeInstance == null)
                {
                    _cachedSchemeInstance = _cachedScheme.CreateProxyInstance(InHolderSchemeInstance);
                }
                return _cachedSchemeInstance;
            }

            public string Key { get; }
            STNodeTranslateSnippet _varSnippet;
            ISTNodeTranslateScheme _cachedScheme;
            ISTNodeTranslateSchemeInstance _cachedSchemeInstance;

        }
        List<VarInitCodeCache> _varInitCodes = new List<VarInitCodeCache>();


    }




}
