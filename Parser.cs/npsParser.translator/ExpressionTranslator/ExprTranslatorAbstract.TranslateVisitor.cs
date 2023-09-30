using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static nf.protoscript.translator.expression.ExprTranslatorAbstract;
using System.Runtime.CompilerServices;

namespace nf.protoscript.translator.expression
{


    public partial class ExprTranslatorAbstract
    {

        /// <summary>
        /// A Visitor to walkthrough a syntaxtree and gather getter-translate-schemes for all nodes.
        /// </summary>
        public class STNodeVisitor_GetNodeValue
        {
            public STNodeVisitor_GetNodeValue(ExprTranslatorAbstract InHostTranslator, ITranslatingContext InParentContext)
            {
                HostTranslator = InHostTranslator;
                ParentContext = InParentContext;
            }

            /// <summary>
            /// The host translator which create this visitor
            /// </summary>
            public ExprTranslatorAbstract HostTranslator { get; }

            /// <summary>
            /// Translating Context.
            /// </summary>
            public ITranslatingContext ParentContext { get; }

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
                ITranslatingContext ctx = null;
                ISTNodeTranslateScheme scheme = null;
                if (InConst.Value == null)
                {
                    ctx = ConstNodeContext.Null(ParentContext, InConst);
                    scheme = HostTranslator.FindBestScheme(ctx, SystemScheme_Const);
                    PredictScope = InConst.Type;
                }
                else if (InConst.Value.GetType().IsValueType)
                {
                    Type valueType = InConst.Value.GetType();
                    ctx = ConstNodeContext.Const(ParentContext, InConst, InConst.Value.ToString());
                    scheme = HostTranslator.FindBestScheme(ctx, SystemScheme_Const);
                    PredictScope = InConst.Type;
                }
                else if (InConst.Value is string)
                {
                    ctx = ConstNodeContext.Const(ParentContext, InConst, InConst.Value as string);
                    scheme = HostTranslator.FindBestScheme(ctx, SystemScheme_Const);
                    PredictScope = InConst.Type;
                }
                else if (InConst.Value is Info)
                {
                    ctx = ConstNodeContext.Const(ParentContext, InConst, InConst.Value as Info);
                    scheme = HostTranslator.FindBestScheme(ctx, SystemScheme_Const);
                    PredictScope = InConst.Type;
                }
                else
                {
                    ctx = new OnlyNodeContext(ParentContext, InConst);
                    scheme = HostTranslator.FindBestScheme(ctx, SystemScheme_Error);
                    PredictScope = null;
                }
                ResultSchemeInstance = scheme.CreateInstance(HostTranslator, ctx);
            }

            public virtual void Visit(STNodeVar InVarNode)
            {
                LoadVarAccessSI(InVarNode, EExprVarAccessType.Get, out var outSI, out var outPredType);
                ResultSchemeInstance = outSI;
                PredictScope = outPredType;
            }

            public virtual void Visit(STNodeMemberAccess InSubNode)
            {
                // Visit host node first.
                STNodeVisitor_GetNodeValue hostVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ParentContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InSubNode.LHS, hostVisitor);

                // Use host to find the best scheme for Accessing the member.
                LoadMemberAccessSI(hostVisitor.PredictScope, InSubNode, EExprVarAccessType.Get, out var outVarType, out var outSI);
                outSI.AddPrerequisiteScheme("HOST", hostVisitor.ResultSchemeInstance);

                ResultSchemeInstance = outSI;
                PredictScope = outVarType;
            }

            public virtual void Visit(STNodeAssign InAssignNode)
            {
                // Order: Right > Left, Handle RHS first.
                STNodeVisitor_GetNodeValue rhsVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ParentContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InAssignNode.RHS, rhsVisitor);

