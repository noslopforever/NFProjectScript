// using nf.protoscript.syntaxtree;
// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using static nf.protoscript.translator.expression.ExprCodeGenerator_DomainBased;

// namespace nf.protoscript.translator.expression
// {


//     /// <summary>
//     /// Key of a domain, which should be used as a key to find a target domain.
//     /// </summary>
//     public struct DomainKey
//         : IEquatable<DomainKey>
//     {
//         public DomainKey()
//         {
//         }

//         /// <summary>
//         /// Domain group which determines the code should be put into the begin of the function? the end of current block? or something like that.
//         /// </summary>
//         public ExprCodeGeneratorAbstract.EStageType DomainGroup = ExprCodeGeneratorAbstract.EStageType.Statement;

//         /// <summary>
//         /// Name of the target domain.
//         /// </summary>
//         public string DomainName = "";

//         /// <summary>
//         /// Domain step.
//         /// </summary>
//         /// <example>
//         /// Statement a --- Step = 1
//         ///     Statement b --- Step = 0 (in default), which means 'the last' statement.
//         ///         Current Node
//         /// </example>
//         public int Step = 0;

//         /// <summary>
//         /// Priority. The more InPriority, the earlier placement.
//         /// </summary>
//         /// <example>
//         /// Statement a
//         ///     Node a0: 
//         ///         emit "## this is code a0" in domain {StatementBegin, Priority = 100}
//         ///     Node a1:
//         ///         emit "## this is code a1" in domain {StatementBegin, Priority = 110}
//         ///         
//         /// The result should be:
//         ///     ## this is code a1
//         ///     ## this is code a0
//         ///     ## Present of statement a.
//         /// 
//         /// </example>
//         public int Priority = 100;

//         /// <see cref="IEquatable{T}.Equals(T?)" />
//         public bool Equals(DomainKey InOther)
//         {
//             return DomainGroup == InOther.DomainGroup
//                 && Step == InOther.Step
//                 && Priority == InOther.Priority
//                 ;
//         }

//     }




//     /// <summary>
//     /// Placeholder which should be created by classes derived ExprCodeGenerator_DomainBased.
//     /// </summary>
//     public interface IDomainBasedPlaceholder
//         : ISTNodeResultPlaceholder
//     {

//         /// <summary>
//         /// Does this PH have a domain with the specific name.
//         /// </summary>
//         /// <param name="InDomainName"></param>
//         /// <returns></returns>
//         bool HasDomain(string InDomainName);

//         /// <summary>
//         /// Set Present Code 
//         /// </summary>
//         /// <param name="InTextString"></param>
//         void SetPresentCode(string InTextString);

//         /// <summary>
//         /// Add code to a domain.
//         /// </summary>
//         /// <param name="InDomainKey"></param>
//         /// <param name="InDomainCode"></param>
//         void AddDomainCode(DomainKey InDomainKey, string InDomainCode);

//         /// <summary>
//         /// Add codes to a domain.
//         /// </summary>
//         /// <param name="InDomainKey"></param>
//         /// <param name="InDomainCode"></param>
//         void AddDomainCode(DomainKey InDomainKey, IReadOnlyList<string> InDomainCodes);

//         /// <summary>
//         /// Iterates emitted codes by each domain.
//         /// </summary>
//         void ForeachEmittedCodes(Action<DomainKey, IReadOnlyList<string>> InFunc);

//     }

//     /// <summary>
//     /// Default class implement IDomainBasedPlaceholder
//     /// </summary>
//     public class DomainBasedPlaceholder
//         : IDomainBasedPlaceholder
//     {
//         public DomainBasedPlaceholder(EInstructionUsage InUsage)
//         {
//             Usage = InUsage;
//         }

//         // BEGIN IDomainBasedPlaceholder members.
//         public EInstructionUsage Usage { get; }

//         public string PresentCode { get; private set; }
//         // ~ END IDomainBasedPlaceholder members.

//         public void ForeachEmittedCodes(Action<DomainKey, IReadOnlyList<string>> InFunc)
//         {
//             foreach (var kvp in _DomainCodeStore)
//             {
//                 //if (kvp.Key.ReverseDomain)
//                 //{
//                 //    var clonedList = new List<string>(kvp.Value);
//                 //    clonedList.Reverse();
//                 //    InFunc(kvp.Key, clonedList);
//                 //}
//                 //else
//                 //{
//                 InFunc(kvp.Key, kvp.Value);
//                 //}
//             }
//         }

//         /// <summary>
//         /// Codes emitted to domains.
//         /// </summary>
//         Dictionary<DomainKey, List<string>> _DomainCodeStore = new Dictionary<DomainKey, List<string>>();

