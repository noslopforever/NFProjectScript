using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Visit STNodes and emit code for them.
    /// </summary>
    public class ExprCodeEmitVisitor
    {
        public ExprCodeEmitVisitor(Info InHostScope, IExprCodeEmitter InEmitter, bool InLeftHand = false)
        {
            HostScope = InHostScope;
            Emitter = InEmitter;
            LeftHand = InLeftHand;
        }


        Info HostScope { get; }

        IExprCodeEmitter Emitter { get; }

        bool LeftHand { get; }

        public IInstructionCode EmittedCode { get; private set; }

        public void Visit(STNodeConstant InConst)
        {
            string constString = "$ERR_UnknownConst";

            if (InConst.Value == null)
            {
                EmittedCode = Emitter.EmitConstValueCode("null");
            }
            else if (InConst.Value.GetType().IsValueType)
            {
                EmittedCode = Emitter.EmitConstValueCode(InConst.Value.ToString());
            }
            else if (InConst.Value is string)
            {
                EmittedCode = Emitter.EmitConstString(InConst.Value as string);
            }
            else if (InConst.Value is Info)
            {
                throw new NotImplementedException();
            }

        }

        public void Visit(STNodeVar InVarNode)
        {
            if (LeftHand)
            {
                EmittedCode = Emitter.EmitRefVarForSet(HostScope, InVarNode.IDName);
            }
            else
            {
                EmittedCode = Emitter.EmitVarLoad(HostScope, InVarNode.IDName);
            }
        }

        public void Visit(STNodeSub InSubNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(STNodeAssign InAssignNode)
        {
            ExprCodeEmitVisitor rhsVisitor = new ExprCodeEmitVisitor(HostScope, Emitter, false);
            VisitByReflectionHelper.FindAndCallVisit(InAssignNode.RHS, rhsVisitor);
            IInstructionCode rhsGenCode = rhsVisitor.EmittedCode;

            ExprCodeEmitVisitor lhsVisitor = new ExprCodeEmitVisitor(HostScope, Emitter, true);
            VisitByReflectionHelper.FindAndCallVisit(InAssignNode.LHS, lhsVisitor);
            IInstructionCode lhsGenCode = lhsVisitor.EmittedCode;

            EmittedCode = Emitter.EmitAssign(lhsGenCode, rhsGenCode);

            //Emitter.Finish(lhsGenCode);
            //Emitter.Finish(rhsGenCode);
        }
        public void Visit(STNodeBinaryOp InBinOpNode)
        {
            throw new NotImplementedException();
        }
        public void Visit(STNodeCall InCallNode)
        {
            throw new NotImplementedException();
        }
        public void Visit(STNodeNew InNewNode)
        {
            throw new NotImplementedException();
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
