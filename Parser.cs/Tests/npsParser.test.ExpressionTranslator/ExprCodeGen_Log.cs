using nf.protoscript;
using nf.protoscript.translator.expression;
using System;
using System.Collections.Generic;
using System.IO;

namespace npsParser.test.ExpressionTranslator
{


    internal class ExprCodeGen_Log
        : ExprCodeGenerator
    {
        class GenCode
            : ISTNodeCodeSnippet
        {
            public GenCode(string InCode)
            {
                PresentCode = InCode;
            }
            public string PresentCode { get; }
        }

        public override IReadOnlyList<string> Results
        {
            get
            {
                return _Results;
            }
        }
        List<string> _Results = new List<string>();


        protected override void BeginFunction()
        {
            _Results.Add("BeginFunction");
        }
        protected override void EndFunction(IReadOnlyList<ISTNodeCodeSnippet> InSubSnippets)
        {
            _Results.Add("EndFunction");
        }
        protected override void BeginStatement()
        {
            _Results.Add("BeginStatement");
        }
        protected override void EndStatement(ISTNodeCodeSnippet InStatementSnippet)
        {
            _Results.Add("EndStatement");
        }
        protected override void BeginSubBlock()
        {
            _Results.Add("BeginSubBlock");
        }
        protected override void EndSubBlock(IReadOnlyList<ISTNodeCodeSnippet> InSubSnippets)
        {
            _Results.Add("EndSubBlock");
        }
        protected override void BeginLoadSnippetAccess(ISTNodeCodeSnippet InSnippet)
        {
            _Results.Add($"BeginLoad ({InSnippet.PresentCode})");
        }
        protected override void EndLoadSnippetAccess(ISTNodeCodeSnippet InSnippet)
        {
            _Results.Add($"EndLoad ({InSnippet.PresentCode})");
        }
        protected override void BeginSetSnippetAccess(ISTNodeCodeSnippet InSnippet)
        {
            _Results.Add($"BeginSet ({InSnippet.PresentCode})");
        }
        protected override void EndSetSnippetAccess(ISTNodeCodeSnippet InSnippet)
        {
            _Results.Add($"EndSet ({InSnippet.PresentCode})");
        }

        protected override ISTNodeCodeSnippet EmitConstString(TypeInfo InValueType, string InTextString)
        {
            string code = $"\"{InTextString}\"";
            _Results.Add(code);
            return new GenCode(code);
        }

        protected override ISTNodeCodeSnippet EmitConstValueCode(TypeInfo InValueType, string InConstValueString)
        {
            _Results.Add(InConstValueString);
            return new GenCode(InConstValueString);
        }

        protected override ISTNodeCodeSnippet EmitConstInfo(TypeInfo InValueType, Info InConstInfo)
        {
            throw new NotImplementedException();
        }

        protected override ISTNodeCodeSnippet EmitVarRef(Info InScope, string InVarID, EInstructionUsage InUsage)
        {
            switch (InUsage)
            {
                case EInstructionUsage.Load:
                case EInstructionUsage.Call:
                    string code = $"LD {InScope.Name}::{InVarID}";
                    _Results.Add(code);
                    return new GenCode(code);
                case EInstructionUsage.Set:
                    string setCode = $"REFSET {InScope.Name}::{InVarID}";
                    _Results.Add(setCode);
                    return new GenCode(setCode);
            }
            return null;
        }

        protected override ISTNodeCodeSnippet EmitMemberRef(ISTNodeCodeSnippet InHost, string InMemberID, EInstructionUsage InUsage)
        {
            switch (InUsage)
            {
                case EInstructionUsage.Load:
                case EInstructionUsage.Call:
                    string code = $"MBRLD {InHost.PresentCode}.{InMemberID}";
                    _Results.Add(code);
                    return new GenCode(code);
                case EInstructionUsage.Set:
                    string setCode = $"REFMBRSET {InHost.PresentCode}.{InMemberID}";
                    _Results.Add(setCode);
                    return new GenCode(setCode);
            }
            return null;
        }

        protected override ISTNodeCodeSnippet EmitAssign(ISTNodeCodeSnippet InLhsCode, ISTNodeCodeSnippet InRhsCode)
        {
            string lhsCode = InLhsCode != null ? InLhsCode.PresentCode : "ERROR_LHS";
            string rhsCode = InRhsCode != null ? InRhsCode.PresentCode : "ERROR_RHS";
            string code = $"ASSIGN ({lhsCode}) ({rhsCode})";
            _Results.Add(code);
            return new GenCode(code);
        }

        protected override ISTNodeCodeSnippet EmitBinOp(string InOpCode, ISTNodeCodeSnippet InLeftCode, ISTNodeCodeSnippet InRightCode)
        {
            string lhsCode = InLeftCode != null ? InLeftCode.PresentCode : "ERROR_LEFT";
            string rhsCode = InRightCode != null ? InRightCode.PresentCode : "ERROR_RIGHT";
            string code = $"BINOP{InOpCode} ({lhsCode}) ({rhsCode})";
            _Results.Add(code);
            return new GenCode(code);
        }

        protected override ISTNodeCodeSnippet EmitUnaryOp(string InOpCode, ISTNodeCodeSnippet InRhsCode)
        {
            throw new NotImplementedException();
        }

        protected override ISTNodeCodeSnippet EmitCall(ISTNodeCodeSnippet InSourceCode, ISTNodeCodeSnippet[] InParamCodes)
        {
            throw new NotImplementedException();
        }

        protected override ISTNodeCodeSnippet EmitNew(Info InArchetype, ISTNodeCodeSnippet[] InParamCodes)
        {
            throw new NotImplementedException();
        }
    }
}