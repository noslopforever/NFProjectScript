using System.Collections.Generic;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// STNode sequence.
    /// 
    /// local a, b, c = 0;
    /// a = b + c;
    /// return c;
    /// 
    /// </summary>
    public class STNodeSequence : STNodeBase
    {
        internal STNodeSequence()
        {
        }

        public STNodeSequence(params ISyntaxTreeNode[] InSTNodes)
        {
            NodeList = InSTNodes;
        }

        /// <summary>
        /// Sub STNode list.
        /// </summary>
        [Serialization.SerializableInfo]
        public IReadOnlyList<ISyntaxTreeNode> NodeList { get; private set; }

    }

}
