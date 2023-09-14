using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static nf.protoscript.translator.expression.ExprCodeGeneratorAbstract;

namespace nf.protoscript.translator.expression
{


    public partial class ExprTranslatorAbstract
    {

        /// <summary>
        /// A Visitor to walkthrough a syntaxtree and gather getter-translate-schemes for all nodes.
        /// </summary>
        public class STNodeVisitor_GetNodeValue
        {
            public STNodeVisitor_GetNodeValue(ExprTranslatorAbstract InHostTranslator, IExprTranslateContext InContext)
            {
                HostTranslator = InHostTranslator;
                ExprTranslateContext = InContext;
            }

            /// <summary>
            /// The host translator which create this visitor
            /// </summary>
            public ExprTranslatorAbstract HostTranslator { get; }

            /// <summary>
            /// Expr translate context.
            /// </summary>
            public IExprTranslateContext ExprTranslateContext { get; }

            /// <summary>
            /// The scheme to translate the visiting STNode, found by this visitor after Visit .
            /// </summary>
            public ISTNodeTranslateSchemeInstance ResultSchemeInstance { get; protected set; }

            /// <summary>
            /// The visiting STNode's scope (predict type) after Visit.
            /// </summary>
            public TypeInfo PredictScope { get; protected set; }

            /// <summary>
            /// Visit Constant Nodes.
            /// </summary>
            public virtual void Visit(STNodeConstant InConst)
            {
                if (InConst.Value == null)
                {
                    PredictScope = InConst.Type;
                    var scheme = HostTranslator.QueryNullScheme(InConst.Type);
                    ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ExprTranslateContext, InConst);
                }
                else if (InConst.Value.GetType().IsValueType)
                {
                    Type valueType = InConst.Value.GetType();

                    PredictScope = InConst.Type;
                    var scheme = HostTranslator.QueryConstGetScheme(InConst.Type, InConst.Value.ToString());
                    ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ExprTranslateContext, InConst);
                }
                else if (InConst.Value is string)
                {
                    PredictScope = InConst.Type;
                    var scheme = HostTranslator.QueryConstGetStringScheme(InConst.Type, InConst.Value as string);
                    ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ExprTranslateContext, InConst);
                }
                else if (InConst.Value is Info)
                {
                    PredictScope = InConst.Type;
                    var scheme = HostTranslator.QueryConstGetInfoScheme(InConst.Type, InConst.Value as Info);
                    ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ExprTranslateContext, InConst);
                }
                else
                {
                    ResultSchemeInstance = HostTranslator.NewErrorScheme(InConst)
                        .CreateInstance(HostTranslator, ExprTranslateContext, InConst)
                        ;
                }
            }

            public virtual void Visit(STNodeVar InVarNode)
            {
                TypeInfo varType = null;
                var scheme = HostTranslator.QueryVarGetScheme(PredictScope, InVarNode.IDName, out varType);
                ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ExprTranslateContext, InVarNode);
                PredictScope = varType;
            }

            public virtual void Visit(STNodeMemberAccess InSubNode)
            {
                // Visit host node first.
                STNodeVisitor_GetNodeValue hostVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InSubNode.LHS, hostVisitor);

                // Use host to find the best scheme for the member.
                TypeInfo varScope = null;
                var memberGetScheme = HostTranslator.QueryVarGetScheme(hostVisitor.PredictScope, InSubNode.MemberID, out varScope);
                ResultSchemeInstance = memberGetScheme.CreateInstance(HostTranslator, ExprTranslateContext, InSubNode);

                ResultSchemeInstance.AddPrerequisiteScheme("HOST", hostVisitor.ResultSchemeInstance);

                PredictScope = varScope;
            }
            public virtual void Visit(STNodeAssign InAssignNode)
            {
                // Order: Right > Left, Handle RHS first.
                STNodeVisitor_GetNodeValue rhsVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InAssignNode.RHS, rhsVisitor);

                // Handle LHS hands.
                STNodeVisitor_GatherSetScheme lhsVisitor = new STNodeVisitor_GatherSetScheme(HostTranslator, ExprTranslateContext, rhsVisitor.PredictScope);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InAssignNode.LHS, lhsVisitor);

                // Add RHS scheme to the LHS's prerequistites.
                lhsVisitor.ResultSchemeInstance.AddPrerequisiteScheme("RHS", rhsVisitor.ResultSchemeInstance);

                PredictScope = lhsVisitor.PredictScope;
                ResultSchemeInstance = lhsVisitor.ResultSchemeInstance;
            }
            public virtual void Visit(STNodeBinaryOp InBinOpNode)
            {
                STNodeVisitor_GetNodeValue lhsVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InBinOpNode.LHS, lhsVisitor);

                STNodeVisitor_GetNodeValue rhsVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InBinOpNode.RHS, rhsVisitor);

                // Construct op scheme and return.
                TypeInfo opResultType = null;
                var opScheme = HostTranslator.QueryBinOpScheme(InBinOpNode, lhsVisitor.PredictScope, rhsVisitor.PredictScope, out opResultType);
                ResultSchemeInstance = opScheme.CreateInstance(HostTranslator, ExprTranslateContext, InBinOpNode);

                ResultSchemeInstance.AddPrerequisiteScheme("LHS", lhsVisitor.ResultSchemeInstance);
                ResultSchemeInstance.AddPrerequisiteScheme("RHS", rhsVisitor.ResultSchemeInstance);

                PredictScope = opResultType;
            }
            public virtual void Visit(STNodeCall InCallNode)
            {
                throw new NotImplementedException();
            }
            public virtual void Visit(STNodeNew InNewNode)
            {
                throw new NotImplementedException();
            }
            public virtual void Visit(STNodeSequence InNodes)
            {
                throw new NotImplementedException();
            }

        }


        /// <summary>
        /// A Visitor to walkthrough a syntaxtree and gather setter-translate-schemes for all nodes.
        /// </summary>
        public class STNodeVisitor_GatherSetScheme
            : STNodeVisitor_GetNodeValue
        {
            public STNodeVisitor_GatherSetScheme(ExprTranslatorAbstract InHostTranslator, IExprTranslateContext InContext, TypeInfo InRHSScope)
                : base(InHostTranslator, InContext)
            {
                RHSScope = InRHSScope;
            }

            /// <summary>
            /// The RHS's Scope to set this STNode.
            /// </summary>
            public TypeInfo RHSScope { get; }

            public override void Visit(STNodeVar InVarNode)
            {
                // try find SET snippet for this the variable-node.
                TypeInfo varType = null;
                var scheme = HostTranslator.QueryVarSetScheme(PredictScope, InVarNode.IDName, out varType);
                ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ExprTranslateContext, InVarNode);
                PredictScope = varType;
            }

            public override void Visit(STNodeMemberAccess InSubNode)
            {
                // Visit host node first.
                STNodeVisitor_GatherRefScheme hostVisitor = new STNodeVisitor_GatherRefScheme(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InSubNode.LHS, hostVisitor);

                // Use host to find the best scheme for the member.
                TypeInfo memberType = null;
                var memberSetScheme = HostTranslator.QueryVarSetScheme(hostVisitor.PredictScope, InSubNode.MemberID, out memberType);
                ResultSchemeInstance = memberSetScheme.CreateInstance(HostTranslator, ExprTranslateContext, InSubNode);

                ResultSchemeInstance.AddPrerequisiteScheme("HOST", hostVisitor.ResultSchemeInstance);

                PredictScope = memberType;
            }

        }


        /// <summary>
        /// A Visitor to walkthrough a syntaxtree and gather ref-translate-schemes for all nodes.
        /// </summary>
        public class STNodeVisitor_GatherRefScheme
            : STNodeVisitor_GetNodeValue
        {
            public STNodeVisitor_GatherRefScheme(ExprTranslatorAbstract InHostTranslator, IExprTranslateContext InContext)
                : base(InHostTranslator, InContext)
            {
            }

            public override void Visit(STNodeVar InVarNode)
            {
                // try find REF snippet for this the variable-node.
                TypeInfo varType = null;
                var scheme = HostTranslator.QueryVarRefScheme(PredictScope, InVarNode.IDName, out varType);
                ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ExprTranslateContext, InVarNode);
                PredictScope = varType;
            }

            public override void Visit(STNodeMemberAccess InSubNode)
            {
                // Visit host node first.
                STNodeVisitor_GatherRefScheme hostVisitor = new STNodeVisitor_GatherRefScheme(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InSubNode.LHS, hostVisitor);

                // Use host to find the best scheme for the member.
                TypeInfo memberType = null;
                var memberSetScheme = HostTranslator.QueryVarRefScheme(hostVisitor.PredictScope, InSubNode.MemberID, out memberType);
                ResultSchemeInstance = memberSetScheme.CreateInstance(HostTranslator, ExprTranslateContext, InSubNode);

                ResultSchemeInstance.AddPrerequisiteScheme("HOST", hostVisitor.ResultSchemeInstance);

                PredictScope = memberType;
            }
        }



        /// <summary>
        /// Visitor to visit a statement node.
        /// </summary>
        public class STNodeVisitor_Statement
        {
            public STNodeVisitor_Statement(ExprTranslatorAbstract InHostTranslator, IExprTranslateContext InContext)
            {
                HostTranslator = InHostTranslator;
                ExprTranslateContext = InContext;
            }

            public ExprTranslatorAbstract HostTranslator { get; }
            public IExprTranslateContext ExprTranslateContext { get; }

            public List<ISTNodeTranslateSchemeInstance> TranslateSchemeInstances { get; } = new List<ISTNodeTranslateSchemeInstance>();

            public void Visit(ISyntaxTreeNode InOtherSTNode)
            {
                // Statement always starts with a getter visitor.
                STNodeVisitor_GetNodeValue valueNodeVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit(InOtherSTNode, valueNodeVisitor);
                TranslateSchemeInstances.Add(valueNodeVisitor.ResultSchemeInstance);
            }

            public void Visit(STNodeSequence InNodes)
            {
                foreach (var subNode in InNodes.NodeList)
                {
                    // Statement always starts with a getter visitor.
                    STNodeVisitor_Statement statementNodeVisitor = new STNodeVisitor_Statement(HostTranslator, ExprTranslateContext);
                    VisitByReflectionHelper.FindAndCallVisit(subNode, statementNodeVisitor);

                    // Merge translate-schemes of sub-statements.
                    TranslateSchemeInstances.AddRange(statementNodeVisitor.TranslateSchemeInstances);
                }
            }
        }

        /// <summary>
        /// Visitor to visit a function body.
        /// </summary>
        public class STNodeVisitor_FunctionBody
        {
            public STNodeVisitor_FunctionBody(ExprTranslatorAbstract InHostTranslator, IExprTranslateContext InContext)
            {
                HostTranslator = InHostTranslator;
                ExprTranslateContext = InContext;
            }

            public ExprTranslatorAbstract HostTranslator { get; }
            public IExprTranslateContext ExprTranslateContext { get; }
            public List<ISTNodeTranslateSchemeInstance> TranslateSchemeInstances { get; private set; } 

            /// <summary>
            /// Visit a single syntax-tree as function-body.
            /// </summary>
            /// <param name="InOtherSTNode"></param>
            public void Visit(ISyntaxTreeNode InOtherSTNode)
            {
                STNodeVisitor_Statement stmtVisitor = new STNodeVisitor_Statement(HostTranslator, ExprTranslateContext);
                VisitByReflectionHelper.FindAndCallVisit(InOtherSTNode, stmtVisitor);
                TranslateSchemeInstances = stmtVisitor.TranslateSchemeInstances;
            }

        }


    }



}