//         public void SetPresentCode(string InTextString)
//         {
//             PresentCode = InTextString;
//         }

//         public bool HasDomain(string InDomainName)
//         {
//             throw new NotImplementedException();
//         }

//         public void AddDomainCode(DomainKey InDomainKey, string InDomainCode)
//         {
//             _EnsureDomainCodeStore(InDomainKey).Add(InDomainCode);
//         }

//         public void AddDomainCode(DomainKey InDomainKey, IReadOnlyList<string> InDomainCodes)
//         {
//             _EnsureDomainCodeStore(InDomainKey).AddRange(InDomainCodes);
//         }

//         // Find or add a domain-code-store.
//         List<string> _EnsureDomainCodeStore(DomainKey InDomainKey)
//         {
//             if (!_DomainCodeStore.ContainsKey(InDomainKey))
//             {
//                 var strList = new List<string>();
//                 _DomainCodeStore.Add(InDomainKey, strList);
//             }
//             return _DomainCodeStore[InDomainKey];
//         }

//     }


//     /// <summary>
//     /// Domain based ExprCodeGenerator.
//     /// 
//     /// + Organize codes by domains like Function-Begin, Statement-Begin.
//     /// + Support TempVars.
//     /// 
//     /// </summary>
//     public abstract class ExprCodeGenerator_DomainBased
//         : ExprCodeGeneratorAbstract
//     {

//         public class DomainBasedStage: Stage
//         {
//             public DomainBasedStage(Stage InParentStage, EStageType InStageType, ISyntaxTreeNode InBoundSTNode)
//                 :base(InParentStage, InStageType, InBoundSTNode)
//             {
//             }

//             /// <summary>
//             /// Domains registered to the group.
//             /// </summary>
//             List<Domain> _domains = new List<Domain>();

//             /// <summary>
//             /// Find a domain with InDomainName. If not exist, add a new one.
//             /// </summary>
//             /// <param name="InDomainName"></param>
//             /// <returns></returns>
//             /// <exception cref="NotImplementedException"></exception>
//             public Domain FindOrAddDomain(string InDomainName)
//             {
//                 throw new NotImplementedException();
//             }


//         }

//         /// <summary>
//         /// The code domain.
//         /// </summary>
//         public class Domain
//         {
//             public Domain(DomainBasedStage InHolderGroup
//                 , string InDomainName
//                 , ISyntaxTreeNode InBoundStage
//                 )
//             {
//                 DomainGroup = InHolderGroup;
//                 DomainName = InDomainName;
//                 BoundStage = InBoundStage;
//             }

//             /// <summary>
//             /// The domain group holds this domain.
//             /// </summary>
//             public DomainBasedStage DomainGroup { get; }

//             /// <summary>
//             /// Name of the domain.
//             /// </summary>
//             public string DomainName { get; }

//             /// <summary>
//             /// DomainBasedStage bound with the domain.
//             /// </summary>
//             public ISyntaxTreeNode BoundStage { get; }

//             /// <summary>
//             /// Is code added reversely? 
//             /// throw new NotImplementedException("拼写错误")
//             /// </summary>
//             public bool ReverseCodes { get; }

//             /// <summary>
//             /// Codes added to the domain.
//             /// </summary>
//             public List<string> Codes { get; } = new List<string>();

//             /// <summary>
//             /// Add code to this domain.
//             /// </summary>
//             /// <param name="InCode"></param>
//             public void AddCodeLine(string InCode)
//             {
//                 Codes.Add(InCode);
//             }

//         }

//         public override IReadOnlyList<string> Results
//         {
//             get
//             {
//                 throw new NotImplementedException();
//             }
//         }


//         //
//         //  Template managements
//         //


//         /// <summary>
//         /// The default code template.
//         /// </summary>
//         public DomainBasedCodeTemplate DefaultTemplate { get; private set; }

//         /// <summary>
//         /// Set the default code-template.
//         /// </summary>
//         /// <param name="InDefaultTemplate"></param>
//         public void SetDefaultCodeTemplate(DomainBasedCodeTemplate InDefaultTemplate)
//         {
//             DefaultTemplate = InDefaultTemplate;
//         }

//         ///// <summary>
//         ///// Add code-template for some condition.
//         ///// </summary>
//         ///// <param name="InCondition"></param>
//         ///// <param name="InTemplate"></param>
//         ///// <exception cref="NotImplementedException"></exception>
//         //public void AddCodeTemplate(Func<, bool> InCondition, DomainBasedCodeTemplate InTemplate)
//         //{
//         //    throw new NotImplementedException();
//         //}



//         //
//         //  Domain generators.
//         //


