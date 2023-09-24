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
    /// Add Temporary variable in current Context.
    /// </summary>
    public class ElementAddTempVar
        : ElementProxyAbstract
    {
        //public ElementTempVar(string InKey)
        //{
        //    Key = InKey;
        //}
        public ElementAddTempVar(string InKey, STNodeTranslateSnippet InInitSnippet)
            : base(InInitSnippet)
        {
            Key = InKey;
        }
        public ElementAddTempVar(string InKey, params STNodeTranslateSnippet.IElement[] InInitElements)
            : base(new STNodeTranslateSnippet(InInitElements))
        {
            Key = InKey;
        }

        /// <summary>
        /// Key name of the temp-var
        /// </summary>
        public string Key { get; }

        public override IReadOnlyList<string> Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            //IExprTranslateContext.IVariable var = InHolderSchemeInstance.EnsureTempVar(Key, InHolderSchemeInstance.NodeToTranslate, InitSnippet);
            IExprTranslateContext.IVariable var = InHolderSchemeInstance.GetTempVar(Key);
            if (var == null)
            {
                // Acquire TempVar init value SI.
                var tempVarInitValueScheme = new STNodeTranslateSchemeDefault(ProxySnippet);
                var tempVarInitValueSI = tempVarInitValueScheme.CreateProxyInstance(InHolderSchemeInstance);

                // Add TempVar and cache it in SI.
                var = InHolderSchemeInstance.TranslateContext.AddTempVar(
                    InHolderSchemeInstance.NodeToTranslate
                    , Key
                    );
                InHolderSchemeInstance.AddTempVar(Key, var);

                // Generate temp var init code.
                var tempVarInitScheme = InHolderSchemeInstance.Translator.QueryInitTempVarScheme(
                    InHolderSchemeInstance.NodeToTranslate
                    , Key
                    , tempVarInitValueSI
                    );
                var tempVarInitSI = tempVarInitScheme.CreateInstance(
                    InHolderSchemeInstance.Translator
                    , InHolderSchemeInstance.TranslateContext
                    , InHolderSchemeInstance.NodeToTranslate
                    );
                tempVarInitSI.SetEnvVariable("TEMPVARNAME", var.Name);
                tempVarInitSI.AddPrerequisiteScheme("TEMPVARVALUE", tempVarInitValueSI);

                var tempVarInitCodes = new List<string>();
                InHolderSchemeInstance.Translator.TranslateOneStatement(tempVarInitCodes, tempVarInitSI);

                //// Add new line at last
                //tempVarInitCodes.Add("");

                return tempVarInitCodes;
            }

            // Only once.
            return new string[] { "" };
        }
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
            IExprTranslateContext.IVariable var = InHolderSchemeInstance.GetTempVar(Key);
            if (var != null)
            {
                return new string[] { var.Name };
            }
            return new string[] { $"<<INVALID_TEMP_VAR_{Key}>>" };
        }

    }





}
