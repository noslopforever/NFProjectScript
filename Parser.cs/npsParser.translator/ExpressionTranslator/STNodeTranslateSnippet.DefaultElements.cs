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
        public string Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
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

        public string Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            return Value;
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

        public string Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            ISyntaxTreeNode nodeToTranslate = InHolderSchemeInstance.NodeToTranslate;
            if (nodeToTranslate is STNodeVar)
            {
                return (nodeToTranslate as STNodeVar).IDName;
            }
            else if (nodeToTranslate is STNodeMemberAccess)
            {
                return (nodeToTranslate as STNodeMemberAccess).MemberID;
            }

            // TODO log error
            throw new InvalidCastException();
            return "<<ERROR NODE TYPE>>";
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

        public string Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            ISyntaxTreeNode nodeToTranslate = InHolderSchemeInstance.NodeToTranslate;
            if (nodeToTranslate is STNodeConstant)
            {
                return (nodeToTranslate as STNodeConstant).Value.ToString();
            }

            // TODO log error
            throw new InvalidCastException();
            return "<<ERROR NODE TYPE>>";
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

        public string Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            return InHolderSchemeInstance.GetVarValue(Key, StageName);
        }
    }


    /// <summary>
    /// Add Temporary variable in current Context.
    /// </summary>
    public class ElementTempVar
        : STNodeTranslateSnippet.IElement
    {
        //public ElementTempVar(string InKey)
        //{
        //    Key = InKey;
        //}
        public ElementTempVar(string InKey, STNodeTranslateSnippet InInitSnippet)
        {
            Key = InKey;
            InitSnippet = InInitSnippet;
        }
        public ElementTempVar(string InKey, params STNodeTranslateSnippet.IElement[] InInitElements)
        {
            Key = InKey;
            InitSnippet = new STNodeTranslateSnippet(InInitElements);
        }

        /// <summary>
        /// Key name of the temp-var
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Init snippet for the temp-var
        /// </summary>
        public STNodeTranslateSnippet InitSnippet { get; }

        public string Apply(ISTNodeTranslateSchemeInstance InHolderSchemeInstance)
        {
            IExprTranslateContext.IVariable var = InHolderSchemeInstance.EnsureTempVar(Key, InHolderSchemeInstance.NodeToTranslate, InitSnippet);
            return var.Name;
        }
    }




}
