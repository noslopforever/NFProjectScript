using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Element-Define statement (member/child-elements/globals/parameters). 
    /// </summary>
    class STNode_ElementDef
        : STNode_DefBase
    {
        public STNode_ElementDef(string InElementName)
            : base(InElementName)
        {
        }

    }


    /// <summary>
    /// Attribute-statements.
    /// </summary>
    class STNode_ElementDefs
        : List<STNode_ElementDef>
        , syntaxtree.ISyntaxTreeNode
    {
        public void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            foreach (var node in this)
            {
                if (!InActionFunc(node)) { return; }
            }
        }

        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new InvalidProgramException();
        }
    }


}