﻿using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using static nf.protoscript.translator.expression.ExprTranslatorAbstract;
using System.Security.Cryptography;

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
    /// A snippet element to get value strings from the translating Context.
    /// </summary>
    public class ElementNodeValue
        : STNodeTranslateSnippet.IElement
    {
        public ElementNodeValue(string InKey, string InStageName = "Present")
        {
            Key = InKey;
        }

        public string Key { get; }

        public string StageName { get; } = "Present";

        public IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Find codes generated by SI's prerequisite
            var schemeInst = InHolderSchemeInstance.FindPrerequisite(Key);
            if (schemeInst != null)
            {
                var result = schemeInst.GetResult(StageName);
                return result;
            }

            // Find data value from SI's context
            var ctxValStr = InHolderSchemeInstance.TranslatingContext.GetContextValueString(Key);
            return new string[] { ctxValStr };
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
    /// Reference A temporary variable in the current Context.
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
            var env = InHolderSchemeInstance.TranslatingContext.HostMethodBody.RootEnvironment;
            ISyntaxTreeNode translatingNode = null;
            if (InHolderSchemeInstance.TranslatingContext is ExprTranslatorAbstract.INodeContext)
            {
                var nodeCtx = InHolderSchemeInstance.TranslatingContext as ExprTranslatorAbstract.INodeContext;
                translatingNode = nodeCtx.TranslatingNode;
            }

            IExprTranslateEnvironment.IVariable nodeTempVar = env.EnsureTempVar(translatingNode, Key);
            return new string[] { nodeTempVar.Name };
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
            // Create a scheme with only Present snippets.
            var refScheme = InHolderSchemeInstance.Translator.FindBestScheme(InHolderSchemeInstance.TranslatingContext, SchemeName);
            var refSchemeInst = refScheme.CreateProxyInstance(InHolderSchemeInstance);

            // Ensure and bind 'Parameter' SIs.
            foreach (var varInitCode in _varInitCodes)
            {
                var paramSchemeInst = varInitCode.EnsureSchemeInstance(InHolderSchemeInstance);
                refSchemeInst.AddPrerequisiteScheme(varInitCode.Key, paramSchemeInst);
            }

            return refSchemeInst.GetResult("Present");
        }

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
                var varScheme = new STNodeTranslateSchemeDefault(_varSnippet);
                var varSchemeInst = varScheme.CreateProxyInstance(InHolderSchemeInstance);
                return varSchemeInst;
            }

            public string Key { get; }
            STNodeTranslateSnippet _varSnippet;

        }
        List<VarInitCodeCache> _varInitCodes = new List<VarInitCodeCache>();


    }




}
