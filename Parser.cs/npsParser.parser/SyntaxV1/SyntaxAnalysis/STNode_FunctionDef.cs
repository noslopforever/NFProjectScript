using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Function-Define statement.
    /// </summary>
    class STNode_FunctionDef
        : STNode_DefBase
    {
        public STNode_FunctionDef(string InFunctionName, List<STNode_ElementDef> InParamDefs)
            : base(InFunctionName)
        {
            Params = InParamDefs;
        }

        /// <summary>
        /// Return type of the function.
        /// </summary>
        public STNode_TypeSignature ReturnType { get { return TypeSig; } }

        /// <summary>
        /// Parameters of the function.
        /// </summary>
        public IReadOnlyList<STNode_ElementDef> Params { get; }

        /// <summary>
        /// The Inline function body.
        /// </summary>
        public syntaxtree.STNodeBase FunctionBody
        {
            get
            {
                return InitExpression;
            }
        }

    }

}