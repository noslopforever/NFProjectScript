// using nf.protoscript.syntaxtree;
// using System;
// using System.Collections.Generic;

// namespace nf.protoscript.translator.expression
// {
//     ///// <summary>
//     ///// for each handling instruction: load parameters from, and save results to the register-data-pool.
//     ///// 
//     ///// e.g.
//     ///// 
//     /////     a = b + c
//     /////         LOAD b TO $rd0
//     /////         LOAD c TO $rd1
//     /////         ADD $rd0 $rd1 TO $rd2
//     /////         SET a BY $rd2
//     ///// 
//     /////     int foo(int p0, int p1);
//     /////     
//     /////         LOAD p0 TO $rd0
//     /////         LOAD p1 TO $rd1
//     /////         CALL foo $rd0 $rd1 TO $rd2
//     /////
//     ///// </summary>
//     //public abstract class ExprCodeEmitter_RDBased
//     //    : IExprCodeEmitter
//     //{

//     //}


//     ///// <summary>
//     ///// A context to find variables.
//     ///// </summary>
//     //public interface IILScope
//     //{
//     //    IILVariable RegTempVar(ISyntaxTreeNode InSourceSyntax);
//     //}

//     ///// <summary>
//     ///// Immediately-language instructions with unique functions.
//     ///// </summary>
//     //public interface IILInstruction
//     //{

//     //    /// <summary>
//     //    /// Instruction target. The result of the instruction will be written to this variable.
//     //    /// </summary>
//     //    TempVariable TargetVariable { get; }

//     //    /// <summary>
//     //    /// Instruction parameters.
//     //    /// </summary>
//     //    TempVariable[] Params { get; }


//     //    public IReadOnlyCollection<IILInstruction> PostInstruction { get; }

//     //}



//     /// <summary>
//     /// Register-Data based expr code generator.
//     /// To generate RD based codes like:
//     ///     a = b + c
//     ///         LOAD b TO $rd0
//     ///         LOAD c TO $rd1
//     ///         ADD $rd0 $rd1 TO $rd2
//     ///         SET a BY $rd2
//     /// 
//     /// The Register-Data will be taken as the Placeholder.
//     /// 
//     /// </summary>
//     public class ExprCodeGenerator_TempVarBased
//         : ExprCodeGeneratorAbstract
//     {
//         public ExprCodeGenerator_TempVarBased()
//         {
//             // Statement Domain Configuration:
//             //
//             //  TEMP-VAR INIT domain: All instructions like $TempVar = INSTRUCTION
//             //  STATEMENT Present: like CALL, ASSIGN, and other individual statements.
//             //  SETBACK domain: All set-back instructions like A.set_HP($TempVar).
//             //      This domain is a 'reverse' domain that all instructions in it will be accessed from


//         }

//         /// <summary>
//         /// Temporary variable that to store immediate results for each STNode.
//         /// </summary>
//         public class TempVariable
//         {
//             private TempVariable()
//             { }

//             internal TempVariable(ExprCodeGenerator_TempVarBased InHostGenerator, int InTempVarID)
//             {
//                 HostGenerator = InHostGenerator;
//                 TempVarID = InTempVarID;
//             }

//             /// <summary>
//             /// Host generator which holds this temp-var.
//             /// </summary>
//             public ExprCodeGenerator_TempVarBased HostGenerator { get; }

//             /// <summary>
//             /// ID of the TempVar
//             /// </summary>
//             internal int TempVarID { get; }

//             /// <summary>
//             /// Standard temporary variable name.
//             /// </summary>
//             public string Name
//             {
//                 get { return $"_NPS_GEN_TEMP_{TempVarID}"; }
//             }

//             /// <summary>
//             /// The source expression which initializes the TempVar.
//             /// 
//             /// e.g.
//             ///     Temp0 = Foo(A, B)
//             /// In this case, the source of the Temp0 should be: STNode_Call(Foo) { params=[ A, B ] }
//             ///     
//             /// e.g.
//             ///     Temp0 = A.B
//             /// In this case, the source of the Temp0 should be: STNode_Sub { lhs = A; rhs = B; }
//             ///     
//             /// </summary>
//             public ISyntaxTreeNode Source { get; }

//             /// <summary>
//             /// RD's PRESENT will always be the name of temp-var.
//             /// </summary>
//             public string PresentCode { get { return Name; } }

//         }

