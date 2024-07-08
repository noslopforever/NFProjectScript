using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Attribute define statement.
    /// </summary>
    class STNode_AttributeDef
        : STNode_DefBase
    {
        public STNode_AttributeDef(string InAttributeName)
            : base(InAttributeName)
        {
        }

        internal override void _Internal_SetType(STNode_TypeSignature InTypeSig)
        {
            throw new InvalidProgramException("InvalidCall: Cannot set the type of attribute-definition");
        }

    }

    /// <summary>
    /// Attribute-statements.
    /// </summary>
    class STNode_AttributeDefs
        : List<STNode_AttributeDef>
        , syntaxtree.ISyntaxTreeNode
    {
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var node = this[i];
                var key = $"Attribute{i}";
                if (!InActionFunc(key, node)) { return; }
            }
        }

        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new InvalidProgramException();
        }
    }

}