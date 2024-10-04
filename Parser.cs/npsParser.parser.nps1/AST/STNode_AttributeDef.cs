using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Represents a single attribute definition statement.
    /// </summary>
    [VirtualSTNode]
    class STNode_AttributeDef
        : STNode_DefBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_AttributeDef"/> class with the specified attribute name.
        /// </summary>
        /// <param name="InAttributeName">The name of the attribute.</param>
        public STNode_AttributeDef(string InAttributeName)
            : base(InAttributeName)
        {
        }

        /// <summary>
        /// Throws an exception because setting the type of an attribute definition is not allowed.
        /// </summary>
        /// <param name="InTypeSig">The type signature to set (not used).</param>
        internal override void _Internal_SetType(STNode_TypeSignature InTypeSig)
        {
            throw new InvalidProgramException("InvalidCall: Cannot set the type of attribute-definition");
        }

    }

    /// <summary>
    /// Represents a collection of attribute definition statements.
    /// </summary>
    [VirtualSTNode]
    class STNode_AttributeDefs
        : List<STNode_AttributeDef>
        , ISyntaxTreeNode
    {
        /// <summary>
        /// Iterates over each attribute definition and applies the provided action function.
        /// </summary>
        /// <param name="InActionFunc">The action function to apply to each attribute definition. The function takes a key and a node, and returns a boolean indicating whether to continue iteration.</param>
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var node = this[i];
                var key = $"Attribute{i}";
                if (!InActionFunc(key, node)) { return; }
            }
        }

        /// <summary>
        /// Gets the predicted type of the attribute definitions. This method is not implemented and will throw an exception.
        /// </summary>
        /// <param name="InHostElemInfo">The element information of the host (not used).</param>
        /// <returns>The predicted type (not implemented).</returns>
        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new InvalidProgramException();
        }
    }

}