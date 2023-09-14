// using nf.protoscript.syntaxtree;
// using System;
// using System.Collections.Generic;

// namespace nf.protoscript.translator.expression
// {

//     /// <summary>
//     /// Expression code prototype.
//     /// </summary>
//     /// <example>
//     /// Here is a simplest code prototype:
//     ///     $PRESENT$
//     /// 
//     /// Here is a complex code prototype:
//     /// 
//     ///     $PRE_DEFINITIONS
//     /// 
//     ///     try 
//     ///     {
//     ///         $PRESENT$ // Special mark which will be replaced by translating codes.
//     ///     }
//     ///     catch (...)
//     ///     {
//     ///         $CATCH_CODES
//     ///     }
//     ///     catch (...)
//     ///     {
//     ///         $CATCH_CODES
//     ///     }
//     ///     finally
//     ///     {
//     ///         $FINAL_CODES
//     ///     }
//     /// 
//     /// </example>
//     public interface IExprCodePrototype
//     {
//         /// <summary>
//         /// Results.
//         /// </summary>
//         public IReadOnlyList<string> Results { get; }

//         /// <summary>
//         /// Create a instance for a generating transaction.
//         /// </summary>
//         /// <returns></returns>
//         IExprCodePrototype Clone();

//     }

//     /// <summary>
//     /// CodeTemplate for a specific STNode (instruction).
//     /// 
//     /// Codes are stored in several domains which listed in CodePrototype.
//     /// </summary>
//     public interface ISTNodeCodeTemplate
//     {
//         /// <summary>
//         /// get human-readable template contents.
//         /// </summary>
//         string TemplateCodes { get; }

//     }

//     /// <summary>
//     /// CodeTemplate Table. To find code template for specific context.
//     /// </summary>
//     public interface IExprCodeTemplateTable
//     {
//         /// <summary>
//         /// Check if a type is a value type.
//         /// A value-type variable may be handled as a 'set-back' property.
//         /// </summary>
//         /// <param name="InType"></param>
//         /// <returns></returns>
//         bool IsValueType(TypeInfo InType);

//         /// <summary>
//         /// Get Var-Ref code.
//         /// The Var-Ref code should be used both in loading or setting like:
//         ///     t = {GlobalOrLocalVar}
//         ///     t = {$OWNER.MemberVar}
//         ///     OR
//         ///     {GlobalOrLocalVar} = $RHS
//         ///     {$OWNER.MemberVar} = $RHS
//         /// </summary>
//         /// <param name="InScope"></param>
//         /// <param name="InOwnerString"></param>
//         /// <param name="InVarID"></param>
//         /// <returns>
//         /// return null if not support Var-Ref for the target Var (e.g. getter/setter property, readonly property ...).
//         /// </returns>
//         ISTNodeCodeTemplate FindVarRefTemplate(Info InScope, string InOwnerString, string InVarID);

//         /// <summary>
//         /// Get Var-load code:
//         ///     t = {GlobalOrLocalVar}
//         ///     OR
//         ///     t = {$OWNER.MemberVar}
//         /// </summary>
//         /// <param name="InScope"></param>
//         /// <param name="InVarID"></param>
//         /// <returns></returns>
//         ISTNodeCodeTemplate FindVarGetTemplate(Info InScope, string InOwnerString, string InVarID);

//         /// <summary>
//         /// Get Var-set code:
//         ///     {GlobalOrLocalVar} = $RHS
//         ///     OR
//         ///     {$OWNER.MemberVar} = $RHS
//         /// </summary>
//         /// <param name="InScope"></param>
//         /// <param name="InVarID"></param>
//         /// <param name="InRhsCode"></param>
//         /// <returns></returns>
//         ISTNodeCodeTemplate FindVarSetTemplate(Info InScope, string InOwnerString, string InVarID);

//         /// <summary>
//         /// Get Bin-Op code: {$LHS $OpCode $RHS}
//         /// </summary>
//         /// <param name="InScope"></param>
//         /// <param name="InBinOpCode"></param>
//         /// <returns></returns>
//         ISTNodeCodeTemplate FindBinOpTemplate(Info InLhsScope, Info InRhsScope, string InBinOpCode);

//         /// <summary>
//         /// Get call code.
//         /// </summary>
//         /// <param name="InScope"></param>
//         /// <param name="InFuncID"></param>
//         /// <param name="InParams"></param>
//         /// <returns></returns>
//         ISTNodeCodeTemplate FindCallTemplate(Info InScope, string InFuncID, string[] InParams);

//     }


