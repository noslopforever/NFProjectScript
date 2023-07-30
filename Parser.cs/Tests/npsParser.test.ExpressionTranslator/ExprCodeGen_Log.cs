using nf.protoscript;
using nf.protoscript.syntaxtree;
using nf.protoscript.translator.expression;
using System;
using System.Collections.Generic;
using System.IO;

namespace npsParser.test.ExpressionTranslator
{


    internal class ExprCodeGen_Log
        : ExprCodeGeneratorAbstract
    {
        class NodeResultPlaceholder
            : ISTNodeResultPlaceholder
        {
            public NodeResultPlaceholder(EInstructionUsage InUsage)
            {
                Usage = InUsage;
            }

            public EInstructionUsage Usage { get; }

            public string PresentCode { get; internal set; }

            public override string ToString()
            {
                return $"{Usage}:{PresentCode}";
            }
        }

        public override IReadOnlyList<string> Results
        {
            get
            {
                return _Results;
            }
        }
        List<string> _Results = new List<string>();


        protected override Stage EnterStage(EStageType InStageType, ISyntaxTreeNode InSTNode)
        {
            var stage = base.EnterStage(InStageType, InSTNode);
            _Results.Add($"Enter Stage {InStageType}");
            return stage;
        }
        protected override void LeaveStage(EStageType InStageType, ISyntaxTreeNode InSTNode)
        {
            _Results.Add($"Leave Stage {InStageType}");
            base.LeaveStage(InStageType, InSTNode);
        }
        protected override void EnterNode(ISyntaxTreeNode InNode)
        {
            _Results.Add("EnterNode");
        }
        protected override void LeaveNode(ISyntaxTreeNode InNode)
        {
            _Results.Add("LeaveNode");
        }
        protected override ISTNodeResultPlaceholder AllocPlaceholderForSubNode(EInstructionUsage InUsage)
        {
            return new NodeResultPlaceholder(InUsage);
        }
        protected override void PreAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
        {
            _Results.Add($"BeginAccess ({InPlaceholder})");
        }
        protected override void PostAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
        {
            _Results.Add($"EndAccess ({InPlaceholder})");
        }


        protected override void EmitConstString(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InTextString)
        {
            string code = $"\"{InTextString}\"";
            _Results.Add(code);

            if (InTargetPlaceholder != null)
            {
                (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = code;
            }
        }

        protected override void EmitConstValueCode(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InConstValueString)
        {
            _Results.Add(InConstValueString);
            if (InTargetPlaceholder != null)
            {
                (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = InConstValueString;
            }
        }

        protected override void EmitConstInfo(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, Info InConstInfo)
        {
            throw new NotImplementedException();
        }

        protected override void EmitVarRef(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, Info InScope, string InVarID, EInstructionUsage InUsage)
        {
            switch (InUsage)
            {
                case EInstructionUsage.Load:
                case EInstructionUsage.Call:
                    string code = $"LD {InScope.Name}::{InVarID}";
                    _Results.Add(code);
                    if (InTargetPlaceholder != null)
                    {
                        (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = code;
                    }
                    break;
                case EInstructionUsage.Set:
                    string setCode = $"REFSET {InScope.Name}::{InVarID}";
                    _Results.Add(setCode);
                    if (InTargetPlaceholder != null)
                    {
                        (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = setCode;
                    }
                    break;
            }
        }

        protected override void EmitMemberRef(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InHost, string InMemberID, EInstructionUsage InUsage)
        {
            switch (InUsage)
            {
                case EInstructionUsage.Load:
                case EInstructionUsage.Call:
                    string code = $"MBRLD {InHost.PresentCode}.{InMemberID}";
                    _Results.Add(code);
                    if (InTargetPlaceholder != null)
                    {
                        (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = code;
                    }
                    break;
                case EInstructionUsage.Set:
                    string setCode = $"REFMBRSET {InHost.PresentCode}.{InMemberID}";
                    _Results.Add(setCode);
                    if (InTargetPlaceholder != null)
                    {
                        (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = setCode;
                    }
                    break;
            }
        }

        protected override void EmitAssign(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InLhsCode, ISTNodeResultPlaceholder InRhsCode)
        {
            string lhsCode = InLhsCode != null ? InLhsCode.PresentCode : "ERROR_LHS";
            string rhsCode = InRhsCode != null ? InRhsCode.PresentCode : "ERROR_RHS";
            string code = $"ASSIGN ({lhsCode}) ({rhsCode})";
            _Results.Add(code);

            if (InTargetPlaceholder != null)
            {
                (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = code;
            }
        }

        protected override void EmitBinOp(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InLeftCode, ISTNodeResultPlaceholder InRightCode)
        {
            string lhsCode = InLeftCode != null ? InLeftCode.PresentCode : "ERROR_LEFT";
            string rhsCode = InRightCode != null ? InRightCode.PresentCode : "ERROR_RIGHT";
            string code = $"BINOP{InOpCode} ({lhsCode}) ({rhsCode})";
            _Results.Add(code);

            if (InTargetPlaceholder != null)
            {
                (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = code;
            }
        }

        protected override void EmitUnaryOp(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InRhsCode)
        {
            throw new NotImplementedException();
        }

        protected override void EmitCall(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InSourceCode, ISTNodeResultPlaceholder[] InParamCodes)
        {
            throw new NotImplementedException();
        }

        protected override void EmitNew(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, Info InArchetype, ISTNodeResultPlaceholder[] InParamCodes)
        {
            throw new NotImplementedException();
        }
    }
}