using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Defines a factory interface for building an intermediate information node from a command-line text.
    /// </summary>
    public interface IIntermediateInfoNodeFactory
    {

        /// <summary>
        /// Builds an intermediate information node from the provided command-line text.
        /// </summary>
        /// <param name="InCommandLine">The command-line text to parse and convert into an intermediate information node.</param>
        /// <returns>An instance of <see cref="IIntermediateInfoNode"/> representing the parsed intermediate information.</returns>
        IIntermediateInfoNode BuildIntermediateInfoNode(string InCommandLine);

    }

}
