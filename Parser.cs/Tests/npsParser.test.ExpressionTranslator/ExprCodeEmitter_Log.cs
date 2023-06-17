using nf.protoscript;
using nf.protoscript.translator.expression;
using System.Collections.Generic;
using System.IO;

namespace npsParser.test.ExpressionTranslator
{
    internal class ExprCodeEmitter_Log
        : IExprCodeEmitter
    {
        public ExprCodeEmitter_Log()
        {
        }

        class GenCode
            : IInstructionCode
        {
            public GenCode(string InCode)
            {
                PresentCode = InCode;
                _ResultCodeLines.Add(PresentCode);
            }
            public GenCode(IEnumerable<string> InCodes, string InPresentCode)
            {
                _ResultCodeLines.AddRange(InCodes);
                PresentCode = InPresentCode;
            }

            public string PresentCode { get; }

            public IEnumerable<string> Codes { get { return _ResultCodeLines; } }
            List<string> _ResultCodeLines = new List<string>();
        }

        public IInstructionCode EmitConstValueCode(string InValueString)
        {
            return new GenCode(InValueString);
        }

        public IInstructionCode EmitConstString(string InTextString)
        {
            string code = $"\"{InTextString}\"";
            return new GenCode(code);
        }

        public IInstructionCode EmitAssign(IInstructionCode InLhsCode, IInstructionCode InRhsCode)
        {
            string lhsCode = InLhsCode != null ? InLhsCode.PresentCode : "ERROR_LHS";
            string rhsCode = InRhsCode != null ? InRhsCode.PresentCode : "ERROR_RHS";
            string code = $"ASSIGN ({lhsCode}) ({rhsCode})";
            return new GenCode(code);
        }

        public IInstructionCode EmitBinOp(string InOpCode, IInstructionCode InLhsCode, IInstructionCode InRhsCode)
        {
            throw new System.NotImplementedException();
        }

        public IInstructionCode EmitCall(IInstructionCode InSourceCode, IInstructionCode[] InParamCodes)
        {
            throw new System.NotImplementedException();
        }

        public IInstructionCode EmitNew(Info InArchetype, IInstructionCode[] InParamCodes)
        {
            throw new System.NotImplementedException();
        }

        public IInstructionCode EmitRefVarForSet(Info InScope, string InVarID)
        {
            string code = $"REFSET {InScope.Name}::{InVarID}";
            return new GenCode(code);
        }

        public IInstructionCode EmitSubVarLoad(IInstructionCode InSourceCode, string InVarID)
        {
            throw new System.NotImplementedException();
        }

        public IInstructionCode EmitSubVarSet(IInstructionCode InSourceCode, string InVarID, IInstructionCode InRhsCode)
        {
            throw new System.NotImplementedException();
        }

        public IInstructionCode EmitUnaryOp(string InOpCode, IInstructionCode InRhsCode)
        {
            throw new System.NotImplementedException();
        }

        public IInstructionCode EmitVarLoad(Info InScope, string InVarID)
        {
            string code = $"LD {InScope.Name}::{InVarID}";
            return new GenCode(code);
        }
    }
}