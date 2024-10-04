using nf.protoscript.syntaxtree;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Represents a function definition statement.
    /// </summary>
    [VirtualSTNode]
    class STNode_FunctionDef
        : STNode_DefBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_FunctionDef"/> class with the specified function name and parameter definitions.
        /// </summary>
        /// <param name="InFunctionName">The name of the function.</param>
        /// <param name="InParamDefs">The list of parameter definitions for the function.</param>
        public STNode_FunctionDef(string InFunctionName, List<STNode_ElementDef> InParamDefs)
            : base(InFunctionName)
        {
            Params = InParamDefs;
        }

        /// <summary>
        /// Gets the return type of the function. This is an alias for the <see cref="TypeSig"/> property.
        /// </summary>
        public STNode_TypeSignature ReturnType { get { return TypeSig; } }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        public IReadOnlyList<STNode_ElementDef> Params { get; }

        /// <summary>
        /// Gets the inline function body.
        /// </summary>
        public ISyntaxTreeNode FunctionBody
        {
            get
            {
                return InitExpression;
            }
        }

    }

}