using System;
using System.Collections.Generic;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// Represents an if-statement node in a syntax tree.
    /// </summary>
    public class STNodeIf
        : STNodeBase
    {
        internal STNodeIf()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="STNodeIf" /> class with the given condition expression.
        /// The 'Then' and 'Else' sequences are initialized to empty sequences.
        /// </summary>
        /// <param name="InConditionExpr">Theh condition expression for the if-statement.</param>
        public STNodeIf(ISyntaxTreeNode InConditionExpr)
        {
            ConditionExpr = InConditionExpr;
            ThenSequence = new STNodeSequence(new ISyntaxTreeNode[0]);
            ElseSequence = new STNodeSequence(new ISyntaxTreeNode[0]);
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="STNodeIf"/> class with the given condition expression, 'Then' sequence and 'Else' sequence.
        /// </summary>
        /// <param name="InConditionExpr">The condition for the if-statement.</param>
        /// <param name="InThenSequence">The sequence to set as the 'Then' sequence of the if-statement.</param>
        /// <param name="InElseSequence">The sequence to set as the 'Else' sequence of the if-statement.</param>
        public STNodeIf(ISyntaxTreeNode InConditionExpr, ISyntaxTreeNode[] InThenSequence, ISyntaxTreeNode[] InElseSequence)
        {
            ConditionExpr = InConditionExpr;
            ThenSequence = new STNodeSequence(InThenSequence);
            ElseSequence = new STNodeSequence(InElseSequence);
        }

        /// <inheritdoc />
        public override void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            InActionFunc("ConditionExpr", ConditionExpr);
            InActionFunc("ThenSequence", ThenSequence);
            InActionFunc("ElseSequence", ElseSequence);
        }

        /// <inheritdoc />
        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            // TODO if statement will not return any valid Type.
            throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Sets the 'Then' sequence for the if-statement.
        /// </summary>
        /// <param name="InSubExprs">The array of sub-expressions to set as the 'Then' sequence.</param>
        public void SetThenSequence(params ISyntaxTreeNode[] InSubExprs)
        {
            ThenSequence = new STNodeSequence(InSubExprs);
        }

        /// <summary>
        /// Sets the 'Else' sequence for the if-statement.
        /// </summary>
        /// <param name="InSubExprs">The array of sub-expressions to set as the 'Else' sequence.</param>
        public void SetElseSequence(params ISyntaxTreeNode[] InSubExprs)
        {
            ElseSequence = new STNodeSequence(InSubExprs);
        }

        /// <summary>
        /// Gets or sets the condition expression for the if-statement.
        /// This property is marked for serialization.
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode ConditionExpr { get; private set; }

        /// <summary>
        /// Gets or sets the 'Then' sequence for the if-statement.
        /// This property is marked for serialization.
        /// </summary>
        [Serialization.SerializableInfo]
        public STNodeSequence ThenSequence { get; private set; }

        /// <summary>
        /// Gets or sets the 'Else' sequence for the if-statement.
        /// This property is marked for serialization.
        /// </summary>
        [Serialization.SerializableInfo]
        public STNodeSequence ElseSequence { get; private set; }

    }

}
