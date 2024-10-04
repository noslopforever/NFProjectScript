using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Represents an intermediate information node that holds documents.
    /// This class extends the <see cref="IntermediateInfoNode_Base"/> class and adds the ability to store and manage document content.
    /// </summary>
    public class IntermediateInfoNode_Document
        : IntermediateInfoNode_Base
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IntermediateInfoNode_Document"/> class with the specified parent node.
        /// </summary>
        /// <param name="InParentNode">The parent intermediate information node.</param>
        public IntermediateInfoNode_Document(IIntermediateInfoNode InParentNode)
            : base(InParentNode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntermediateInfoNode_Document"/> class with the specified parent node and initial document content.
        /// </summary>
        /// <param name="InParentNode">The parent intermediate information node.</param>
        /// <param name="InDocuments">The initial document content to set.</param>
        public IntermediateInfoNode_Document(IIntermediateInfoNode InParentNode, string InDocuments)
            : base(InParentNode)
        {
            Documents = InDocuments;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntermediateInfoNode_Document"/> class with the specified parent node and initial document content.
        /// </summary>
        /// <param name="InParentNode">The parent intermediate information node.</param>
        /// <param name="InDocuments">The initial document content to set.</param>
        public string Documents { get; private set; }

        /// <summary>
        /// Sets the document content.
        /// </summary>
        /// <param name="InDocuments">The document content to set.</param>
        internal void _Internal_SetDocuments(string InDocuments)
        {
            Documents = InDocuments;
        }

    }

}
