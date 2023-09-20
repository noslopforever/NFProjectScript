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
            // Find env-variables
            var envVar = InHolderSchemeInstance.FindEnvVariable(Key);
            if (envVar != null)
            {
                return envVar.ToString();
            }

            // Find prerequisite
            var schemeInst = InHolderSchemeInstance.FindPrerequisite(Key);
            if (schemeInst != null)
            {
                var result = schemeInst.GetResult(StageName);
                if (result.Count > 1)
                {
                    // TODO log error
                    throw new InvalidOperationException();
                }
                return result[0];
            }

            return "<<INVALID_SUB_VALUE>>";
        }
    }




}
