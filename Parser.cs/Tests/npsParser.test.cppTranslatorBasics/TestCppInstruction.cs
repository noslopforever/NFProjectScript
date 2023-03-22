using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.IO;

namespace nf.protoscript.test
{

    /// <summary>
    /// Atomic and simplest Cpp-language instructions like ASSIGN, CALL, BINARY_OPs, ADDR, REF.
    /// 
    /// - Each instruction has a result.
    /// 
    /// </summary>
    abstract class CppILInstruction
    {
        public CppILInstruction(CppFunction InHostFunction)
        {
            HostFunction = InHostFunction;
        }

        /// <summary>
        /// Host function of instructions.
        /// Where can also host the temporary variables needed by the instruction.
        /// </summary>
        public CppFunction HostFunction { get; }

        /// <summary>
        /// 
        /// </summary>
        public CppILInstruction[] RefInstructions { get; protected set; } = new CppILInstruction[] { };

        /// <summary>
        /// Generate code and return the result signature of the instruction.
        /// </summary>
        /// <param name="InCodeList"></param>
        /// <returns></returns>
        internal protected abstract string GenCode(IList<String> InCodeList);

        /// <summary>
        /// Some instructions need 'set-back' code. 
        /// e.g.
        ///     tmp = A.Foo();
        ///     tmp = DestRhs;
        ///     
        ///     A.SetFoo(tmp);  // <- set-back
        /// </summary>
        /// <param name="InCodeList"></param>
        internal protected virtual void ConditionalGenSetbackCode(IList<String> InCodeList)
        {
            foreach (var instruction in RefInstructions)
            {
                instruction.ConditionalGenSetbackCode(InCodeList);
            }
        }

        /// <summary>
        /// Mark this instruction as 'to be modified'
        /// </summary>
        internal protected virtual void MarkModified()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// We purpose this instruction to have a ref-result.
        /// </summary>
        internal protected virtual void RequestRef()
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// $R: Ref($T)
    /// </summary>
    class CppILInstruction_Var
        : CppILInstruction
    {
        public CppILInstruction_Var(CppFunction InHostFunction)
            : base(InHostFunction)
        {
        }

        public enum EAccessType
        {
            Value,
            Ref,
        }

        /// <summary>
        /// Value-access or ref-access?
        /// </summary>
        public EAccessType AccessType { get; internal set; }

        /// <summary>
        /// Is variable constant?
        /// </summary>
        public bool Constant { get; internal set; } = false;

        /// <summary>
        /// 
        /// null to disable RefCode
        /// </summary>
        public string RefCode { get; internal set; } = null;

        /// <summary>
        /// 
        /// null to disable Setter.
        /// </summary>
        public string SetCode { get; internal set; } = null;

        /// <summary>
        /// 
        /// Must be avaliable.
        /// </summary>
        public string GetCode { get; internal set; } = "";

        /// <summary>
        /// return if RefCode is non-null.
        /// </summary>
        public bool IsSupportRef { get { return RefCode != null; } }

        /// <summary>
        /// Is the instruction pending to be modified?
        /// </summary>
        public bool IsModifyMarked { get; private set; } = false;

        /// <summary>
        /// The host instruction requires this variable to be a ref.
        /// </summary>
        public bool IsRefRequired { get; private set; } = false;

        /// <summary>
        /// Name of the TempVar used in 'get/modify/set-back' routine.
        /// </summary>
        public string TempVarName { get; private set; } = null;

        internal protected override void MarkModified()
        {
            IsModifyMarked = true;
        }
        internal protected override void RequestRef()
        {
            IsRefRequired = true;
        }

        internal protected override string GenCode(IList<String> InCodeList)
        {
            // not ref-required, return getter immediately.
            if (!IsRefRequired)
            {
                return $"{GetCode}";
            }

            // ref required

            // but invalid
            if (!IsSupportRef)
            {
                // if ref for write, create a 'get/modify/set-back' routine.
                if (IsModifyMarked)
                {
                    // Trigger get/modify/set-back routine:
                    //      auto temp = get_Source();
                    //      temp = target;
                    //      set_Source(temp);
                    var tmpVar = HostFunction.TryRegTempVar("auto");
                    TempVarName = tmpVar.Name;

                    // register preparation codes.
                    InCodeList.Add($"auto {TempVarName} = {GetCode};");

                    return $"{TempVarName}";
                }

                // Use getter instead.
                return $"{GetCode}";
            }

            return $"{RefCode}";
        }

        internal protected override void ConditionalGenSetbackCode(IList<String> InCodeList)
        {
            if (!IsSupportRef)
            {
                if (IsModifyMarked)
                {
                    // Complete the 'get/modify/set-back' routine.
                    // See ConditionalGenPrepareCode for more informations.
                    string setter = $"{SetCode};";
                    setter = setter.Replace("$RHS", $"{TempVarName}");
                    InCodeList.Add(setter);
                }
            }

            base.ConditionalGenSetbackCode(InCodeList);
        }


    }


