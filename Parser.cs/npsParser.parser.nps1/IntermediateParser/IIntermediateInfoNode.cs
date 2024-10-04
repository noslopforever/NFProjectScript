using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Defines a contract for an intermediate information node in the syntax tree.
    /// These nodes are used to represent intermediate information that is not directly part of the final syntax tree,
    /// but is useful during the parsing and analysis phases.
    /// </summary>
    public interface IIntermediateInfoNode
    {
        /// <summary>
        /// Gets the parent intermediate information node.
        /// </summary>
        IIntermediateInfoNode ParentIntermediateInfoNode { get; }

        /// <summary>
        /// Gets a read-only list of sub-intermediate information nodes.
        /// </summary>
        IReadOnlyList<IIntermediateInfoNode> SubIntermediateInfoNodes { get; }

        /// <summary>
        /// Gets a unique identifier for debugging purposes.
        /// </summary>
        string DebugID { get; }

        /// <summary>
        /// Gets a trivial debug information string, which can be used for quick inspection or logging.
        /// </summary>
        string TrivialDebugInfo { get; }

        /// <summary>
        /// Adds a sub-intermediate information node to the current node.
        /// </summary>
        /// <param name="InSubNode">The sub-intermediate information node to add.</param>
        void AddSubIntermediateInfoNode(IIntermediateInfoNode InSubNode);

    }

}
