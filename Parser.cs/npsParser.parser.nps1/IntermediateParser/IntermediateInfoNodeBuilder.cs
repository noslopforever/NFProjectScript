using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Manager to manage all IntermediateInfoNodeFactories.
    /// This class provides a singleton instance to register, query, and use factories to build intermediate information nodes.
    /// </summary>
    public sealed class IntermediateInfoNodeBuilder
    {
        // Private constructor to prevent multiple instances.
        private IntermediateInfoNodeBuilder()
        {
        }

        /// <summary>
        /// The singleton instance of the IntermediateInfoNodeBuilder. 
        /// </summary>
        public static IntermediateInfoNodeBuilder Instance { get; } = new IntermediateInfoNodeBuilder();

        /// <summary>
        /// Registers a factory with a factory name.
        /// </summary>
        /// <param name="InCmdName">The name associated with the factory.</param>
        /// <param name="InFactory">The factory to register.</param>
        public void RegisterFactory(string InCmdName, IIntermediateInfoNodeFactory InFactory)
        {
            _factoryTable.Add(InCmdName, InFactory);
        }

        /// <summary>
        /// Queries a factory by its factory name.
        /// </summary>
        /// <param name="InFactoryName">The name of the factory to query.</param>
        /// <returns>The factory if found; otherwise, null.</returns>
        public IIntermediateInfoNodeFactory QueryFactory(string InFactoryName)
        {
            if (_factoryTable.TryGetValue(InFactoryName, out var outFactory))
            {
                return outFactory;
            }

            return null;
        }

        /// <summary>
        /// Replaces a command alias in the original code with the corresponding command.
        /// </summary>
        /// <param name="InOriginCode">The original code containing the command alias.</param>
        /// <param name="OutCommand">The command after replacing the alias.</param>
        /// <returns>The filtered code after replacing the command alias.</returns>
        public string ReplaceCommandAlias(string InOriginCode, out string OutCommand)
        {
            foreach (var filterEntry in _filterTable)
            {
                string cmd = "";
                var filtedCode = _CheckCommandAlias(filterEntry, InOriginCode, out OutCommand);
                if (filtedCode != null)
                {
                    return filtedCode;
                }
            }

            OutCommand = "";
            return InOriginCode;
        }

        /// <summary>
        /// Parses a single line of code and builds an intermediate information node.
        /// </summary>
        /// <param name="InParentNode">The parent intermediate information node.</param>
        /// <param name="InCodesTrimmed">The trimmed line of code to parse.</param>
        /// <returns>The newly created intermediate information node, or null if the line is empty.</returns>
        public IIntermediateInfoNode ParseOneLine(IIntermediateInfoNode InParentNode, string InCodesTrimmed)
        {
            // empty line: Finish and seek to the next line.
            if (InCodesTrimmed == "")
            {
                return null;
            }

            // Translate header alias to header keywords.
            string command = "";
            string commandParams = ReplaceCommandAlias(InCodesTrimmed, out command);

            // If no command, take it as a instant text (like a special comment).
            if (command == "")
            {
                command = "AddText";
            }

            // Query a valid builder to build the IntermediateInfoNode
            var iiNodeBuilder = QueryFactory(command);
            var iiNode = iiNodeBuilder.BuildIntermediateInfoNode(commandParams);

            // Add the new IntermediateInfoNode to the parent node.
            InParentNode.AddSubIntermediateInfoNode(iiNode);

            return iiNode;
        }



        // Dictionary to store registered factories.
        Dictionary<string, IIntermediateInfoNodeFactory> _factoryTable = new Dictionary<string, IIntermediateInfoNodeFactory>();

        /// <summary>
        /// Structure to hold filter entries for command aliases.
        /// </summary>
        struct FilterEntry
        {
            public string FilterCode;
            public string OriginCode;
        }

        // List to store filter entries for command aliases.
        List<FilterEntry> _filterTable = new List<FilterEntry>();

        /// <summary>
        /// Checks if the command alias matches and returns the filtered code.
        /// </summary>
        /// <param name="InEntry">The filter entry to check.</param>
        /// <param name="InOriginCode">The original code to check against the filter entry.</param>
        /// <param name="OutCommand">The command after replacing the alias.</param>
        /// <returns>The filtered code if the alias matches; otherwise, null.</returns>
        string _CheckCommandAlias(FilterEntry InEntry, string InOriginCode, out string OutCommand)
        {
            if (InOriginCode.StartsWith(InEntry.FilterCode))
            {
                string otherCodes = InOriginCode.Substring(InEntry.FilterCode.Length);
                OutCommand = InEntry.OriginCode;
                return otherCodes;
            }
            OutCommand = null;
            return null;
        }


    }

}