//     /// <summary>
//     /// Template based expr-code generator.
//     /// </summary>
//     public class ExprCodeGenerator_TemplateBased
//         : ExprCodeGenerator_DomainBased
//     {

//         /// <summary>
//         /// Code generating instance cloned from the CodePrototype.
//         /// </summary>
//         IExprCodePrototype CodeGenerating { get; }

//         /// <summary>
//         /// Registered code templates
//         /// </summary>
//         IExprCodeTemplateTable CodeTemplateTable { get; }

//         // Begin Domain based overrides.

//         public override IReadOnlyList<string> Results { get { return CodeGenerating.Results; } }

//         protected override ISTNodeResultPlaceholder AllocPlaceholderForSubNode(EInstructionUsage InUsage)
//         {
//             throw new NotImplementedException();
//         }

//         protected override void EmitConstStringToDomain(IDomainBasedPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InTextString)
//         {
//             InTargetPlaceholder.SetPresentCode(InTextString);
//         }

//         //protected override void EmitVarRefToDomain(ISTNodeResultPlaceholder InTargetPlaceholder, Info InScope, string InVarID, EInstructionUsage InUsage)
//         //{
//         //    var varRefTemplate = CodeTemplateTable.FindVarRefTemplate(InScope, "", InVarID);
//         //    if (varRefTemplate != null)
//         //    {
//         //        throw new NotImplementedException();
//         //        //varRefTemplate
//         //        //return varRefTemplate.GenCode();
//         //    }
//         //    else
//         //    {
//         //        var varGetTemplate = CodeTemplateTable.FindVarGetTemplate(InScope, "", InVarID);
//         //        if (varGetTemplate == null)
//         //        {
//         //            // TODO log error.
//         //            throw new NotImplementedException();
//         //        }

//         //        // If the variable is used as 'set' (LHS like $Target = 100), try generating 'set-back' after set-operation applied (ASSIGNMENT, OUT_PARAM ...).
//         //        if (InUsage == EInstructionUsage.Set)
//         //        {
//         //            var varSetTemplate = CodeTemplateTable.FindVarSetTemplate(InScope, "", InVarID);
//         //            if (varSetTemplate == null)
//         //            {
//         //                // TODO log error.
//         //                throw new NotImplementedException();
//         //            }

//         //            // register 'set-back' to PostSet

//         //        }

//         //        //return varGetTemplate.PredictScope;
//         //    }
//         //}

//         // ~ End Domain based overrides.

//     }


//     ///// <summary>
//     ///// Template based emitter. Find codes from code-factory.
//     ///// </summary>
//     //public class ExprCodeEmitter_TemplateBased
//     //    : IExprCodeEmitter
//     //{
//     //public ExprCodeEmitter_TemplateBased(IExprCodePrototype InCodePrototype, Info InScope)
//     //{
//     //    GeneratingCode = InCodePrototype.Clone();

//     //    Scope = InScope;
//     //}

//     ///// <summary>
//     ///// Expression code generating.
//     ///// </summary>
//     //public IExprCodePrototype GeneratingCode { get; }

//     //    public class InstructionCode
//     //        : ISTNodeCodeSnippet
//     //    {
//     //        public IEnumerable<string> CodeLines
//     //        {
//     //            get
//     //            {
//     //                throw new NotImplementedException();
//     //            }
//     //        }

//     //        public string PresentCode { get; }

//     //    }

//     //    public IExprCodeFactory CodeFactory { get; }

//     //    public ISTNodeCodeSnippet EmitConstInfo(TypeInfo InValueType, Info InConstInfo)
//     //    {
//     //        var codeTemplate = CodeFactory.FindConstTemplate(InValueType, InConstInfo);

//     //        codeTemplate.PresentCode;
//     //    }

//     //    public ISTNodeCodeSnippet EmitConstString(TypeInfo InValueType, string InTextString)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitConstValueCode(TypeInfo InValueType, string InConstValueString)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitVarRef(Info InScope, string InVarID, EInstructionUsage InUsage)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitMemberRef(ISTNodeCodeSnippet InHost, string InMemberId, EInstructionUsage InUsage)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitAssign(ISTNodeCodeSnippet InLhsCode, ISTNodeCodeSnippet InRhsCode)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitBinOp(string InOpCode, ISTNodeCodeSnippet InLhsCode, ISTNodeCodeSnippet InRhsCode)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitUnaryOp(string InOpCode, ISTNodeCodeSnippet InRhsCode)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitCall(ISTNodeCodeSnippet InSourceCode, ISTNodeCodeSnippet[] InParamCodes)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public ISTNodeCodeSnippet EmitNew(Info InArchetype, ISTNodeCodeSnippet[] InParamCodes)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //}


// }