    /// <summary>
    /// $R: (A $op B)
    /// </summary>
    class CppILInstruction_BinaryOp
        : CppILInstruction
    {
        public CppILInstruction_BinaryOp(CppFunction InHostFunction, string InOpCode, CppILInstruction InLhs, CppILInstruction InRhs)
            : base(InHostFunction)
        {
            this.OpCode = InOpCode;
            this.LhsInstruction = InLhs;
            this.RhsInstruction = InRhs;
            RefInstructions = new CppILInstruction[] { InLhs, InRhs };
        }

        /// <summary>
        /// Opcode of the instruction.
        /// </summary>
        public string OpCode { get; }

        /// <summary>
        /// The lhs instruction.
        /// </summary>
        public CppILInstruction LhsInstruction { get; }

        /// <summary>
        /// The rhs instruction.
        /// </summary>
        public CppILInstruction RhsInstruction { get; }

        internal protected override string GenCode(IList<String> InCodeList)
        {
            string lhs = LhsInstruction.GenCode(InCodeList);
            string rhs = RhsInstruction.GenCode(InCodeList);
            return $"({lhs} {OpCode} {rhs})";
        }

    }


    /// <summary>
    /// Call instruction,
    /// $R: Foo(p0,...pn) or new FooType(p0,...pn)
    /// </summary>
    class CppILInstruction_Call
        : CppILInstruction
    {
        public CppILInstruction_Call(CppFunction InHostFunction, string InCallCode, CppILInstruction[] InParams)
            : base(InHostFunction)
        {
            CallCode = InCallCode;
            RefInstructions = InParams;
            Params = InParams;
        }

        public CppILInstruction_Call(CppFunction InHostFunction, CppILInstruction InFuncExpr, CppILInstruction[] InParams)
            : base(InHostFunction)
        {
            CallCode = "$ERR_USE_FUNC_EXPR";
            FuncExpr = InFuncExpr;
            RefInstructions = InParams;
            Params = InParams;
        }

        /// <summary>
        /// Call code, should be function name "Foo", or "new FooType"
        /// </summary>
        public string CallCode { get; set; }

        /// <summary>
        /// Function expression: getFn()(), the getFn() will be the Function expression.
        /// </summary>
        public CppILInstruction FuncExpr { get; } = null;

        /// <summary>
        /// Parameters.
        /// </summary>
        public CppILInstruction[] Params { get; }

        internal protected override string GenCode(IList<String> InCodeList)
        {
            // Default call-code
            string callCode = $"{CallCode}(";

            // Modify the call-code by the function-expression.
            if (FuncExpr != null)
            {
                string lhs = FuncExpr.GenCode(InCodeList);
                callCode = $"{lhs}(";
            }

            // Push parameters
            for (int i = 0; i < Params.Length; i++)
            {
                var paramInst = Params[i].GenCode(InCodeList);
                if (i > 0)
                {
                    callCode += " ,";
                }

                callCode += paramInst;
            }
            callCode += ")";
            return callCode;
        }
    }

    /// <summary>
    /// $R: (A = B)
    /// </summary>
    class CppILInstruction_Assign
        : CppILInstruction
    {
        public CppILInstruction_Assign(CppFunction InHostFunction, CppILInstruction InLhs, CppILInstruction InRhs)
            : base(InHostFunction)
        {
            this.LhsInstruction = InLhs;
            this.RhsInstruction = InRhs;
            RefInstructions = new CppILInstruction[] { InLhs, InRhs };

            // The result of LhsInstruction should be a ref-var.
            LhsInstruction.RequestRef();

            // The result of LhsInstruction will be modified.
            LhsInstruction.MarkModified();
        }

        /// <summary>
        /// The lhs instruction.
        /// </summary>
        public CppILInstruction LhsInstruction { get; }

        /// <summary>
        /// The rhs instruction.
        /// </summary>
        public CppILInstruction RhsInstruction { get; }

        internal protected override string GenCode(IList<String> InCodeList)
        {
            string lhs = LhsInstruction.GenCode(InCodeList);
            string rhs = RhsInstruction.GenCode(InCodeList);
            string ret = $"({lhs} = {rhs})";
            return ret;
        }

    }

}
