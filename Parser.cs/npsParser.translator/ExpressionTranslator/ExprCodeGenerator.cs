using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Code snippet generated for a syntax-tree node.
    /// </summary>
    public interface ISTNodeCodeSnippet
    {

        /// <summary>
        /// Present code that can be used as name or identity of this snippet.
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

            BeginFunction();

            ValueNodeVisitor visitor = new ValueNodeVisitor(this);
            VisitByReflectionHelper.FindAndCallVisit(InBodySyntax, visitor);

            EndFunction();

            return Results;
        }

        protected virtual void BeginFunction()
        {
        }

        protected virtual void EndFunction()
        {
        }

        protected virtual void BeginSubBlock(ISTNodeCodeSnippet InSnippet)
        {
        }

        protected virtual void EndSubBlock(ISTNodeCodeSnippet InSnippet)
        {
        }

        protected virtual void BeginSet(ISTNodeCodeSnippet InSnippet)
        {
        }

        protected virtual void EndSet(ISTNodeCodeSnippet InSnippet)
        {
        }

        protected virtual void BeginLoad(ISTNodeCodeSnippet InSnippet)
        {
        }

        protected virtual void EndLoad(ISTNodeCodeSnippet InSnippet)
        {
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
        /// Emit codes for constant string.
        /// </summary>
        /// <param name="InTextString"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitConstString(TypeInfo InValueType, string InTextString);

        /// <summary>
        /// Emit codes for constant value-string (like integer, floating, structure ...).
        /// </summary>
        /// <param name="InConstValueString"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitConstValueCode(TypeInfo InValueType, string InConstValueString);

        /// <summary>
        /// Emit codes for constant Info-object
        /// </summary>
        /// <param name="InConstInfo"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitConstInfo(TypeInfo InValueType, Info InConstInfo);

        /// <summary>
        /// Emit a global/local variable's reference for loading or overwriting its content in future.
        /// </summary>
        /// <param name="InScope"></param>
        /// <param name="InVarID"></param>
        /// <param name="InLoadValue">Determine if </param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitVarRef(Info InScope, string InVarID, EInstructionUsage InUsage);

        /// <summary>
        /// Emit the member's reference for loading or overwriting its content in future.
        /// </summary>
        /// <param name="InHost"></param>
        /// <param name="InMemberId"></param>
        /// <param name="InUsage"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitMemberRef(ISTNodeCodeSnippet InHost, string InMemberId, EInstructionUsage InUsage);

        /// <summary>
        /// Emit codes for assignment.
        /// </summary>
        /// <param name="InLhsCode"></param>
        /// <param name="InRhsCode"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitAssign(ISTNodeCodeSnippet InLhsCode, ISTNodeCodeSnippet InRhsCode);

        /// <summary>
        /// Emit codes for binary-operations.
        /// </summary>
        /// <param name="InOpCode"></param>
        /// <param name="InLeftCode"></param>
        /// <param name="InRightCode"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitBinOp(string InOpCode, ISTNodeCodeSnippet InLeftCode, ISTNodeCodeSnippet InRightCode);

        /// <summary>
        /// Emit codes for unary-operations.
        /// </summary>
        /// <param name="InOpCode"></param>
        /// <param name="InRhsCode"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitUnaryOp(string InOpCode, ISTNodeCodeSnippet InRhsCode);

        /// <summary>
        /// Emit codes for function call.
        /// </summary>
        /// <param name="InSourceCode"></param>
        /// <param name="InParamCodes"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitCall(ISTNodeCodeSnippet InSourceCode, ISTNodeCodeSnippet[] InParamCodes);

        /// <summary>
        /// Emit codes for creating an object.
        /// </summary>
        /// <param name="InArchetype"></param>
        /// <param name="InParamCodes"></param>
        /// <returns></returns>
        protected abstract ISTNodeCodeSnippet EmitNew(Info InArchetype, ISTNodeCodeSnippet[] InParamCodes);

        class ValueNodeVisitor
        {
            public ValueNodeVisitor(ExprCodeGenerator InHostGenerator)
            {
                HostGenerator = InHostGenerator;
            }

            public ExprCodeGenerator HostGenerator { get; }

            /// <summary>
            /// Emitted code for the syntax-tree node.
            /// </summary>
            public ISTNodeCodeSnippet EmittedCode { get; protected set; }

            public virtual void Visit(STNodeConstant InConst)
            {
                if (InConst.Value == null)
                {
                    EmittedCode = HostGenerator.EmitConstValueCode(InConst.Type, "null");
                }
                else if (InConst.Value.GetType().IsValueType)
                {
                    Type valueType = InConst.Value.GetType();

                    EmittedCode = HostGenerator.EmitConstValueCode(InConst.Type, InConst.Value.ToString());
                }
                else if (InConst.Value is string)
                {
                    EmittedCode = HostGenerator.EmitConstString(InConst.Type, InConst.Value as string);
                }
                else if (InConst.Value is Info)
                {
                    EmittedCode = HostGenerator.EmitConstInfo(InConst.Type, InConst.Value as Info);
                }
            }

            public virtual void Visit(STNodeVar InVarNode)
            {
                EmittedCode = HostGenerator.EmitVarRef(HostGenerator.Scope, InVarNode.IDName, EInstructionUsage.Load);
            }

            public virtual void Visit(STNodeSub InSubNode)
            {
                throw new NotImplementedException();
            }
            public virtual void Visit(STNodeAssign InAssignNode)
            {
                ValueNodeVisitor rhsVisitor = new ValueNodeVisitor(HostGenerator);
                VisitByReflectionHelper.FindAndCallVisit(InAssignNode.RHS, rhsVisitor);
                var rhsSnippet = rhsVisitor.EmittedCode;

                LHSNodeVisitor lhsVisitor = new LHSNodeVisitor(HostGenerator);
                VisitByReflectionHelper.FindAndCallVisit(InAssignNode.LHS, lhsVisitor);
                var lhsSnippet = lhsVisitor.EmittedCode;

                HostGenerator.BeginLoad(rhsSnippet);
                HostGenerator.BeginSet(lhsSnippet);
                EmittedCode = HostGenerator.EmitAssign(lhsSnippet, rhsSnippet);
                HostGenerator.EndSet(lhsSnippet);
                HostGenerator.EndLoad(rhsSnippet);
            }
            public virtual void Visit(STNodeBinaryOp InBinOpNode)
            {
                ValueNodeVisitor subVisitor_L = new ValueNodeVisitor(HostGenerator);
                VisitByReflectionHelper.FindAndCallVisit(InBinOpNode.LHS, subVisitor_L);
                var lSnippet = subVisitor_L.EmittedCode;

                ValueNodeVisitor subVisitor_R = new ValueNodeVisitor(HostGenerator);
                VisitByReflectionHelper.FindAndCallVisit(InBinOpNode.RHS, subVisitor_R);
                var rSnippet = subVisitor_R.EmittedCode;

                HostGenerator.BeginLoad(lSnippet);
                HostGenerator.BeginLoad(rSnippet);
                EmittedCode = HostGenerator.EmitBinOp(InBinOpNode.OpCode, lSnippet, rSnippet);
                HostGenerator.EndLoad(rSnippet);
                HostGenerator.EndLoad(lSnippet);
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
            public LHSNodeVisitor(ExprCodeGenerator InHostGenerator)
                : base(InHostGenerator)
            {
            }

            public override void Visit(STNodeVar InVarNode)
            {
                EmittedCode = HostGenerator.EmitVarRef(HostGenerator.Scope, InVarNode.IDName, EInstructionUsage.Set);
            }

            public override void Visit(STNodeSub InSubNode)
            {
                throw new NotImplementedException();
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