//         /// <summary>
//         /// Stage to save TempVar-Init/TempVar-SetBack codes.
//         /// </summary>
//         public class StatementStage
//             : Stage
//         {
//             public StatementStage(Stage InParentStage, ISyntaxTreeNode InBoundSTNode)
//                 : base(InParentStage, EStageType.Statement, InBoundSTNode)
//             {
//             }

//             /// <summary>
//             /// TempVar registerred in the current statement.
//             /// </summary>
//             List<TempVariable> _tempVarRegisterred = new List<TempVariable>();

//         }

//         /// <summary>
//         /// Placeholder to save variables in.
//         /// </summary>
//         public class Placeholder
//             : ISTNodeResultPlaceholder
//         {
//             public Placeholder(ExprCodeGenerator_TempVarBased InHost, EInstructionUsage InUsage)
//             {
//                 Usage = InUsage;
//                 HostGenerator = InHost;
//             }

//             // Host generator which create this placeholder.
//             ExprCodeGenerator_TempVarBased HostGenerator { get; }

//             // temporary variable registered in the host generator.
//             internal int _tempVarID = -1;

//             // The present code when there is no temporary variable
//             internal string _presentCode = "";

//             // BEGIN ISTNodeResultPlaceholder interfaces

//             public EInstructionUsage Usage { get; }

//             public string PresentCode
//             {
//                 get
//                 {
//                     if (_tempVarID == -1)
//                     {
//                         return _presentCode;
//                     }
//                     var tempVar = HostGenerator.GetTempVar(_tempVarID);
//                     return tempVar.Name;
//                 }
//             }

//             // ~ END ISTNodeResultPlaceholderInterfaces

//             /// <summary>
//             /// Try get the temp-var bound with this placeholder.
//             /// </summary>
//             public TempVariable TempVar
//             {
//                 get
//                 {
//                     if (_tempVarID != -1)
//                     {
//                         return HostGenerator.GetTempVar(_tempVarID);
//                     }
//                     return null;
//                 }
//             }

//         }


//         public override IReadOnlyList<string> Results => throw new System.NotImplementedException();


//         // BEGIN temp-var management

//         List<TempVariable> _tempVariables = new List<TempVariable>();

//         private TempVariable AddTempVar()
//         {
//             var newTempVar = new TempVariable(this, _tempVariables.Count);
//             _tempVariables.Add(newTempVar);
//             return newTempVar;
//         }

//         private TempVariable GetTempVar(int tempVarID)
//         {
//             return _tempVariables[tempVarID];
//         }

//         // ~ END temp-var management


//         // BEGIN stages

//         protected override Stage EnterStage(EStageType InStageType, ISyntaxTreeNode InSTNode)
//         {
//             return base.EnterStage(InStageType, InSTNode);
//         }

//         protected override void LeaveStage(EStageType InStageType, ISyntaxTreeNode InSTNode)
//         {
//             base.LeaveStage(InStageType, InSTNode);

//             if (InStageType == EStageType.Statement)
//             {
//                 // for each RD set by the statement, try write it back to its origin.
//             }
//         }

//         protected override Stage AllocStage(Stage InParentStage, EStageType InStageType, ISyntaxTreeNode InSTNode)
//         {
//             if (InStageType == EStageType.Statement)
//             {
//                 return new StatementStage(InParentStage, InSTNode);
//             }
//             return base.AllocStage(InParentStage, InStageType, InSTNode);
//         }

//         protected override void EnterNode(ISyntaxTreeNode InNode)
//         {
//         }

//         protected override void LeaveNode(ISyntaxTreeNode InNode)
//         {
//         }

//         // ~ END stages

//         // Begin AllocPlaceholderForSubNode

//         protected override ISTNodeResultPlaceholder AllocPlaceholderForSubNode(EInstructionUsage InUsage)
//         {
//             return new Placeholder(this, InUsage);
//         }

//         protected override void PreAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
//         {
//             // Generate pre-access codes for TempVar.
//         }

//         protected override void PostAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
//         {
//             // Generate post-access (like set-backs, change-notifies) for a TempVar
//             var ph = InPlaceholder as Placeholder;
//             // try handle set-backs
//             if (ph.Usage == EInstructionUsage.Set)
//             {
//                 var tempVar = ph.TempVar;
//                 if (tempVar != null)
//                 {
//                     throw new NotImplementedException();
//                 }
//             }

//         }