                // Handle LHS hands. Visit it by 'set' visitor.
                STNodeVisitor_GatherSetScheme lhsVisitor = new STNodeVisitor_GatherSetScheme(HostTranslator, ParentContext, rhsVisitor.PredictScope);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InAssignNode.LHS, lhsVisitor);

                // Add RHS scheme to the LHS's prerequistites.
                lhsVisitor.ResultSchemeInstance.AddPrerequisiteScheme("RHS", rhsVisitor.ResultSchemeInstance);

                PredictScope = lhsVisitor.PredictScope;
                ResultSchemeInstance = lhsVisitor.ResultSchemeInstance;
            }
            public virtual void Visit(STNodeBinaryOp InBinOpNode)
            {
                STNodeVisitor_GetNodeValue lhsVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ParentContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InBinOpNode.LHS, lhsVisitor);

                STNodeVisitor_GetNodeValue rhsVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, ParentContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InBinOpNode.RHS, rhsVisitor);

                // Construct op scheme and return.
                var ctx = new OnlyNodeContext(ParentContext, InBinOpNode);
                var opScheme = HostTranslator.FindBestScheme(ctx, SystemScheme_BinOp);
                ResultSchemeInstance = opScheme.CreateInstance(HostTranslator, ctx);

                ResultSchemeInstance.AddPrerequisiteScheme("LHS", lhsVisitor.ResultSchemeInstance);
                ResultSchemeInstance.AddPrerequisiteScheme("RHS", rhsVisitor.ResultSchemeInstance);

                // TODO Let Parser decide the result type of a bin-op.
                //throw new NotImplementedException();
                PredictScope = CommonTypeInfos.Any;
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

            protected void LoadVarAccessSI(STNodeVar InVarNode, EExprVarAccessType InAccessType, out ISTNodeTranslateSchemeInstance OutSchemeInstance, out TypeInfo OutPredictType)
            {

                // Try find the variable in the environment's scope-chain.
                var hostScopeVar = ParentContext.RootEnvironment.FindVariable(InVarNode.IDName);
                if (hostScopeVar == null)
                {
                    OutPredictType = null;
                    var errorCtx = new OnlyNodeContext(ParentContext, InVarNode);
                    var scheme = HostTranslator.FindBestScheme(errorCtx, SystemScheme_Error);
                    OutSchemeInstance = scheme.CreateInstance(HostTranslator, errorCtx);
                    return;
                }

                TypeInfo varType = hostScopeVar.VarType;
                var ctx = new VarContext(ParentContext, InVarNode, hostScopeVar);
                ISTNodeTranslateScheme varAccessScheme = HostTranslator.FindBestScheme(ctx, SystemScheme_VarAccess(InAccessType));

                // Create scheme instance
                OutSchemeInstance = varAccessScheme.CreateInstance(HostTranslator, ctx);

                // -- Host will be set by the contexts and environments
                //// Set the host's present code to the '%{Host}%' variable.
                //OutSchemeInstance.SetEnvVariable("HOST", hostScopeVar.HostScope.ScopePresentCode);

                OutPredictType = varType;
            }

            protected void LoadMemberAccessSI(
                TypeInfo InHostPredictType
                , STNodeMemberAccess InMemberAccess
                , EExprVarAccessType InAccessType
                , out TypeInfo OutMemberType
                , out ISTNodeTranslateSchemeInstance OutSI
                )
            {
                var elemInfo = InfoHelper.FindPropertyOfType(InHostPredictType, InMemberAccess.MemberID);
                if (elemInfo == null)
                {
                    // TODO log warning: cannot find the property in type.
                    OutMemberType = CommonTypeInfos.Any;
                }
                else
                {
                    OutMemberType = elemInfo.ElementType;
                }

                var ctx = new MemberContext(ParentContext, InMemberAccess, elemInfo);
                var scheme = HostTranslator.FindBestScheme(ctx, SystemScheme_VarAccess(InAccessType));
                OutSI = scheme.CreateInstance(HostTranslator, ctx);
            }


        }

        /// <summary>
        /// A Visitor to walkthrough a syntaxtree and gather setter-translate-schemes for all nodes.
        /// </summary>
        public class STNodeVisitor_GatherSetScheme
            : STNodeVisitor_GetNodeValue
        {
            public STNodeVisitor_GatherSetScheme(ExprTranslatorAbstract InHostTranslator, ITranslatingContext InContext, TypeInfo InRHSScope)
                : base(InHostTranslator, InContext)
            {
                RHSScope = InRHSScope;
            }

            /// <summary>
            /// The RHS's Scope to set this STNode.
            /// </summary>
            public TypeInfo RHSScope { get; }
            // TODO Setter scheme should be decided with the RHS's Type.

            public override void Visit(STNodeVar InVarNode)
            {
                LoadVarAccessSI(InVarNode, EExprVarAccessType.Set, out var outSI, out var outPredType);
                ResultSchemeInstance = outSI;
                PredictScope = outPredType;
            }

            public override void Visit(STNodeMemberAccess InSubNode)
            {
                // Visit host node first.
                STNodeVisitor_GatherRefScheme hostVisitor = new STNodeVisitor_GatherRefScheme(HostTranslator, ParentContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InSubNode.LHS, hostVisitor);

                // Use host to find the best scheme for Setting the member.
                LoadMemberAccessSI(hostVisitor.PredictScope, InSubNode, EExprVarAccessType.Set, out var outVarType, out var outSI);
                outSI.AddPrerequisiteScheme("HOST", hostVisitor.ResultSchemeInstance);

                ResultSchemeInstance = outSI;
                PredictScope = outVarType;
            }

        }


        /// <summary>
        /// A Visitor to walkthrough a syntaxtree and gather ref-translate-schemes for all nodes.
        /// </summary>
        public class STNodeVisitor_GatherRefScheme
            : STNodeVisitor_GetNodeValue
        {
            public STNodeVisitor_GatherRefScheme(ExprTranslatorAbstract InHostTranslator, ITranslatingContext InContext)
                : base(InHostTranslator, InContext)
            {
            }

            public override void Visit(STNodeVar InVarNode)
            {
                LoadVarAccessSI(InVarNode, EExprVarAccessType.Ref, out var outSI, out var outPredType);
                ResultSchemeInstance = outSI;
                PredictScope = outPredType;
            }

            public override void Visit(STNodeMemberAccess InSubNode)
            {
                // Visit host node first.
                STNodeVisitor_GatherRefScheme hostVisitor = new STNodeVisitor_GatherRefScheme(HostTranslator, ParentContext);
                VisitByReflectionHelper.FindAndCallVisit<ISTNodeTranslateScheme>(InSubNode.LHS, hostVisitor);

                // Use host to find the best scheme for Referring the member.
                LoadMemberAccessSI(hostVisitor.PredictScope, InSubNode, EExprVarAccessType.Ref, out var outVarType, out var outSI);
                outSI.AddPrerequisiteScheme("HOST", hostVisitor.ResultSchemeInstance);

                ResultSchemeInstance = outSI;
                PredictScope = outVarType;
            }


        }



        /// <summary>
        /// Visitor to visit a statement node.
        /// </summary>
        public class STNodeVisitor_Statement
        {
            public STNodeVisitor_Statement(ExprTranslatorAbstract InHostTranslator, ITranslatingContext InContext)
            {
                HostTranslator = InHostTranslator;
                ParentContext = InContext;
            }

            public ExprTranslatorAbstract HostTranslator { get; }
            public ITranslatingContext ParentContext { get; }

            public List<ISTNodeTranslateSchemeInstance> TranslateSchemeInstances { get; } = new List<ISTNodeTranslateSchemeInstance>();

            public void Visit(ISyntaxTreeNode InOtherSTNode)
            {
                var stmtCtx = new StatementContext(ParentContext, InOtherSTNode);
                // Statement always starts with a getter visitor.
                STNodeVisitor_GetNodeValue valueNodeVisitor = new STNodeVisitor_GetNodeValue(HostTranslator, stmtCtx);
                VisitByReflectionHelper.FindAndCallVisit(InOtherSTNode, valueNodeVisitor);
                TranslateSchemeInstances.Add(valueNodeVisitor.ResultSchemeInstance);
            }

            public void Visit(STNodeSequence InNodes)
            {
                // TODO Block statement

                foreach (var subNode in InNodes.NodeList)
                {
                    var stmtCtx = new StatementContext(ParentContext, subNode);

                    // Statement always starts with a getter visitor.
                    STNodeVisitor_Statement statementNodeVisitor = new STNodeVisitor_Statement(HostTranslator, stmtCtx);
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
            public STNodeVisitor_FunctionBody(ExprTranslatorAbstract InHostTranslator, IExprTranslateEnvironment InEnvironment)
            {
                HostTranslator = InHostTranslator;
                ExprTranslateEnvironment = InEnvironment;
            }

            public ExprTranslatorAbstract HostTranslator { get; }
            public IExprTranslateEnvironment ExprTranslateEnvironment { get; }
            public List<ISTNodeTranslateSchemeInstance> TranslateSchemeInstances { get; private set; }

            /// <summary>
            /// Visit a single syntax-tree as function-body.
            /// </summary>
            /// <param name="InOtherSTNode"></param>
            public void Visit(ISyntaxTreeNode InOtherSTNode)
            {
                var rootCtx = new FuncBodyContext(ExprTranslateEnvironment);
                STNodeVisitor_Statement stmtVisitor = new STNodeVisitor_Statement(HostTranslator, rootCtx);
                VisitByReflectionHelper.FindAndCallVisit(InOtherSTNode, stmtVisitor);
                TranslateSchemeInstances = stmtVisitor.TranslateSchemeInstances;
            }

        }


    }

}
