using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Base of all 'DEFINE' statement, like def HP = 0 / def call(A, B) / [Attribute] Name:string = "" / Label = $db{Path=Name, Source=This}
    /// </summary>
    abstract class STNode_DefBase
        : syntaxtree.ISyntaxTreeNode
    {
        protected STNode_DefBase(string InDefName)
        {
            DefName = InDefName;
        }

        /// <summary>
        /// Attributes to decorate the definition.
        /// </summary>
        public STNode_AttributeDefs Attributes { get; } = new STNode_AttributeDefs();

        /// <summary>
        /// Name of the definition.
        /// </summary>
        public string DefName { get; }

        /// <summary>
        /// Init expression of the definition.
        /// </summary>
        public syntaxtree.STNodeBase InitExpression { get; private set; }

        /// <summary>
        /// Type of the element.
        /// </summary>
        public STNode_TypeSignature TypeSig { get; private set; }

        internal virtual void _Internal_SetType(STNode_TypeSignature InTypeSig)
        {
            TypeSig = InTypeSig;
        }

        internal virtual void _Internal_SetInitExpr(syntaxtree.STNodeBase InInitExpr)
        {
            InitExpression = InInitExpr;
        }

        internal void _Internal_AddAttributes(STNode_AttributeDefs InAttrs)
        {
            Attributes.AddRange(InAttrs);
        }

        public void ForeachSubNodes(Func<ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc(TypeSig)) { return; }
            if (!InActionFunc(InitExpression)) { return; }
            if (!InActionFunc(Attributes)) { return; }
        }

        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new NotImplementedException();
        }

    }

}