//         // allocate DomainBasedStage to store domain informations.
//         protected override Stage AllocStage(Stage InParentStage, EStageType InStageType, ISyntaxTreeNode InSTNode)
//         {
//             return new DomainBasedStage(InParentStage, InStageType, InSTNode); 
//         }

//         // allocate placeholder to retrieve sub-node's result.
//         protected override ISTNodeResultPlaceholder AllocPlaceholderForSubNode(EInstructionUsage InUsage)
//         {
//             return new DomainBasedPlaceholder(InUsage);
//         }

//         protected override void PreAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
//         {
//         }

//         protected override void PostAccessPlaceholder(ISTNodeResultPlaceholder InPlaceholder)
//         {
//         }


//         protected override sealed void EmitConstString(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InTextString)
//         {
//             if (InTargetPlaceholder != null)
//             {
//                 var dph = InTargetPlaceholder as IDomainBasedPlaceholder;
//                 EmitConstStringToDomain(dph, InValueType, InTextString);
//             }
//             else
//             {
//                 var dph = new DomainBasedPlaceholder(EInstructionUsage.Load);
//                 EmitConstStringToDomain(dph, InValueType, InTextString);
//             }
//         }

//         protected abstract void EmitConstStringToDomain(IDomainBasedPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InTextString);

//         protected override sealed void EmitConstValueCode(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, string InConstValueString)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitConstInfo(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, TypeInfo InValueType, Info InConstInfo)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitVarRef(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, Info InScope, string InVarID, EInstructionUsage InUsage)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitMemberRef(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InHost, string InMemberId, EInstructionUsage InUsage)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitAssign(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InLhsCode, ISTNodeResultPlaceholder InRhsCode)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitBinOp(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InLeftCode, ISTNodeResultPlaceholder InRightCode)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitUnaryOp(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, string InOpCode, ISTNodeResultPlaceholder InRhsCode)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitCall(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, ISTNodeResultPlaceholder InSourceCode, ISTNodeResultPlaceholder[] InParamCodes)
//         {
//             throw new NotImplementedException();
//         }

//         protected override sealed void EmitNew(Stage InStage, ISTNodeResultPlaceholder InTargetPlaceholder, Info InArchetype, ISTNodeResultPlaceholder[] InParamCodes)
//         {
//             throw new NotImplementedException();
//         }

//     }

//     ///// <summary>
//     ///// Default ExprCodeEmitter
//     ///// </summary>
//     //public abstract class ExprCodeEmitterBase
//     //    : IExprCodeEmitter
//     //{
//     //
//     //    public class InstCode
//     //        : IInstructionCode
//     //    {
//     //        public InstCode(Info InScope, string InPresentCode)
//     //        {
//     //            Scope = InScope;
//     //            PresentCode = InPresentCode;
//     //        }
//     //
//     //        /// <summary>
//     //        /// Type of the instruction's result.
//     //        /// </summary>
//     //        public Info Scope { get; private set; }
//     //
//     //        // IExprCodeEmitter.PresentCode
//     //        public string PresentCode { get; }
//     //
//     //        // IExprCodeEmitter.CodeLines
//     //        public IEnumerable<string> CodeLines
//     //        {
//     //            get
//     //            {
//     //                List<string> result = new List<string>();
//     //                result.AddRange(_PreCodeLines);
//     //                result.Add(PresentCode);
//     //                result.AddRange(_PostCodeLines);
//     //                return result;
//     //            }
//     //        }
//     //
//     //        List<string> _PreCodeLines = new List<string>();
//     //
//     //        List<string> _PostCodeLines = new List<string>();
//     //
//     //    }
//     //
//     //    public virtual InstCode GenSimpleCode(TypeInfo InCodeType, string InCode)
//     //    {
//     //        return new InstCode(InCodeType, InCode);
//     //    }
//     //
//     //    public virtual IInstructionCode EmitConstString(TypeInfo InValueType, string InTextString)
//     //    {
//     //        return GenSimpleCode(InValueType, $"\"{InTextString}\"");
//     //    }
//     //
//     //    public virtual IInstructionCode EmitConstValueCode(TypeInfo InValueType, string InValueString)
//     //    {
//     //        return GenSimpleCode(InValueType, InValueString);
//     //    }
//     //
//     //    public abstract IInstructionCode EmitConstInfo(Info InConstInfo);
//     //
//     //    public virtual IInstructionCode EmitVarLoad(Info InScope, string InVarID)
//     //    {
//     //        ElementInfo propInfo = InfoHelper.FindPropertyAlongScopeTree(InScope, InVarID);
//     //        if (propInfo != null)
//     //        {
//     //            return _GenVarLoad(InScope, propInfo, InVarID);
//     //        }
//     //
//     //        return _GenUnknownVarLoad(InScope, InVarID);
//     //    }
//     //
//     //    public virtual IInstructionCode EmitRefVarForSet(Info InScope, string InVarID)
//     //    {
//     //        ElementInfo propInfo = InfoHelper.FindPropertyAlongScopeTree(InScope, InVarID);
//     //        if (propInfo != null)
//     //        {
//     //            return _GenVarForSet(InScope, propInfo, InVarID);
//     //        }
//     //
//     //        return _GenUnknownVarForSet(InScope, InVarID);
//     //    }
//     //
//     //    public virtual IInstructionCode EmitSubVarLoad(IInstructionCode InSourceCode, string InVarID)
//     //    {
//     //        InstCode srcCode = InSourceCode as InstCode;
//     //        ElementInfo propInfo = InfoHelper.FindPropertyAlongScopeTree(srcCode.Scope, InVarID);
//     //        if (propInfo != null)
//     //        {
//     //            return _GenLoadSubVar(srcCode, propInfo, InVarID);
//     //        }

