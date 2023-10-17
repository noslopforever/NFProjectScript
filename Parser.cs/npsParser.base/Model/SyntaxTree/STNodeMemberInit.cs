using System;
using System.Collections.Generic;

namespace nf.protoscript.syntaxtree
{
    /// <summary>
    /// Init, describe a variable init expression.
    /// 
    /// model mdl
    ///     - member = 100
    /// Here, the expr's purpose is to 'init' the member to 100, not 'set' the member to 100.
    /// So we need STNodeInit to describe this situition.
    /// </summary>
    public class STNodeMemberInit
        : STNodeBase
    {
        public STNodeMemberInit()
            : base()
        {
        }

        public STNodeMemberInit(ElementInfo InInfoToBeInit, ISyntaxTreeNode InRHS)
            : base()
        {
            InfoToBeInit = InInfoToBeInit;
            RHS = InRHS;
        }

        public override void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc(RHS)) { return; }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return InfoToBeInit.ElementType;
        }

        /// <summary>
        /// The element to be initialized.
        /// </summary>
        [Serialization.SerializableInfo]
        public ElementInfo InfoToBeInit { get; private set; }

        /// <summary>
        /// Right hand value
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode RHS { get; private set; }

        /// <summary>
        /// Type of the LHS variable
        /// </summary>
        public TypeInfo VarType
        {
            get
            {
                return InfoToBeInit.ElementType;
            }
        }


    }

}
