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
    abstract class JsInstruction
    {
        public JsInstruction(JsFunction InHostFunction)
        {
            HostFunction = InHostFunction;
        }

        /// <summary>
        /// Host function of instructions.
        /// Where can also host the temporary variables needed by the instruction.
        /// </summary>
        public JsFunction HostFunction { get; }

        /// <summary>
        /// 
        /// </summary>
        public JsInstruction[] RefInstructions { get; protected set; } = new JsInstruction[] { };

        /// <summary>
        /// Generate code for a syntax-tree node.
        /// </summary>
        public static IList<String> GenCodeForExpr(JsFunction InFunction, ISyntaxTreeNode InSTNode)
        {
            List<string> strList = new List<string>();

            var instruction = _ExactInstructions(InFunction, InSTNode);
            if (instruction != null)
            {
                string instCode = instruction.GenCode(strList);
                strList.Add(instCode);

                instruction.ConditionalGenSetbackCode(strList);
            }
            else
            {
                strList.Add("ERR");
            }

            return strList;

        }


        /// <summary>
        /// Exact STNode to JS instructions.
        /// </summary>
        /// <param name="InFunction"></param>
        /// <param name="InSTNode"></param>
        /// <returns></returns>
        public static JsInstruction _ExactInstructions(JsFunction InFunction, ISyntaxTreeNode InSTNode)
        {
            // Assign => binop'='
            STNodeAssign stnAssign = InSTNode as STNodeAssign;
            if (stnAssign != null)
            {
                // Exact Instructions of sub ST tree
                var instLhs = _ExactInstructions(InFunction, stnAssign.LHS);
                var instRhs = _ExactInstructions(InFunction, stnAssign.RHS);

                var inst = new JsILInstruction_Assign(InFunction, instLhs, instRhs);

                return inst;
            }
            // BinOp => binop
            STNodeBinaryOp stnBinOp = InSTNode as STNodeBinaryOp;
            if (stnBinOp != null)
            {
                string opcode = "$ERR";
                switch (stnBinOp.OpCode)
                {
                    case STNodeBinaryOp.Def.Add: opcode = "+"; break;
                    case STNodeBinaryOp.Def.Sub: opcode = "-"; break;
                    case STNodeBinaryOp.Def.Mul: opcode = "*"; break;
                    case STNodeBinaryOp.Def.Div: opcode = "/"; break;
                    case STNodeBinaryOp.Def.Mod: opcode = "%"; break;
                }

                // Exact Instructions of sub ST tree
                var instLhs = _ExactInstructions(InFunction, stnBinOp.LHS);
                var instRhs = _ExactInstructions(InFunction, stnBinOp.RHS);
                var inst = new JsILInstruction_BinaryOp(
                    InFunction
                    , $"{opcode}"
                    , instLhs
                    , instRhs
                    );

                return inst;
            }
            // Sub => binOp .
            STNodeSub stnSub = InSTNode as STNodeSub;
            if (stnSub != null)
            {
                // Exact Instructions of sub ST tree
                var instLhs = _ExactInstructions(InFunction, stnBinOp.LHS);
                var instRhs = _ExactInstructions(InFunction, stnBinOp.RHS);
                var inst = new JsILInstruction_Sub(
                    InFunction
                    , instLhs
                    , instRhs
                    );

                return inst;
            }

            // VarGet => ref
            STNodeGetVar stnVarGet = InSTNode as STNodeGetVar;
            if (stnVarGet != null)
            {
                Info propInfo = InfoHelper.FindPropertyAlongScopeTree(InFunction.ContextInfo, stnVarGet.IDName);
                if (propInfo != null)
                {
                    return new JsILInstruction_Var(InFunction)
                    {
                        AccessType = JsILInstruction_Var.EAccessType.Ref,
                        Constant = false,
                        GetCode = propInfo.Extra.MemberGetExprCode,
                        RefCode = propInfo.Extra.MemberRefExprCode,
                        SetCode = propInfo.Extra.MemberSetExprCode,
                    };
                }

                // direct property/local/global/member, ref it directly.
                return new JsILInstruction_Var(InFunction)
                {
                    AccessType = JsILInstruction_Var.EAccessType.Ref,
                    Constant = false,
                    GetCode = stnVarGet.IDName,
                    SetCode = $"{stnVarGet.IDName} = $RHS",
                    RefCode = stnVarGet.IDName,
                };
            }

            // Const => constant.
            STNodeConstant stnConst = InSTNode as STNodeConstant;
            if (stnConst != null)
            {
                string constString = _GetConstString(stnConst);

                var inst = new JsILInstruction_Var(InFunction)
                {
                    AccessType = JsILInstruction_Var.EAccessType.Value,
                    Constant = true,
                    GetCode = constString,
                    SetCode = null,
                    RefCode = null,
                };
                return inst;
            }

            return null;
        }

        private static string _GetConstString(STNodeConstant InConst)
        {
            string constString = "$ERR_UnknownConst";

            if (InConst.Value == null)
            { constString = "null"; }
            else if (InConst.Value.GetType().IsValueType)
            { constString = InConst.Value.ToString(); }
            else if (InConst.Value is Info)
            { constString = (InConst.Value as Info).Name; }

            return constString;
        }

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
        /// Mark this instruction as 'some sub of it will be modified'.
        /// </summary>
        internal void MarkSubModified()
        {
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
    class JsILInstruction_Var
        : JsInstruction
    {
        public JsILInstruction_Var(JsFunction InHostFunction)
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
        /// Get value of the var, must be valid.
        /// </summary>
        public string GetCode { get; internal set; } = null;

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
        /// JS Always support ref access.
        /// </summary>
        public bool IsSupportRef { get { return true; } }

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

            // if set, create a 'get/modify/set-back' routine.
            if (IsModifyMarked)
            {
                // Trigger get/modify/set-back routine:
                //      auto temp = get_Source();
                //      temp = target;
                //      set_Source(temp);
                var tmpVar = HostFunction.TryRegTempVar();
                TempVarName = tmpVar;

                // register preparation codes.
                InCodeList.Add($"let {TempVarName} = {GetCode};");

                return $"{TempVarName}";
            }

            // value object (constant), return getter-code.
            if (!IsSupportRef)
            {
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
    class JsILInstruction_BinaryOp
        : JsInstruction
    {
        public JsILInstruction_BinaryOp(JsFunction InHostFunction, string InOpCode, JsInstruction InLhs, JsInstruction InRhs)
            : base(InHostFunction)
        {
            this.OpCode = InOpCode;
            this.LhsInstruction = InLhs;
            this.RhsInstruction = InRhs;
            RefInstructions = new JsInstruction[] { InLhs, InRhs };
        }

        /// <summary>
        /// Opcode of the instruction.
        /// </summary>
        public string OpCode { get; }

        /// <summary>
        /// The lhs instruction.
        /// </summary>
        public JsInstruction LhsInstruction { get; }

        /// <summary>
        /// The rhs instruction.
        /// </summary>
        public JsInstruction RhsInstruction { get; }

        internal protected override string GenCode(IList<String> InCodeList)
        {
            string lhs = LhsInstruction.GenCode(InCodeList);
            string rhs = RhsInstruction.GenCode(InCodeList);
            return $"({lhs} {OpCode} {rhs})";
        }

        protected internal override void MarkModified()
        {
            // Lhs's sub has been modified.
            LhsInstruction.MarkSubModified();

            // Only the rhs has been modified.
            RhsInstruction.MarkModified();
        }

    }

    class JsILInstruction_Sub
    : JsInstruction
    {
        public JsILInstruction_Sub(JsFunction InHostFunction, JsInstruction InLhs, JsInstruction InRhs)
            : base(InHostFunction)
        {
            this.LhsInstruction = InLhs;
            this.RhsInstruction = InRhs;
            RefInstructions = new JsInstruction[] { InLhs, InRhs };
        }

        /// <summary>
        /// The lhs instruction.
        /// </summary>
        public JsInstruction LhsInstruction { get; }

        /// <summary>
        /// The rhs instruction.
        /// </summary>
        public JsInstruction RhsInstruction { get; }

        internal protected override string GenCode(IList<String> InCodeList)
        {
            string lhs = LhsInstruction.GenCode(InCodeList);
            string rhs = RhsInstruction.GenCode(InCodeList);
            return $"({lhs}.{rhs})";
        }

    }



    /// <summary>
    /// $R: (A = B)
    /// </summary>
    class JsILInstruction_Assign
        : JsInstruction
    {
        public JsILInstruction_Assign(JsFunction InHostFunction, JsInstruction InLhs, JsInstruction InRhs)
            : base(InHostFunction)
        {
            this.LhsInstruction = InLhs;
            this.RhsInstruction = InRhs;
            RefInstructions = new JsInstruction[] { InLhs, InRhs };

            // The result of LhsInstruction should be a ref-var.
            LhsInstruction.RequestRef();

            // The result of LhsInstruction will be modified.
            LhsInstruction.MarkModified();
        }

        /// <summary>
        /// The lhs instruction.
        /// </summary>
        public JsInstruction LhsInstruction { get; }

        /// <summary>
        /// The rhs instruction.
        /// </summary>
        public JsInstruction RhsInstruction { get; }

        internal protected override string GenCode(IList<String> InCodeList)
        {
            string lhs = LhsInstruction.GenCode(InCodeList);
            string rhs = RhsInstruction.GenCode(InCodeList);
            string ret = $"({lhs} = {rhs})";
            return ret;
        }

    }

}
