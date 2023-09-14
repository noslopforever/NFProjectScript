// using nf.protoscript.syntaxtree;
// using System;
// using System.Collections.Generic;

// namespace nf.protoscript.translator.expression
// {
//     /// <summary>
//     /// Code template for a sector.
//     /// </summary>
//     public class DomainBasedCodeTemplate
//     {
//         public DomainBasedCodeTemplate() { }

//         public DomainBasedCodeTemplate(IEnumerable<SnippetBase> snippets)
//         {
//             foreach (var snippet in snippets)
//             {
//                 AddSnippet(snippet);
//             }
//         }

//         public abstract class SnippetBase
//         {
//             /// <summary>
//             /// Indent of the snippet. Each line of the snippet should be indent by this number.
//             /// </summary>
//             public int Indent { get; set; } = 0;

//             public abstract string[] GenerateCode(ISyntaxTreeNode InSTNode);

//         }

//         public class SnippetConst
//             : SnippetBase
//         {
//             public SnippetConst(string InCodes)
//             {
//                 Strings = new string[] { InCodes };
//             }

//             public SnippetConst(IEnumerable<string> InCodes)
//             {
//                 List<string> tempCodes = new List<string>(InCodes);
//                 Strings = tempCodes.ToArray();
//             }

//             public string[] Strings { get; set; }

//             public override string[] GenerateCode(ISyntaxTreeNode InSTNode)
//             {
//                 return Strings;
//             }
//         }

//         public class SnippetDomain
//             : SnippetBase
//         {
//             public SnippetDomain(string InDomainName)
//             {
//                 DomainName = InDomainName;
//             }

//             public string DomainName { get; set; }
//             public override string[] GenerateCode(ISyntaxTreeNode InSTNode)
//             {
//                 throw new NotImplementedException();
//             }
//         }

//         public class SnippetPresent
//             : SnippetBase
//         {
//             public override string[] GenerateCode(ISyntaxTreeNode InSTNode)
//             {
//                 throw new NotImplementedException();
//             }
//         }

//         public class SnippetConditionalDomain
//             : SnippetBase
//         {
//             public string DomainName { get; set; }
//             public override string[] GenerateCode(ISyntaxTreeNode InSTNode)
//             {
//                 throw new NotImplementedException();
//             }
//         }

//         // Snippets
//         List<SnippetBase> _snippets = new List<SnippetBase>();

//         /// <summary>
//         /// Add snippet to this template.
//         /// </summary>
//         /// <param name="InSnippet"></param>
//         public void AddSnippet(SnippetBase InSnippet)
//         {
//             _snippets.Add(InSnippet);
//         }

//         /// <summary>
//         /// Generate code for .
//         /// </summary>
//         /// <param name="InSTNode"></param>
//         /// <returns></returns>
//         public string GenerateCode(ISyntaxTreeNode InSTNode)
//         {
//             string codes = "";
//             for (int i = 0; i < _snippets.Count; i++)
//             {
//                 codes += _snippets[i].GenerateCode(InSTNode);
//             }
//             return codes;
//         }

//         /// <summary>
//         /// Add snippet when matches the InCondition.
//         /// </summary>
//         /// <param name="InCondition"></param>
//         /// <param name="InTrueSnippet"></param>
//         public void AddConditionalSnippet(Func<object, bool> InCondition, SnippetBase InTrueSnippet)
//         {
//             AddConditionalSnippets(InCondition, new SnippetBase[] { InTrueSnippet }, new SnippetBase[] { });
//         }

//         /// <summary>
//         /// Add True snippets when matches the InCondition.
//         /// </summary>
//         /// <param name="InCondition"></param>
//         /// <param name="InTrueSnippets"></param>
//         public void AddConditionalSnippets(Func<object, bool> InCondition, SnippetBase[] InTrueSnippets)
//         {
//             AddConditionalSnippets(InCondition, InTrueSnippets);
//         }

//         /// <summary>
//         /// Add True snippets when matches the InCondition, or add the False snippet when not.
//         /// </summary>
//         /// <param name="InCondition"></param>
//         /// <param name="InTrueSnippets"></param>
//         /// <param name="InFalseSnippet"></param>
//         public void AddConditionalSnippets(Func<object, bool> InCondition, SnippetBase[] InTrueSnippets, SnippetBase InFalseSnippet)
//         {
//             AddConditionalSnippets(InCondition, InTrueSnippets, new SnippetBase[] { InFalseSnippet });
//         }

//         /// <summary>
//         /// Add True snippets when matches the InCondition, or add False snippets when not.
//         /// </summary>
//         /// <param name="InCondition"></param>
//         /// <param name="InTrueSnippets"></param>
//         /// <param name="InFalseSnippets"></param>
//         public void AddConditionalSnippets(Func<object, bool> InCondition, SnippetBase[] InTrueSnippets, SnippetBase[] InFalseSnippets)
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
