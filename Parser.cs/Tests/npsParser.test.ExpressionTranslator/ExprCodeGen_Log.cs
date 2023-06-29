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


        protected override void BeginFunction(ISyntaxTreeNode InFunctionRoot)
        {
            _Results.Add("BeginFunction");
        }
        protected override void EndFunction(ISyntaxTreeNode InFunctionRoot)
        {
            _Results.Add("EndFunction");
        }
        protected override void BeginStatement(ISyntaxTreeNode InStatementNode)
        {
            _Results.Add("BeginStatement");
        }
        protected override void EndStatement(ISyntaxTreeNode InStatementNode)
        {
            _Results.Add("EndStatement");
        }
        protected override void BeginSubBlock(STNodeSequence InBlockNodes)
        {
            _Results.Add("BeginSubBlock");
        }
        protected override void EndSubBlock(STNodeSequence InBlockNodes)
        {
            _Results.Add("EndSubBlock");
        }
        protected override void BeginNode(ISyntaxTreeNode InNode)
        {
            _Results.Add("BeginNode");
        }
        protected override void EndNode(ISyntaxTreeNode InNode)
        {
            _Results.Add("EndNode");
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


        protected override void EmitConstString(ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InTextString)
        {
            string code = $"\"{InTextString}\"";
            _Results.Add(code);

            if (InTargetPlaceholder != null)
            {
                (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = code;
            }
        }

        protected override void EmitConstValueCode(ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InConstValueString)
        {
            _Results.Add(InConstValueString);
            if (InTargetPlaceholder != null)
            {
                (InTargetPlaceholder as NodeResultPlaceholder).PresentCode = InConstValueString;
            }
        }

        protected override void EmitConstInfo(ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, Info InConstInfo)
        {
            throw new NotImplementedException();
        }

        protected override void EmitVarRef(ISTNodeResultPlaceholder InTargetPlaceholder, Info InScope, string InVarID, EInstructionUsage InUsage)
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

        protected override void EmitMemberRef(ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InHost, string InMemberID, EInstructionUsage InUsage)
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

        protected override void EmitAssign(ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InLhsCode, ISTNodeResultPlaceholder InRhsCode)
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

        protected override void EmitBinOp(ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InLeftCode, ISTNodeResultPlaceholder InRightCode)
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

        protected override void EmitUnaryOp(ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InRhsCode)
        {
            throw new NotImplementedException();
        }

        protected override void EmitCall(ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InSourceCode, ISTNodeResultPlaceholder[] InParamCodes)
        {
            throw new NotImplementedException();
        }

        protected override void EmitNew(ISTNodeResultPlaceholder InTargetPlaceholder, Info InArchetype, ISTNodeResultPlaceholder[] InParamCodes)
        {
            throw new NotImplementedException();
        }
    }
}