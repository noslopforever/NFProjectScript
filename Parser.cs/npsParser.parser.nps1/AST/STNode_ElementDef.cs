using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Represents an element definition statement (e.g., member, child elements, globals, parameters).
    /// </summary>
    [VirtualSTNode]
    class STNode_ElementDef
        : STNode_DefBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_ElementDef"/> class with the specified element name.
        /// </summary>
        /// <param name="InElementName">The name of the element.</param>
        public STNode_ElementDef(string InElementName)
            : base(InElementName)
        {
        }

    }


    /// <summary>
    /// Represents a collection of element definition statements.
    /// </summary>
    [VirtualSTNode]
    class STNode_ElementDefs
        : List<STNode_ElementDef>
        , ISyntaxTreeNode
    {
        /// <inheritdoc />
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var node = this[i];
                var key = $"Element{i}";
                if (!InActionFunc(key, node)) { return; }
            }
        }

        /// <inheritdoc />
        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new InvalidProgramException();
        }

    }

}