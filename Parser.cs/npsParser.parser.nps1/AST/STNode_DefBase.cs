using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Base of all 'DEFINE' statements.
    /// Like def HP = 0 / def call(A, B) / [Attribute] Name:string = "" / Label = $db{Path=Name, Source=This}
    /// </summary>
    [VirtualSTNode]
    abstract class STNode_DefBase
        : ISyntaxTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_DefBase"/> class with the specified definition name.
        /// </summary>
        /// <param name="InDefName">The name of the definition.</param>
        protected STNode_DefBase(string InDefName)
        {
            DefName = InDefName;
        }

        /// <summary>
        /// Gets the attributes to decorate the definition.
        /// </summary>
        public STNode_AttributeDefs Attributes { get; } = new STNode_AttributeDefs();

        /// <summary>
        /// Gets the name of the definition.
        /// </summary>
        public string DefName { get; }

        /// <summary>
        /// Gets the initialization expression of the definition.
        /// </summary>
        public ISyntaxTreeNode InitExpression { get; private set; }

        /// <summary>
        /// Gets the type signature of the element.
        /// </summary>
        public STNode_TypeSignature TypeSig { get; private set; }

        /// <summary>
        /// Internally sets the type signature of the definition.
        /// </summary>
        /// <param name="InTypeSig">The type signature to set.</param>
        internal virtual void _Internal_SetType(STNode_TypeSignature InTypeSig)
        {
            TypeSig = InTypeSig;
        }

        /// <summary>
        /// Internally sets the initialization expression of the definition.
        /// </summary>
        /// <param name="InInitExpr">The initialization expression to set.</param>
        internal virtual void _Internal_SetInitExpr(ISyntaxTreeNode InInitExpr)
        {
            InitExpression = InInitExpr;
        }

        /// <summary>
        /// Internally adds a collection of attributes to the existing attributes.
        /// </summary>
        /// <param name="InAttrs">The attributes to add.</param>
        internal void _Internal_AddAttributes(STNode_AttributeDefs InAttrs)
        {
            Attributes.AddRange(InAttrs);
        }

        /// <inheritdoc />
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            if (!InActionFunc("TypeSig", TypeSig)) { return; }
            if (!InActionFunc("InitExpr", InitExpression)) { return; }
            if (!InActionFunc("Attributes", Attributes)) { return; }
        }

        /// <inheritdoc />
        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new NotImplementedException();
        }

    }

}