//         // ~ End AllocPlaceholderForSubNode

//         private string _GetPresentFromPH(ISTNodeResultPlaceholder InPlaceholder)
//         {
//             var ph = InPlaceholder as Placeholder;
//             return ph.PresentCode;
//         }

//         protected override sealed void EmitConstInfo(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, Info InConstInfo)
//         {
//             throw new System.NotImplementedException();
//         }
//         protected override sealed void EmitConstString(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InTextString)
//         {
//             string code = $"\"{InTextString}\"";

//             var ph = InTargetPlaceholder as Placeholder;
//             ph._presentCode = code;
//         }
//         protected override sealed void EmitConstValueCode(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InConstValueString)
//         {
//             var ph = InTargetPlaceholder as Placeholder;
//             ph._presentCode = InConstValueString;
//         }

//         protected override sealed void EmitAssign(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InLhsCode, ISTNodeResultPlaceholder InRhsCode)
//         {
//             var lhsPresent = _GetPresentFromPH(InLhsCode);
//             var rhsPresent = _GetPresentFromPH(InRhsCode);

//             // Emit assign instruction:
//             //     $Lhs = $Rhs
//             //     ASSIGN TARGET($Rhs) SOURCE($Lhs)
//             TempVarEmit_Assign(InStage, lhsPresent, rhsPresent);

//             // If there is a holder, set $Lhs to the result of the instruction.
//             var ph = InTargetPlaceholder as Placeholder;
//             if (ph != null)
//             {
//                 var tempVar = AddTempVar();
//                 TempVarEmit_Assign(InStage, tempVar.Name, lhsPresent);
//                 ph._tempVarID = tempVar.TempVarID;
//             }
//         }
//         protected virtual void TempVarEmit_Assign(Stage InStage, string InLhsPresent, string InRhsPresent)
//         {
//             InStage.AddCodeLine($"{InLhsPresent} = {InRhsPresent}");
//             //InStage.AddCodeLine($"{InResultTempVar.Name} = {InLhsPresent}");
//         }

//         protected override sealed void EmitBinOp(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InLeftCode, ISTNodeResultPlaceholder InRightCode)
//         {
//             var leftPresent = _GetPresentFromPH(InLeftCode);
//             var rightPresent = _GetPresentFromPH(InRightCode);

//             // Emit BinOp instruction:
//             //      $R = $Left $OP $Right
//             var tempVar = AddTempVar();
//             TempVarEmit_BinOp(tempVar, InOpCode, leftPresent, rightPresent);
//         }

//         protected virtual void TempVarEmit_BinOp(TempVariable InResultVar, string InOpCode, string InLeftRD, string InRightRD)
//         {
//             var tempVar = AddTempVar();
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitCall(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InSourceCode, ISTNodeResultPlaceholder[] InParamCodes)
//         {
//             string[] paramPresents = new string[InParamCodes.Length];
//             for (int i = 0; i < InParamCodes.Length; i++)
//             {
//                 paramPresents[i] = _GetPresentFromPH(InParamCodes[i]);
//             }
//             var funcPresent = _GetPresentFromPH(InSourceCode);
//             TempVarEmit_Call(funcPresent, paramPresents);
//         }

//         protected virtual void TempVarEmit_Call(string funcPresent, string[] paramPresents)
//         {
//             var tempVar = AddTempVar();
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitVarRef(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, Info InScope, string InVarID, EInstructionUsage InUsage)
//         {
//             switch (InTargetPlaceholder.Usage)
//             {
//                 case EInstructionUsage.Load:
//                 case EInstructionUsage.Call:
//                     break;
//                 case EInstructionUsage.Set:
//                     // TODO register set-back after statement
//                     break;
//             }
//         }

//         protected override sealed void EmitMemberRef(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InHost, string InMemberId, EInstructionUsage InUsage)
//         {
//             switch (InTargetPlaceholder.Usage)
//             {
//                 case EInstructionUsage.Load:
//                 case EInstructionUsage.Call:
//                     break;
//                 case EInstructionUsage.Set:
//                     break;
//             }
//         }

//         protected override sealed void EmitNew(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, Info InArchetype, ISTNodeResultPlaceholder[] InParamCodes)
//         {
//             throw new System.NotImplementedException();
//         }

//         protected override sealed void EmitUnaryOp(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InRhsCode)
//         {
//             throw new System.NotImplementedException();
//         }

//     }




// }
