using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// The default implementation of the <see cref="IIntermediateInfoNode"/> interface.
    /// This abstract class provides a base implementation for intermediate information nodes.
    /// </summary>
    public abstract class IntermediateInfoNode_Base
        : IIntermediateInfoNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntermediateInfoNode_Base"/> class with the specified parent node.
        /// </summary>
        /// <param name="InParentNode">The parent intermediate information node.</param>
        public IntermediateInfoNode_Base(IIntermediateInfoNode InParentNode)
        {
            ParentIntermediateInfoNode = InParentNode;
        }

        /// <inheritdoc />
        public IIntermediateInfoNode ParentIntermediateInfoNode { get; }

        /// <inheritdoc />
        public IReadOnlyList<IIntermediateInfoNode> SubIntermediateInfoNodes
        { get { return _subNodes; } }

        /// <inheritdoc />
        public string DebugID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public string TrivialDebugInfo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public void AddSubIntermediateInfoNode(IIntermediateInfoNode InSubNode)
        {
            if (InSubNode.ParentIntermediateInfoNode != this)
            {
                throw new ArgumentException($"The input sub node ({InSubNode}) has already been attached to the target node: {InSubNode.ParentIntermediateInfoNode}");
            }

            _subNodes.Add(InSubNode);
        }


        // private fields.
        List<IIntermediateInfoNode> _subNodes = new List<IIntermediateInfoNode>();

    }

}