//     //        return _GenLoadUnknownSubVar(srcCode, srcCode.Scope, InVarID);
//     //    }

//     //    public virtual IInstructionCode EmitSubVarSet(IInstructionCode InSourceCode, string InVarID, IInstructionCode InRhsCode)
//     //    {
//     //        InstCode srcCode = InSourceCode as InstCode;
//     //        ElementInfo propInfo = InfoHelper.FindPropertyAlongScopeTree(srcCode.Scope, InVarID);
//     //        if (propInfo != null)
//     //        {
//     //            return _GenSubVarForSet(srcCode, propInfo, InVarID);
//     //        }

//     //        return _GenUnknownSubVarForSet(srcCode, srcCode.Scope, InVarID);
//     //    }

//     //    public virtual IInstructionCode EmitAssign(IInstructionCode InLhsCode, IInstructionCode InRhsCode)
//     //    {
//     //        var ilcode = _GenAssignCode(InLhsCode, InRhsCode);

//     //        _MergePreCodes(ilcode, InRhsCode);
//     //        _MergePreCodes(ilcode, InLhsCode);


//     //        _MergePostCodes(ilcode, InLhsCode);
//     //        _MergePostCodes(ilcode, InRhsCode);
//     //    }

//     //    public virtual IInstructionCode EmitBinOp(string InOpCode, IInstructionCode InLhsCode, IInstructionCode InRhsCode)
//     //    {
//     //        _GenBinOpCode(InOpCode, InLhsCode, InRhsCode);
//     //    }

//     //    public virtual IInstructionCode EmitUnaryOp(string InOpCode, IInstructionCode InRhsCode)
//     //    {
//     //        throw new NotImplementedException();
//     //    }

//     //    public virtual IInstructionCode EmitCall(IInstructionCode InSourceCode, IInstructionCode[] InParamCodes)
//     //    {
//     //        // Save Lhs to a temp-var.
//     //        InstCode result = new InstCode();
//     //        var tmpVar = result.RegTempVar();
//     //        _WriteCodeLine($"{tmpVar.Name} = {InFuncExprCode.Present}");

//     //        // Prepare call string
//     //        string callStr = $"{tmpVar.Name}(";

//     //        // Fill parameters
//     //        int count = 0;
//     //        foreach (var tuple in InParamCodes)
//     //        {
//     //            var name = tuple.Item1;
//     //            var usage = tuple.Item2;
//     //            var paramCode = tuple.Item3;

//     //            if (usage == ESyntaxTreeNodeUsage.Ref)
//     //            {
//     //                // paramCode is not support ref-call, Try handle set-back.
//     //                if (!paramCode.IsSupportRef
//     //                    && paramCode.IsSupportSet)
//     //                {
//     //                    throw new NotImplementedException();
//     //                }
//     //            }

//     //            if (count != 0)
//     //            { callStr += ", "; }

//     //            callStr += $", {paramCode.Present}";
//     //            ++count;
//     //        }

//     //        callStr += ")";

//     //        result.Present = callStr;

//     //        return result;
//     //    }

//     //    public IInstructionCode EmitNew(Info InArchetype, IInstructionCode[] InParamCodes)
//     //    {
//     //        throw new NotImplementedException();
//     //    }


//     //    private void _MergePreCodes(IInstructionCode InSubCode)
//     //    {
//     //        _PreCodeLines.AddRange(InSubCode._PreCodeLines)
//     //    }
//     //    private void _MergePostCodes(IInstructionCode InSubCode)
//     //    {
//     //        _PostCodeLines.AddRange(InSubCode._PostCodeLines)
//     //    }

//     //}



// }
