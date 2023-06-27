using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Retrieve results of a sub node.
    /// </summary>
    public interface ISTNodeResultPlaceholder
    {
        /// <summary>
        /// Usage of the sub-node.
        /// </summary>
        EInstructionUsage Usage { get; }

        /// <summary>
        /// Present code that can be used as name or identity of sub node.
        /// </summary>
        string PresentCode { get; }

    }




    /// <summary>
    /// Usage of an instruction, to determine if the instruction is used for loading/setting/calling.
    /// </summary>
    public enum EInstructionUsage
    {
        /// <summary>
        /// This instruction is used for loading (RHS).
        /// A = b + c: {b + c}, {b} and {c} are 'Loading' instructions.
        /// </summary>
        Load,

        /// <summary>
        /// The instruction is used for saving (LHS).
        /// a.b = c: {b} is a 'Setting' instruction.
        /// </summary>
        Set,

        /// <summary>
        /// The instruction is used for calling.
        /// a.foo(b).c = 1: {a.foo} and {foo} are Calling instructions.
        /// </summary>
        Call,

    }

    /// <summary>
    /// Expression code generator.
    /// Create new generator for each expr-generating transaction.
    /// </summary>
    public abstract class ExprCodeGenerator
    {
        public ExprCodeGenerator()
        {
        }

        /// <summary>
        /// Scope (MethodInfo, TypeInfo) of the generating expression-codes.
        /// To search local and member variables from it.
        /// </summary>
        public Info Scope { get; protected set; }

        /// <summary>
        /// Results after Gen**** call.
        /// </summary>
        public abstract IReadOnlyList<string> Results { get; }

        /// <summary>
        /// Generate function codes.
        /// </summary>
        /// <param name="InFuncName"></param>
        /// <param name="InScopeInfo"></param>
        /// <param name="InExpressions"></param>
        /// <returns></returns>
        public virtual IReadOnlyList<string> GenFunctionCodes(string InFuncName, Info InScopeInfo, ISyntaxTreeNode InBodySyntax)
        {
            Scope = InScopeInfo;
            FunctionBodyVisitor visitor = new FunctionBodyVisitor(this);
            VisitByReflectionHelper.FindAndCallVisit(InBodySyntax, visitor);

            return Results;
        }

        //public string[] GenInitExprCodes(ISyntaxTreeNode InExpression)
        //{
        //    var codes = CodePrototype.Clone();
        //    codes.BeginInit();
        //    codes.EndInit();
        //
        //    return codes.Results;
        //}

        /// <summary>
        /// Emit codes for constant string and write results to the InTargetPlaceholder.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InValueType"></param>
        /// <param name="InTextString"></param>
        /// <returns></returns>
        protected abstract void EmitConstString(ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InTextString);

        /// <summary>
        /// Emit codes for constant value-string (like integer, floating, structure ...).
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InConstValueString"></param>
        /// <returns></returns>
        protected abstract void EmitConstValueCode(ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InConstValueString);

        /// <summary>
        /// Emit codes for constant Info-object
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InConstInfo"></param>
        /// <returns></returns>
        protected abstract void EmitConstInfo(ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, Info InConstInfo);

        /// <summary>
        /// Emit a global/local variable's reference for loading or overwriting its content in future.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InScope"></param>
        /// <param name="InVarID"></param>
        /// <param name="InUsage">Determine which way to use the variable in the future.</param>
        /// <returns></returns>
        protected abstract void EmitVarRef(ISTNodeResultPlaceholder InTargetPlaceholder, Info InScope, string InVarID, EInstructionUsage InUsage);

        /// <summary>
        /// Emit the member's reference for loading or overwriting its content in future.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InHost"></param>
        /// <param name="InMemberId"></param>
        /// <param name="InUsage"></param>
        /// <returns></returns>
        protected abstract void EmitMemberRef(ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InHost, string InMemberId, EInstructionUsage InUsage);

        /// <summary>
        /// Emit codes for assignment.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InLhsCode"></param>
        /// <param name="InRhsCode"></param>
        /// <returns></returns>
        protected abstract void EmitAssign(ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InLhsCode, ISTNodeResultPlaceholder InRhsCode);

        /// <summary>
        /// Emit codes for binary-operations.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InOpCode"></param>
        /// <param name="InLeftCode"></param>
        /// <param name="InRightCode"></param>
        /// <returns></returns>
        protected abstract void EmitBinOp(ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InLeftCode, ISTNodeResultPlaceholder InRightCode);

        /// <summary>
        /// Emit codes for unary-operations.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InOpCode"></param>
        /// <param name="InRhsCode"></param>
        /// <returns></returns>
        protected abstract void EmitUnaryOp(ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InRhsCode);

        /// <summary>
        /// Emit codes for function call.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InSourceCode"></param>
        /// <param name="InParamCodes"></param>
        /// <returns></returns>
        protected abstract void EmitCall(ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InSourceCode, ISTNodeResultPlaceholder[] InParamCodes);

        /// <summary>
        /// Emit codes for creating an object.
        /// </summary>
        /// <param name="InTargetPlaceholder">Should be null, see <see cref="ISTNodeResultPlaceholder"/> for more informations. </param>
        /// <param name="InArchetype"></param>
        /// <param name="InParamCodes"></param>
        /// <returns></returns>
        protected abstract void EmitNew(ISTNodeResultPlaceholder InTargetPlaceholder, Info InArchetype, ISTNodeResultPlaceholder[] InParamCodes);

        /// <summary>
        /// Called before generating codes for a function.
        /// </summary>
        protected virtual void BeginFunction(ISyntaxTreeNode InFunctionRoot)
        {
        }

        /// <summary>
        /// Called after generated codes for a function.
        /// </summary>
        protected virtual void EndFunction(ISyntaxTreeNode InFunctionRoot)
        {
        }

        /// <summary>
        /// Called before generating statement codes.
        /// </summary>
        protected virtual void BeginStatement(ISyntaxTreeNode InStatementNode)
        {
        }

        /// <summary>
        /// Called after generated codes for a statement.
        /// </summary>
        protected virtual void EndStatement(ISyntaxTreeNode InStatementNode)
        {
        }

        /// <summary>
        /// Called before generating codes for a sub-block.
        /// </summary>
        protected virtual void BeginSubBlock(STNodeSequence InBlockNodes)
        {
        }

        /// <summary>
        /// Called after generated codes for a sub-block.
        /// </summary>
        protected virtual void EndSubBlock(STNodeSequence InBlockNodes)
        {
        }

        /// <summary>
        /// Called before generating codes for a syntax node in a statement.
        /// </summary>
        /// <param name="InNode"></param>
        protected virtual void BeginNode(ISyntaxTreeNode InNode)
        {
        }

        /// <summary>
        /// Called after generated codes for a syntax node in a statement.
        /// </summary>
        /// <param name="InNode"></param>
        protected virtual void EndNode(ISyntaxTreeNode InNode)
        {
        }

        /// <summary>
        /// Alloc placeholder for sub-nodes to retrieve their results in future.
        /// </summary>
        /// <param name="InUsage"></param>
        protected abstract ISTNodeResultPlaceholder AllocPlaceholderForSubNode(EInstructionUsage InUsage);

        /// <summary>
        /// Called before accessing placeholder of a sub-node.
        /// </summary>
        /// <param name="InPlaceholder"></param>
        protected virtual void PreAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
        {
        }

        /// <summary>
        /// Called after a sub-node's placeholder has been used.
        /// </summary>
        /// <param name="InPlaceholder"></param>
        protected virtual void PostAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
        {
        }


        class ValueNodeVisitor
        {
            public ValueNodeVisitor(ExprCodeGenerator InHostGenerator, ISTNodeResultPlaceholder InTargetPlaceholder)
            {
                HostGenerator = InHostGenerator;
                TargetPlaceholder = InTargetPlaceholder;
            }

            public ExprCodeGenerator HostGenerator { get; }

            /// <summary>
            /// Placeholder which retrieves result from the current visiting node.
            /// </summary>
            public ISTNodeResultPlaceholder TargetPlaceholder { get; }

            public virtual void Visit(STNodeConstant InConst)
            {
                if (InConst.Value == null)
                {
                    HostGenerator.EmitConstValueCode(TargetPlaceholder, InConst.Type, "null");
                }
                else if (InConst.Value.GetType().IsValueType)
                {
                    Type valueType = InConst.Value.GetType();

                    HostGenerator.EmitConstValueCode(TargetPlaceholder, InConst.Type, InConst.Value.ToString());
                }
                else if (InConst.Value is string)
                {
                    HostGenerator.EmitConstString(TargetPlaceholder, InConst.Type, InConst.Value as string);
                }
                else if (InConst.Value is Info)
                {
                    HostGenerator.EmitConstInfo(TargetPlaceholder, InConst.Type, InConst.Value as Info);
                }
            }

            public virtual void Visit(STNodeVar InVarNode)
            {
                HostGenerator.EmitVarRef(TargetPlaceholder, HostGenerator.Scope, InVarNode.IDName, EInstructionUsage.Load);
            }

            public virtual void Visit(STNodeSub InSubNode)
            {
                throw new NotImplementedException();
            }
            public virtual void Visit(STNodeAssign InAssignNode)
            {
                HostGenerator.BeginNode(InAssignNode);

                var lhsPlaceholder = HostGenerator.AllocPlaceholderForSubNode(EInstructionUsage.Set);
                var rhsPlaceholder = HostGenerator.AllocPlaceholderForSubNode(EInstructionUsage.Load);

                ValueNodeVisitor rhsVisitor = new ValueNodeVisitor(HostGenerator, rhsPlaceholder);
                VisitByReflectionHelper.FindAndCallVisit(InAssignNode.RHS, rhsVisitor);

                LHSNodeVisitor lhsVisitor = new LHSNodeVisitor(HostGenerator, lhsPlaceholder);
                VisitByReflectionHelper.FindAndCallVisit(InAssignNode.LHS, lhsVisitor);

                HostGenerator.PreAccessPlaceholder(rhsPlaceholder);
                HostGenerator.PreAccessPlaceholder(lhsPlaceholder);
                HostGenerator.EmitAssign(TargetPlaceholder, lhsPlaceholder, rhsPlaceholder);
                HostGenerator.PostAccessPlaceholder(lhsPlaceholder);
                HostGenerator.PostAccessPlaceholder(rhsPlaceholder);

                HostGenerator.EndNode(InAssignNode);
            }
            public virtual void Visit(STNodeBinaryOp InBinOpNode)
            {
                var lPlaceholder = HostGenerator.AllocPlaceholderForSubNode(EInstructionUsage.Load);
                var rPlaceholder = HostGenerator.AllocPlaceholderForSubNode(EInstructionUsage.Load);

                ValueNodeVisitor subVisitor_L = new ValueNodeVisitor(HostGenerator, lPlaceholder);
                VisitByReflectionHelper.FindAndCallVisit(InBinOpNode.LHS, subVisitor_L);

                ValueNodeVisitor subVisitor_R = new ValueNodeVisitor(HostGenerator, rPlaceholder);
                VisitByReflectionHelper.FindAndCallVisit(InBinOpNode.RHS, subVisitor_R);

                HostGenerator.PreAccessPlaceholder(rPlaceholder);
                HostGenerator.PreAccessPlaceholder(lPlaceholder);
                HostGenerator.EmitBinOp(TargetPlaceholder, InBinOpNode.OpCode, lPlaceholder, rPlaceholder);
                HostGenerator.PostAccessPlaceholder(lPlaceholder);
                HostGenerator.PostAccessPlaceholder(rPlaceholder);
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

        class LHSNodeVisitor
            : ValueNodeVisitor
        {
            public LHSNodeVisitor(ExprCodeGenerator InHostGenerator, ISTNodeResultPlaceholder InTargetPlaceholder)
                : base(InHostGenerator, InTargetPlaceholder)
            {
            }

            public override void Visit(STNodeVar InVarNode)
            {
                HostGenerator.EmitVarRef(TargetPlaceholder, HostGenerator.Scope, InVarNode.IDName, EInstructionUsage.Set);
            }

            public override void Visit(STNodeSub InSubNode)
            {
                throw new NotImplementedException();
            }


        }

        /// <summary>
        /// Visitor to visit a statement code.
        /// </summary>
        class StatementNodeVisitor
        {
            public StatementNodeVisitor(ExprCodeGenerator InHostGenerator)
            {
                HostGenerator = InHostGenerator;
            }

            public ExprCodeGenerator HostGenerator { get; }

            public void Visit(ISyntaxTreeNode InOtherSTNode)
            {
                HostGenerator.BeginStatement(InOtherSTNode);

                ValueNodeVisitor valueNodeVisitor = new ValueNodeVisitor(HostGenerator, null);
                VisitByReflectionHelper.FindAndCallVisit(InOtherSTNode, valueNodeVisitor);

                HostGenerator.EndStatement(InOtherSTNode);
            }

            public void Visit(STNodeSequence InNodes)
            {
                HostGenerator.BeginSubBlock(InNodes);

                //List<ISTNodeCodeSnippet> snippets = new List<ISTNodeCodeSnippet>();
                foreach (var subNode in InNodes.NodeList)
                {
                    StatementNodeVisitor statementNodeVisitor = new StatementNodeVisitor(HostGenerator);
                    VisitByReflectionHelper.FindAndCallVisit(subNode, statementNodeVisitor);
                    //snippets.Add(statementNodeVisitor.EmittedCode);
                }

                HostGenerator.EndSubBlock(InNodes);
            }
        }

        /// <summary>
        /// Visitor to visit a function body.
        /// </summary>
        class FunctionBodyVisitor
        {
            public FunctionBodyVisitor(ExprCodeGenerator InHostGenerator)
            {
                HostGenerator = InHostGenerator;
            }

            public ExprCodeGenerator HostGenerator { get; }

            /// <summary>
            /// Visit a single syntax-tree as function-body.
            /// </summary>
            /// <param name="InOtherSTNode"></param>
            public void Visit(ISyntaxTreeNode InOtherSTNode)
            {
                HostGenerator.BeginFunction(InOtherSTNode);

                StatementNodeVisitor stmtVisitor = new StatementNodeVisitor(HostGenerator);
                VisitByReflectionHelper.FindAndCallVisit(InOtherSTNode, stmtVisitor);

                //HostGenerator.EndFunction(new ISTNodeCodeSnippet[] { stmtVisitor.EmittedCode });
                HostGenerator.EndFunction(InOtherSTNode);
            }

        }


    }



    ///// <summary>
    ///// Analyze STNode and dispatch it to the CodeGenerator.
    ///// </summary>
    //class ExprCodeTranslator
    //{
    //    public ExprCodeTranslator(Info InScope, ISyntaxTreeNode InSTNode)
    //    {
    //        Scope = InScope;
    //        STNode = InSTNode;
    //    }

    //    /// <summary>
    //    /// Scope of the syntax-tree.
    //    /// </summary>
    //    public Info Scope { get; }

    //    /// <summary>
    //    /// STNode handled by this node.
    //    /// </summary>
    //    public ISyntaxTreeNode STNode { get; }

    //    /// <summary>
    //    /// The scope predicted from STNode.
    //    /// </summary>
    //    public Info STNodeScope { get; }

    //    /// <summary>
    //    /// Usage of the current STNode.
    //    /// </summary>
    //    ESyntaxTreeNodeUsage Usage { get; }

    //    /// <summary>
    //    /// Present code of the STNode.
    //    /// </summary>
    //    public string PresentCode { get; }

    //    public List<ExprCodeTranslator> SubInstructions { get; }

    //    /// <summary>
    //    /// Generate codes for the RootST.
    //    /// </summary>
    //    public static ExprCodeTranslator Process(IExprCodeEmitter InEmitter, Info InScope, ISyntaxTreeNode InSTNode, ESyntaxTreeNodeUsage InUsage = ESyntaxTreeNodeUsage.Statement)
    //    {
    //        // _PreProcessSubNodes(InNode);

    //        ExprCodeTranslator rootNode = new ExprCodeTranslator(InScope, InSTNode);

    //        List<ExprCodeTranslator> subInstructions = new List<ExprCodeTranslator>();
    //        InSTNode.ForeachSubEntriesAndUsagesByOrder(InUsage, (subUsage, subSTNode) =>
    //        {
    //            var subInst = Process(InEmitter, InScope, subSTNode, subUsage);
    //            subInstructions.Add(subInst);
    //        });

    //        ExprCodeTranslator instruction = new ExprCodeTranslator(InScope, InSTNode, InUsage, subInstructions);

    //        return instruction;
    //    }



    //    private void _GenerateCode()
    //    {
    //        if (STNode is STNodeCall)
    //        {
    //        }
    //        // Var: determine its usage: ref(lhs)/get(rhs)/set-back (in out param)
    //        //      Var codes will be generated when analyzing other nodes.
    //        else if (STNode is STNodeVar)
    //        {
    //            var stnVar = STNode as STNodeVar;

    //            bool isSetbackVar = false;
    //            if (Usage == ESyntaxTreeNodeUsage.Set
    //                || Usage == ESyntaxTreeNodeUsage.Ref)
    //            {
    //                if (!_IsSupportRef())
    //                {
    //                    if (!_IsSupportSet())
    //                    {
    //                        isSetbackVar = true;
    //                    }
    //                }
    //            }

    //            if (isSetbackVar)
    //            {
    //                // TODO Create TEMP var
    //                // TODO Register POST var
    //            }

    //            //IILVariable var = _FindVariable(Scope, , stnVar.VariableName);

    //            //ElementInfo propInfo = InfoHelper.FindPropertyAlongScopeTree(Scope, stnVar.VariableName);
    //            //if (propInfo != null)
    //            //{
    //            //    SimpleHtml5PageTranslator.EnsureMemberAccessCodes(propInfo);
    //            //    var getCode = propInfo.Extra.MemberGetExprCode;
    //            //    var refCode = propInfo.Extra.MemberRefExprCode;
    //            //    var setCode = propInfo.Extra.MemberSetExprCode;

    //            //    var filtedGetCode = getCode.Replace("$OWNER", $"{Scope.Name}.");
    //            //    var filtedRefCode = refCode == null ? refCode : refCode.Replace("$OWNER", $"{Scope.Name}.");
    //            //}
    //        }
    //        else if (STNode is STNodeConstant)
    //        {
    //            var stnConst = STNode as STNodeConstant;
    //        }

    //        // handle stage-end scripts
    //        foreach (var subInst in SubInstructions)
    //        {
    //            if (subInst.Usage == ESyntaxTreeNodeUsage.Set
    //                || subInst.Usage == ESyntaxTreeNodeUsage.Ref
    //                )
    //            {
    //                subInst.NotifyPostModify();
    //            }
    //        }

    //    }

    //}

}
