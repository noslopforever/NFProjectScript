using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.test
{


    /// 
    /// unit Ship
    ///     -n level @max:4
    /// 
    /// unit Asteroid
    ///     -n level @max:3
    /// 
    /// fakebullet Bullet
    ///     - trail 
    /// 
    /// unit Rocket
    /// 

    class TestCppTranslator
    {
        /// <summary>
        /// Translate a project.
        /// </summary>
        /// <param name="InProj"></param>
        public void Translate(ProjectInfo InProj)
        {
            // gather translation informations.
            foreach (Info InInfo in InProj.SubInfos)
            {
                // generate class from the model
                if (InInfo is TypeInfo)
                {
                    TryProcessInfoAsClass(InInfo as TypeInfo);
                }
            }

            // Generate code for expressions.
            foreach (Info InInfo in InProj.SubInfos)
            {
                // Skip delegate types.
                if (InInfo is DelegateTypeInfo)
                { continue; }

                // Generate class from the model
                if (InInfo is TypeInfo)
                {
                    // Create a temporary cpp-function for the 'ctor' method.
                    // But don't register it into MemberFunctions, because the ctor is always different with MemberFunctions.
                    CppFunction ctorFunc = new CppFunction(InInfo)
                    {
                        FuncName = "__ctor",
                    };

                    // Gen codes for member-inits
                    InInfo.ForeachSubInfo<ElementInfo>(elemInfo =>
                    {
                        if (elemInfo.Header == "property")
                        {
                            // TODO fix bad smell.
                            IList<string> initCodeList = _GenCodeForExpr(ctorFunc, elemInfo.InitSyntax);
                            elemInfo.Extra.MemberInitExprCode = initCodeList[0];
                        }
                        // Gen codes for methods.
                        else if (elemInfo.Header == "method")
                        {
                            TryGenExprCodesForMethod(InInfo, elemInfo);
                        }
                    }
                    );

                }
            }

            // do translating after all informations gathered.
            foreach (Info InInfo in InProj.SubInfos)
            {
                // skip delegate types.
                if (InInfo is DelegateTypeInfo)
                { continue; }

                // generate class from the model
                if (InInfo is TypeInfo)
                {
                    // # generate header files:
                    _GenerateHeaderFile(InInfo);

                    // # generate cpp files:
                    _GenerateCppFile(InInfo);

                }
            }
        }

        /// <summary>
        /// Generate header file.
        /// </summary>
        /// <param name="InInfo"></param>
        private void _GenerateHeaderFile(Info InInfo)
        {
            Console.WriteLine($">>>> HEADER : {InInfo.Extra.HeaderFilename}");
            {

                Console.WriteLine($"class {InInfo.Extra.ClassFullname}");
                if (InInfo.Extra.ClassBase != "")
                    Console.WriteLine($"    : public {InInfo.Extra.BaseClass}");
                Console.WriteLine("{");
                Console.WriteLine("    GENERATED_BODY()");
                Console.WriteLine("public:");

                // Handle members
                InInfo.ForeachSubInfo<Info>(info =>
                {
                    if (info.IsExtraContains("MemberDeclCodes"))
                    {
                        foreach (string decl in info.Extra.MemberDeclCodes)
                        { Console.WriteLine("    " + decl); }
                    }

                    if (info.IsExtraContains("MemberFunctions"))
                    {
                        foreach (CppFunction func in info.Extra.MemberFunctions)
                        {
                            Console.Write("    ");
                            WriteFunctionDecl(InInfo.Extra.ClassFullname, func, Console.Out, true);
                            Console.WriteLine("");
                        }
                    }
                });

                Console.WriteLine("};");
            }
        }

        /// <summary>
        /// Generate cpp file.
        /// </summary>
        /// <param name="InInfo"></param>
        private void _GenerateCppFile(Info InInfo)
        {
            Console.WriteLine($">>>> CPP : {InInfo.Extra.CppFilename}");
            {
                Console.WriteLine($"#include \"InInfo.Extra.HeaderFilename\"");
                Console.WriteLine($"");

                // ## c-tors
                {
                    Console.WriteLine($"{InInfo.Extra.ClassFullname}::{InInfo.Extra.ClassFullname}()");
                    Console.WriteLine("{");

                    // ### member-initializations.
                    InInfo.ForeachSubInfo<Info>(info =>
                    {
                        if (info.IsExtraContains("MemberInitCodes")
                            && info.IsExtraContains("MemberInitExprCode"))
                        {
                            foreach (string init in info.Extra.MemberInitCodes)
                            {
                                string replacedInitCode = init.Replace("$RHS", info.Extra.MemberInitExprCode);
                                Console.WriteLine("    " + replacedInitCode + ";");
                            }
                        }
                    });

                    Console.WriteLine("}");
                }

                Console.WriteLine($"");

                // ## function implements
                InInfo.ForeachSubInfo<Info>(info =>
                {
                    if (info.IsExtraContains("MemberFunctions"))
                    {
                        foreach (CppFunction func in info.Extra.MemberFunctions)
                        {
                            WriteFunctionDecl(InInfo.Extra.ClassFullname, func, Console.Out, false);
                            Console.WriteLine("");
                            Console.WriteLine("{");
                            foreach (string code in func.FuncBodyCodes)
                            {
                                Console.WriteLine($"    {code}");
                            }
                            Console.WriteLine("}");
                        }
                    }
                });

            }
        }

        /// <summary>
        /// Write function declaration.
        /// </summary>
        /// <param name="InClassName"></param>
        /// <param name="InFunc"></param>
        /// <param name="Output"></param>
        /// <param name="InDecl"></param>
        private static void WriteFunctionDecl(string InClassName, CppFunction InFunc, TextWriter Output, bool InDecl)
        {
            Output.Write($"{InFunc.FuncReturn} ");

            if (!InDecl)
            { Output.Write($"{InClassName}::"); }

            Output.Write($"{InFunc.FuncName}(");

            for (int i = 0; i < InFunc.FuncParams.Count; i++)
            {
                var param = InFunc.FuncParams[i];
                if (i != 0)
                { Output.Write(", "); }

                Output.Write($"{param.Typecode} {param.Name}");

                if (param.Default != "")
                {
                    if (!InDecl)
                    { Output.Write("/*"); }

                    Output.Write($" = {param.Default}");

                    if (!InDecl)
                    { Output.Write("*/"); }
                }
            }

            Output.Write($")");

            if (InFunc.FuncConst)
            { Output.Write(" const"); }

            if (InDecl)
            { Output.Write($";"); }
        }

        /// <summary>
        /// Process a TypeInfo to gather enough informations for translating.
        /// </summary>
        /// <param name="InInfo"></param>
        private void TryProcessInfoAsClass(TypeInfo InInfo)
        {
            // set different base-classes by Header
            if (InInfo.Header == "unit")
            {
                InInfo.Extra.ClassNamePrefix = "A";
                InInfo.Extra.ClassName = InInfo.Name;
                InInfo.Extra.ClassFullname = InInfo.Extra.ClassNamePrefix + InInfo.Name;
                InInfo.Extra.ClassBase = "APawn";
                InInfo.Extra.HeaderFilename = InInfo.Name;
                InInfo.Extra.CppFilename = InInfo.Name;
            }
            else
            {
                InInfo.Extra.ClassNamePrefix = "C";
                InInfo.Extra.ClassName = InInfo.Name;
                InInfo.Extra.ClassFullname = InInfo.Extra.ClassNamePrefix + InInfo.Name;
                InInfo.Extra.ClassBase = "";
                InInfo.Extra.HeaderFilename = InInfo.Name;
                InInfo.Extra.CppFilename = InInfo.Name;
            }

            // Handle members
            InInfo.ForeachSubInfo<ElementInfo>(elemInfo =>
                {
                    TryProcessInfoAsProperty(InInfo, elemInfo);
                }
                , elemInfo => elemInfo.Header == "property"
                );

            // Handle methods
            InInfo.ForeachSubInfo<ElementInfo>(elemInfo =>
                {
                    TryProcessInfoAsFunction(InInfo, elemInfo);
                }
                , elemInfo => elemInfo.Header == "method"
                );

            // TODO handle states, graphs, events

        }

        /// <summary>
        /// Try gather enough informations of a function.
        /// </summary>
        /// <param name="inInfo"></param>
        /// <param name="info"></param>
        private void TryProcessInfoAsFunction(TypeInfo InParentTypeInfo, ElementInfo InMethodInfo)
        {
            // # Exact signatures(name, parameters) of the function, but DO NOT translate any expression in the function body.
            DelegateTypeInfo delegateType = InMethodInfo.ElementType as DelegateTypeInfo;
            System.Diagnostics.Debug.Assert(delegateType != null);

            // Exact the return-param and in/out parameters.
            ElementInfo returnMember = null;
            List<ElementInfo> inOutMembers = new List<ElementInfo>();
            delegateType.ForeachSubInfo<ElementInfo>(elem =>
            {
                // parameter-elements must be the param.
                System.Diagnostics.Debug.Assert(elem.Header == "param");

                if (elem.HasSubInfoWithHeader<AttributeInfo>("Return"))
                { returnMember = elem; }
                else
                { inOutMembers.Add(elem); }
            });

            InMethodInfo.Extra.MethodReturnMember = returnMember;
            InMethodInfo.Extra.MethodReturnType = null;
            InMethodInfo.Extra.MethodReturnTypeCode = "void";
            if (returnMember != null)
            {
                InMethodInfo.Extra.MethodReturnType = returnMember.ElementType;
                InMethodInfo.Extra.MethodReturnTypeCode = _ExactCppTypeCodeFromInfo(returnMember.ElementType);
            }

            // generate C++ function decl by delegateType
            InMethodInfo.Extra.cppFuncCode = new CppFunction(InMethodInfo)
            {
                FuncName = $"{InMethodInfo.Name}",
                FuncReturn = InMethodInfo.Extra.MethodReturnTypeCode,
                FuncConst = false,
            };
            // add parameters
            foreach (ElementInfo param in inOutMembers)
            {
                string typeCode = _ExactCppTypeCodeFromInfo(param.ElementType);
                InMethodInfo.Extra.cppFuncCode.FuncParams.Add(new CppFunction.FuncParam($"{typeCode}", $"{param.Name}"));
            }

            // Only declarations and signatures.
            // The translation of the function body should start after all infos in a project had been parsed.
        }

        /// <summary>
        /// Try gather enough informations of a property.
        /// </summary>
        /// <param name="InInfo"></param>
        private void TryProcessInfoAsProperty(TypeInfo InParentTypeInfo, ElementInfo InInfo)
        {
            // Generate different prop codes by info's type and attributes.
            string typeCode = _ExactCppTypeCodeFromInfo(InInfo.ElementType);

            InInfo.Extra.MemberTypeCode = typeCode;
            InInfo.Extra.MemberName = InInfo.Name;

            // - if : Attribute access:
            bool isAttribute = InInfo.HasSubInfoWithHeader<AttributeInfo>("Property");
            if (!isAttribute)
            {
                InInfo.Extra.MemberDirectAccess = true;
                InInfo.Extra.MemberIndirectAccess = false;

                // Common property access codes:
                InInfo.Extra.MemberInitCodes = new string[1] { $"{InInfo.Name} = $RHS" };
                InInfo.Extra.MemberSetExprCode = $"{InInfo.Name} = $RHS";
                InInfo.Extra.MemberGetExprCode = $"{InInfo.Name}";
                InInfo.Extra.MemberRefExprCode = $"{InInfo.Name}";

                // Declare it directly.
                InInfo.Extra.MemberDeclCodes = new List<string>();
                InInfo.Extra.MemberDeclCodes.Add($"{typeCode} {InInfo.Name};");
            }
            else
            {
                InInfo.Extra.MemberDirectAccess = false;
                InInfo.Extra.MemberIndirectAccess = true;

                // special Member init/set/get/ref codes
                InInfo.Extra.MemberInitCodes = new string[1] { $"{InInfo.Name} = $RHS" };
                InInfo.Extra.MemberSetExprCode = $"set{InInfo.Name}($RHS)";
                InInfo.Extra.MemberGetExprCode = $"get{InInfo.Name}()";
                InInfo.Extra.MemberRefExprCode = null;

                // getter/setter functions.
                InInfo.Extra.MemberFunctions = new List<CppFunction>();

                // construct a setter function
                CppFunction setter = new CppFunction(InParentTypeInfo)
                {
                    FuncName = $"set{InInfo.Name}",
                    FuncReturn = "void",
                    FuncConst = false,
                };
                setter.FuncParams.Add(new CppFunction.FuncParam($"{typeCode}", "rhs"));
                setter.FuncBodyCodes.Add("// ??? ");
                InInfo.Extra.MemberFunctions.Add(setter);

                // construct a getter function.
                CppFunction getter = new CppFunction(InParentTypeInfo)
                {
                    FuncName = $"get{InInfo.Name}",
                    FuncReturn = $"{typeCode}",
                    FuncConst = true,
                };
                getter.FuncBodyCodes.Add("// ???");
                getter.FuncBodyCodes.Add($"return {typeCode}(0);");
                InInfo.Extra.MemberFunctions.Add(getter);
            }

        }


        /// <summary>
        /// Exact STNode to cpp instructions.
        /// </summary>
        /// <param name="InCodelist"></param>
        /// <param name="InFunction"></param>
        /// <param name="InSTNode"></param>
        CppILInstruction _ExactInstructions(CppFunction InFunction, ISyntaxTreeNode InSTNode)
        {
            STNodeCall stnCall = InSTNode as STNodeCall;
            if (stnCall != null)
            {
                // TODO ref parameters.
                List<CppILInstruction> paramInsts = new List<CppILInstruction>();
                foreach (var param in stnCall.Params)
                {
                    paramInsts.Add(_ExactInstructions(InFunction, param));
                }
                var inst = new CppILInstruction_Call(InFunction, stnCall.FuncName, paramInsts.ToArray());
                return inst;
            }
            STNodeNew stnNew = InSTNode as STNodeNew;
            if (stnNew != null)
            {
                List<CppILInstruction> paramInsts = new List<CppILInstruction>();
                foreach (var param in stnNew.Params)
                {
                    paramInsts.Add(_ExactInstructions(InFunction, param));
                }
                var inst = new CppILInstruction_Call(InFunction, $"new {stnNew.Typename}", paramInsts.ToArray());
                return inst;
            }


            // Assign => binop'='
            STNodeAssign stnAssign = InSTNode as STNodeAssign;
            if (stnAssign != null)
            {
                // Exact Instructions of sub ST tree
                var instLhs = _ExactInstructions(InFunction, stnAssign.LHS);
                var instRhs = _ExactInstructions(InFunction, stnAssign.RHS);

                var inst = new CppILInstruction_Assign(InFunction, instLhs, instRhs);

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
                var inst = new CppILInstruction_BinaryOp(
                    InFunction
                    , $"{opcode}"
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
                    if (propInfo.IsExtraContains("MemberIndirectAccess") && propInfo.Extra.MemberIndirectAccess)
                    {
                        // indirect property, but with ref-access code like int& refFoo()
                        if (propInfo.IsExtraContains("MemberRefExprCode")
                            && propInfo.Extra.MemberRefExprCode != null)
                        {
                            return new CppILInstruction_Var(InFunction)
                            {
                                AccessType = CppILInstruction_Var.EAccessType.Ref,
                                Constant = false,
                                RefCode = propInfo.Extra.MemberRefExprCode,
                                GetCode = propInfo.Extra.MemberGetExprCode,
                                SetCode = propInfo.Extra.MemberSetExprCode,
                            };
                        }
                        // indirect property, with only getter or getter/setter.
                        // Can be only accessed by value.
                        else
                        {
                            return new CppILInstruction_Var(InFunction)
                            {
                                AccessType = CppILInstruction_Var.EAccessType.Value,
                                Constant = false,
                                RefCode = null,
                                GetCode = propInfo.Extra.MemberGetExprCode,
                                SetCode = propInfo.Extra.MemberSetExprCode,
                            };
                        }
                    }
                }

                // direct property/local/global/member, ref it directly.
                return new CppILInstruction_Var(InFunction)
                {
                    AccessType = CppILInstruction_Var.EAccessType.Ref,
                    Constant = false,
                    RefCode = stnVarGet.IDName,
                    GetCode = stnVarGet.IDName,
                    SetCode = $"{stnVarGet.IDName} = $RHS",
                };
            }

            // Const => constant.
            STNodeConstant stnConst = InSTNode as STNodeConstant;
            if (stnConst != null)
            {
                string constString = _GetConstString(stnConst);

                var inst = new CppILInstruction_Var(InFunction)
                {
                    AccessType = CppILInstruction_Var.EAccessType.Value,
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
        /// Generate code for a syntax-tree node.
        /// </summary>
        IList<String> _GenCodeForExpr(CppFunction InFunction, ISyntaxTreeNode InSTNode)
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

            //STNodeAssign stnAssign = InSTNode as STNodeAssign;
            //if (stnAssign != null)
            //{
            //    // use the member's 'SetExprCode' and pass the RHS's expr-code.
            //    string lhsCode = _GenCodeForExpr(InFunction, stnAssign.LHS);
            //    string rhsCode = _GenCodeForExpr(InFunction, stnAssign.RHS);
            //    return lhsCode.Replace("$RHS", rhsCode);
            //}

            //STNodeGetVar stnVarGet = InSTNode as STNodeGetVar;
            //if (stnVarGet != null)
            //{
            //    Info propInfo = InfoHelper.FindPropertyAlongScopeTree(InContextInfo, stnVarGet.IDName);
            //    if (propInfo != null)
            //    {
            //        if (!stnVarGet.LeftHandValue
            //            && propInfo.IsExtraContains("MemberGetExprCode")
            //            )
            //        {
            //            return propInfo.Extra.MemberGetExprCode;
            //        }
            //        if (stnVarGet.LeftHandValue
            //            && propInfo.IsExtraContains("MemberSetExprCode"))
            //        {
            //            return propInfo.Extra.MemberSetExprCode;
            //        }
            //        return "$ERR_PROP_EXTRA";
            //    }
            //    return "$ERR_PROP";
            //}

            //STNodeBinaryOp stnBinOp = InSTNode as STNodeBinaryOp;
            //if (stnBinOp != null)
            //{
            //    string opcode = "$ERR";
            //    switch (stnBinOp.OpCode)
            //    {
            //        case STNodeBinaryOp.Def.Add: opcode = "+"; break;
            //        case STNodeBinaryOp.Def.Sub: opcode = "-"; break;
            //        case STNodeBinaryOp.Def.Mul: opcode = "*"; break;
            //        case STNodeBinaryOp.Def.Div: opcode = "/"; break;
            //        case STNodeBinaryOp.Def.Mod: opcode = "%"; break;
            //    }
            //    return _GenCodeForExpr(InFunction, stnBinOp.LHS) + " " + opcode + " " + _GenCodeForExpr(InFunction, stnBinOp.RHS);
            //}

            //STNodeConstant stnConst = InSTNode as STNodeConstant;
            //if (stnConst != null)
            //{
            //    return stnConst.ValueString;
            //}

            //return "$ERR";
        }

        /// <summary>
        /// Try generate expression codes for method.
        /// </summary>
        /// <param name="InInfo"></param>
        void TryGenExprCodesForMethod(Info InInfo, ElementInfo InMethodInfo)
        {
            // generate C++ function codes
            STNodeSequence exprSeq = InMethodInfo.InitSyntax as STNodeSequence;
            System.Diagnostics.Debug.Assert(exprSeq != null);

            var returnMember = InMethodInfo.Extra.MethodReturnMember;
            var retTypeCode = InMethodInfo.Extra.MethodReturnTypeCode;

            // prepare return
            CppFunction cppFunc = InMethodInfo.Extra.cppFuncCode;
            if (returnMember != null)
            {
                cppFunc.TryRegTempVar($"{returnMember.Name}", $"{retTypeCode}");
            }

            // generate c++ codes per ST line.
            foreach (ISyntaxTreeNode stnode in exprSeq.NodeList)
            {
                IList<string> codeList = _GenCodeForExpr(cppFunc, stnode);
                foreach (string code in codeList)
                {
                    cppFunc.FuncBodyCodes.Add(code + ";");
                }
            }

            // finish return
            if (returnMember != null)
            {
                cppFunc.FuncBodyCodes.Add($"return {returnMember.Name};");
            }

            InMethodInfo.Extra.MemberFunctions = new List<CppFunction>();
            InMethodInfo.Extra.MemberFunctions.Add(cppFunc);
        }

        private static string _ExactCppTypeCodeFromInfo(TypeInfo InArchetype)
        {
            string typeCode = $"{InArchetype.Name}";
            if (CommonTypeInfos.IsInteger32Type(InArchetype))
            {
                typeCode = "int32";
            }
            else if (CommonTypeInfos.IsStringType(InArchetype))
            {
                typeCode = "FString";
            }

            return typeCode;
        }
    }

}
