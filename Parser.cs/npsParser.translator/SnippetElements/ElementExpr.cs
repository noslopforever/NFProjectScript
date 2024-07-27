﻿using nf.protoscript.syntaxtree;
using nf.protoscript.translator.DefaultSnippetElements.Internal;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.DefaultSnippetElements
{

    /// <summary>
    /// Represents an element implemented by expressions.
    /// </summary>
    public class ElementExpr
        : InfoTranslateSnippet.IElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementExpr"/> class with the specified expression code.
        /// </summary>
        /// <param name="InExprCode">The original expression code.</param>
        public ElementExpr(string InExprCode)
        {
            ExprCode = InExprCode;
        }

        /// <summary>
        /// Creates a test expression element.
        /// </summary>
        /// <param name="InExpr">The syntax tree node representing the expression.</param>
        /// <returns>A new <see cref="ElementExpr"/> instance configured for testing.</returns>
        public static ElementExpr TEST_CreateTestExpr(ISyntaxTreeNode InExpr)
        {
            return new ElementExpr("__Test_Expr__") { Expression = InExpr };
        }

        /// <summary>
        /// Gets the original expression code.
        /// </summary>
        public string ExprCode { get; }

        /// <summary>
        /// Gets the expression parsed from the ExprCode.
        /// </summary>
        public ISyntaxTreeNode Expression { get; private set; } = null;

        // Begin IElement interfaces

        /// <see cref="IElement.Apply"/>
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            // Try parsing expressions from the ExprCode if it has not been done yet.
            if (Expression == null)
            {
                Expression = ParseExpression(ExprCode);
            }

            // Execute the translator expressions
            var exprResult = Execute(InHolderSchemeInstance, Expression);
            if (exprResult is IReadOnlyList<string>)
            {
                return exprResult as IReadOnlyList<string>;
            }
            return new string[] { exprResult.ToString() };
        }

        // ~ End IElement interfaces

        /// <summary>
        /// Parses the expression code into a syntax tree node.
        /// </summary>
        /// <param name="InCode">The expression code to parse.</param>
        /// <returns>The parsed syntax tree node.</returns>
        private static ISyntaxTreeNode ParseExpression(string InCode)
        {
            // TODO: Implement parsing logic.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes an expression statement within the given translation scheme instance.
        /// </summary>
        /// <param name="InHolderSchemeInstance">The instance of the translation scheme.</param>
        /// <param name="InStatement">The expression to execute.</param>
        /// <returns>The result of the expression execution.</returns>
        private static object Execute(IInfoTranslateSchemeInstance InHolderSchemeInstance, ISyntaxTreeNode InStatement)
        {
            ExprExecutor executor = new ExprExecutor();
            object val = executor.EvalValue(InHolderSchemeInstance, InStatement);
            return val;
        }
    }

}