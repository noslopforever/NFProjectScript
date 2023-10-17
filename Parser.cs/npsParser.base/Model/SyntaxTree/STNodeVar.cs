
using System;
using System.Collections.Generic;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// expr node: identifier.
    /// </summary>
    public class STNodeVar
        : STNodeBase
    {
        internal STNodeVar()
        {
        }

        public STNodeVar(string InIdName)
        {
            IDName = InIdName;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            // TODO Find var in scope chain: so scope must be decided by the Info system, not the translator.
            throw new NotImplementedException();
        } 

        /// <summary>
        /// The identifier's name: variable's name, member's name, and so on...
        /// </summary>
        [Serialization.SerializableInfo]
        public string IDName { get; private set; }

    }
}