using nf.protoscript.syntaxtree;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{

    /// <summary>
    /// Instruction code.
    /// </summary>
    public interface IInstructionCode
    {

        /// <summary>
        /// Code lines generated for this instruction, include child-instructions.
        /// </summary>
        IEnumerable<string> Codes { get; }

        /// <summary>
        /// Code of the instruction.
        /// </summary>
        string PresentCode { get; }

    }


    /// <summary>
    /// Code emitter.
    /// </summary>
    public interface IExprCodeEmitter
    {

        /// <summary>
        /// Emit codes for constant string.
        /// </summary>
        /// <param name="InTextString"></param>
        /// <returns></returns>
        IInstructionCode EmitConstString(string InTextString);

        /// <summary>
        /// Emit codes for constant number.
        /// </summary>
        /// <param name="InInteger"></param>
        /// <returns></returns>
        IInstructionCode EmitConstValueCode(string InValueString);

        /// <summary>
        /// Emit the variable's reference for accessing its content in future.
        /// </summary>
        /// <param name="InScope"></param>
        /// <param name="InVarID"></param>
        /// <returns></returns>
        IInstructionCode EmitVarLoad(Info InScope, string InVarID);

        /// <summary>
        /// Emit the variable's reference for setting it in future.
        /// </summary>
        /// <param name="InScope"></param>
        /// <param name="InVarID"></param>
        /// <returns></returns>
        IInstructionCode EmitRefVarForSet(Info InScope, string InVarID);

        IInstructionCode EmitSubVarLoad(IInstructionCode InSourceCode, string InVarID);

        IInstructionCode EmitSubVarSet(IInstructionCode InSourceCode, string InVarID, IInstructionCode InRhsCode);

        IInstructionCode EmitAssign(IInstructionCode InLhsCode, IInstructionCode InRhsCode);

        IInstructionCode EmitBinOp(string InOpCode, IInstructionCode InLhsCode, IInstructionCode InRhsCode);

        IInstructionCode EmitUnaryOp(string InOpCode, IInstructionCode InRhsCode);

        IInstructionCode EmitCall(IInstructionCode InSourceCode, IInstructionCode[] InParamCodes);

        IInstructionCode EmitNew(Info InArchetype, IInstructionCode[] InParamCodes);

    }



}
