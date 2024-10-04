using System.Collections.Generic;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// A parser to parse a NPS code block with multiple lines of NPS codes.
    /// This class is responsible for parsing the NPS code and building the intermediate information nodes.
    /// </summary>
    public class NPSCodeParser
    {

        /// <summary>
        /// Parses the provided NPS codes into the target project.
        /// </summary>
        /// <param name="InProjectInfo">The project information to which the parsed nodes will be added.</param>
        /// <param name="InReader">The reader that provides the NPS code content.</param>
        public void Parse(ProjectInfo InProjectInfo, ICodeContentReader InReader)
        {
            InReader.GoStart();

            // Parse each line as a command
            do
            {
                // Check indent
                int indent = 0;
                string codesTrimmed = NPSCodeParserHelper.TrimCodes(InReader.CurrentCodeLine.Content, out indent);

                // Empty line: Skip and move to the next line.
                if (codesTrimmed == "")
                {
                    continue;
                }

                // Find the parent node based on the current indent.
                IIntermediateInfoNode parentIINode = null;
                var parentIINodeWithIndent = _FindParentContext(indent);
                if (parentIINodeWithIndent == null)
                {
                    parentIINode = parentIINodeWithIndent.IntermediateInfoNode;
                }

                // Try building an intermediate info node.
                var iiNode = IntermediateInfoNodeBuilder.Instance.ParseOneLine(parentIINode, codesTrimmed);

                // Save the new node with its indent.
                var iidNodeWithIndent = new IntermediateInfoNodeWithIndent(indent, iiNode);
                _parsingIntermediateInfoNodes.Add(iidNodeWithIndent);
            }
            while (InReader.IsEndOfFile);
        }

        /// <summary>
        /// Finds the parent context based on the current indent.
        /// </summary>
        /// <param name="InIndent">The current indent level.</param>
        /// <returns>The parent context with the corresponding indent, or null if no parent is found.</returns>
        private IntermediateInfoNodeWithIndent _FindParentContext(int InIndent)
        {
            for (int i = _parsingIntermediateInfoNodes.Count - 1; i >= 0; i--)
            {
                var checkCtx = _parsingIntermediateInfoNodes[i];
                if (checkCtx.Indent < InIndent)
                {
                    return checkCtx;
                }
            }
            return null;
        }

        /// <summary>
        /// Represents an intermediate information node with its associated indent level.
        /// </summary>
        class IntermediateInfoNodeWithIndent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="IntermediateInfoNodeWithIndent"/> class.
            /// </summary>
            /// <param name="InIndent">The indent level of the node.</param>
            /// <param name="InIntermediateInfoNode">The intermediate information node.</param>
            public IntermediateInfoNodeWithIndent(int InIndent, IIntermediateInfoNode InIntermediateInfoNode)
            {
                Indent = InIndent;
            }

            /// <summary>
            /// Gets the indent level of the node.
            /// </summary>
            public int Indent { get; }

            /// <summary>
            /// Gets the intermediate information node.
            /// </summary>
            public IIntermediateInfoNode IntermediateInfoNode { get; }

        }

        /// <summary>
        /// A list to store the intermediate information nodes with their associated indent levels.
        /// </summary>
        List<IntermediateInfoNodeWithIndent> _parsingIntermediateInfoNodes = new List<IntermediateInfoNodeWithIndent>();

    }

}