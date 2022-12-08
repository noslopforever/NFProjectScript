using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.cs
{

    class FunctionCode
    {
        public struct FuncParam
        {
            public FuncParam(string InType, string InName)
            {
                Typecode = InType;
                Name = InName;
                Default = "";
            }

            public string Typecode;

            public string Name;

            public string Default;

        }

        public string FuncName { get; set; } = "";

        public string FuncReturn { get; set; } = "";

        public List<FuncParam> FuncParams { get; } = new List<FuncParam>();

        public List<string> FuncBodyCodes { get; } = new List<string>();

        public bool FuncConst { get; internal set; }
    }

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

            // generate code for expressions.
            foreach (Info InInfo in InProj.SubInfos)
            {
                // skip delegate types.
                if (InInfo is DelegateTypeInfo)
                { continue; }

                // generate class from the model
                if (InInfo is TypeInfo)
                {
                    TryGenFunctionCodesForMethods(InInfo as TypeInfo);
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
                InInfo.ForeachSubInfo<MemberInfo>(info =>
                {
                    if (info.IsExtraContains("MemberDeclCodes"))
                    {
                        foreach (string decl in info.Extra.MemberDeclCodes)
                        { Console.WriteLine("    " + decl); }
                    }

                    if (info.IsExtraContains("MemberFunctions"))
                    {
                        foreach (FunctionCode func in info.Extra.MemberFunctions)
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
                    InInfo.ForeachSubInfo<MemberInfo>(info =>
                    {
                        if (info.IsExtraContains("MemberInitCodes"))
                        {
                            foreach (string init in info.Extra.MemberInitCodes)
                            {
                                string initExpCode = _GenCodeForExpr(InInfo, info.InitSyntax);
                                string replacedInitCode = init.Replace("$RHS", initExpCode);

                                Console.WriteLine("    " + replacedInitCode + ";");
                            }
                        }
                    });

                    Console.WriteLine("}");
                }

                Console.WriteLine($"");

                // ## function implements
                InInfo.ForeachSubInfo<MemberInfo>(info =>
                {
                    if (info.IsExtraContains("MemberFunctions"))
                    {

                        foreach (FunctionCode func in info.Extra.MemberFunctions)
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
        /// Generate code for a syntax-tree node.
        /// </summary>
        string _GenCodeForExpr(Info InContextInfo, ISyntaxTreeNode InSTNode)
        {
            STNodeAssign stnAssign = InSTNode as STNodeAssign;
            if (stnAssign != null)
            {
                // TODO use the member's 'SetExprCode' and pass the RHS's expr-code.
                return _GenCodeForExpr(InContextInfo, stnAssign.LHS) + " = " + _GenCodeForExpr(InContextInfo, stnAssign.RHS);
            }

            STNodeGetVar stnVarGet = InSTNode as STNodeGetVar;
            if (stnVarGet != null)
            {
                // Find the member with certain name in the context.
                // Then use the member's 'GetExprCode'.
                Info propInfo = InfoHelper.FindPropertyAlongScopeTree(InContextInfo, stnVarGet.IDName);
                if (propInfo != null)
                {
                    if (propInfo.IsExtraContains("MemberGetExprCode"))
                    {
                        return propInfo.Extra.MemberGetExprCode;
                    }
                    return "$ERR_PROP_EXTRA";
                }
                return "$ERR_PROP";
            }

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
                return _GenCodeForExpr(InContextInfo, stnBinOp.LHS) + " " + opcode + " " + _GenCodeForExpr(InContextInfo, stnBinOp.RHS);
            }

            STNodeConstant stnConst = InSTNode as STNodeConstant;
            if (stnConst != null)
            {
                return stnConst.ValueString;
            }

            return "$ERR";
        }

        /// <summary>
        /// Write function declaration.
        /// </summary>
        /// <param name="InClassName"></param>
        /// <param name="InFunc"></param>
        /// <param name="Output"></param>
        /// <param name="InDecl"></param>
        private static void WriteFunctionDecl(string InClassName, FunctionCode InFunc, TextWriter Output, bool InDecl)
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
            InInfo.ForeachSubInfo<MemberInfo>(info =>
            {
                if (info is MethodInfo)
                {
                    TryProcessInfoAsFunction(InInfo, info);
                }
                // Handle common TypeInfo
                else
                {
                    TryProcessInfoAsProperty(InInfo, info);
                }
            });

        }

        /// <summary>
        /// Try gather enough informations of a function.
        /// </summary>
        /// <param name="inInfo"></param>
        /// <param name="info"></param>
        private void TryProcessInfoAsFunction(TypeInfo InParentTypeInfo, MemberInfo InInfo)
        {
            // # Exact signatures(name, parameters) of the function, but DO NOT translate any expression in the function body.
            DelegateTypeInfo delegateType = InInfo.Archetype as DelegateTypeInfo;
            System.Diagnostics.Debug.Assert(delegateType != null);

            // Exact the return-param and in/out parameters.
            MemberInfo returnMember = null;
            List<MemberInfo> inOutMembers = new List<MemberInfo>();
            delegateType.ForeachSubInfo<MemberInfo>(member =>
            {
                if (member.HasSubInfoWithHeader<AttributeInfo>("Return"))
                { returnMember = member; }
                else
                { inOutMembers.Add(member); }
            });

            InInfo.Extra.MethodReturnMember = returnMember;
            InInfo.Extra.MethodReturnType = null;
            InInfo.Extra.MethodReturnTypeCode = "void";
            if (returnMember != null)
            {
                InInfo.Extra.MethodReturnType = returnMember.Archetype;
                InInfo.Extra.MethodReturnTypeCode = _ExactCppTypeCodeFromInfo(returnMember.Archetype);
            }

            // generate C++ function decl by delegateType
            InInfo.Extra.cppFuncCode = new FunctionCode()
            {
                FuncName = $"{InInfo.Name}",
                FuncReturn = InInfo.Extra.MethodReturnTypeCode,
                FuncConst = false,
            };
            // add parameters
            foreach (MemberInfo param in inOutMembers)
            {
                string typeCode = _ExactCppTypeCodeFromInfo(param.Archetype);
                InInfo.Extra.cppFuncCode.FuncParams.Add(new FunctionCode.FuncParam($"{typeCode}", $"{param.Name}"));
            }

            // Only declarations and signatures.
            // The translation of the function body should start after all infos in a project had been parsed.
        }

        /// <summary>
        /// Try gather enough informations of a property.
        /// </summary>
        /// <param name="InInfo"></param>
        private void TryProcessInfoAsProperty(TypeInfo InParentTypeInfo, MemberInfo InInfo)
        {
            // Generate different prop codes by info's type and attributes.
            string typeCode = _ExactCppTypeCodeFromInfo(InInfo.Archetype);

            // - if : Attribute access:
            bool isAttribute = InInfo.HasSubInfoWithHeader<AttributeInfo>("Property");
            if (!isAttribute)
            {
                // Common property access codes:
                InInfo.Extra.MemberTypeCode = typeCode;
                InInfo.Extra.MemberName = InInfo.Name;

                InInfo.Extra.MemberInitCodes = new string[1] { $"{InInfo.Name} = $RHS" };
                InInfo.Extra.MemberSetExprCode = $"{InInfo.Name} = $RHS";
                InInfo.Extra.MemberGetExprCode = $"{InInfo.Name}";
                InInfo.Extra.MemberRefToSetCode = $"{InInfo.Name}";

                InInfo.Extra.MemberDeclCodes = new List<string>();
                InInfo.Extra.MemberDeclCodes.Add($"{typeCode} {InInfo.Name};");
            }
            else
            {
                InInfo.Extra.MemberFunctions = new List<FunctionCode>();

                // construct a setter function
                FunctionCode setter = new FunctionCode()
                {
                    FuncName = $"set{InInfo.Name}",
                    FuncReturn = "void",
                    FuncConst = false,
                };
                setter.FuncParams.Add(new FunctionCode.FuncParam($"{typeCode}", "rhs"));
                setter.FuncBodyCodes.Add("// ??? ");
                InInfo.Extra.MemberFunctions.Add(setter);

                // construct a getter function.
                FunctionCode getter = new FunctionCode()
                {
                    FuncName = $"get{InInfo.Name}",
                    FuncReturn = $"{typeCode}",
                    FuncConst = true,
                };
                getter.FuncBodyCodes.Add("// ???");
                getter.FuncBodyCodes.Add($"return {typeCode}(0);");
                InInfo.Extra.MemberFunctions.Add(getter);

                // special Member init/set/get/ref codes
                InInfo.Extra.MemberInitCodes = new string[1] { $"{InInfo.Name} = $RHS" };
                InInfo.Extra.MemberSetExprCode = $"set{InInfo.Name}($RHS)";
                InInfo.Extra.MemberGetExprCode = $"get{InInfo.Name}()";
                InInfo.Extra.MemberRefToSetCode = $"{InInfo.Name}";
            }

        }

        /// <summary>
        /// Try generate expression codes.
        /// </summary>
        /// <param name="InInfo"></param>
        private void TryGenFunctionCodesForMethods(TypeInfo InInfo)
        {
            // Handle methods
            InInfo.ForeachSubInfo<MethodInfo>(methodInfo =>
            {
                // generate C++ function codes
                STNodeSequence exprSeq = methodInfo.InitSyntax as STNodeSequence;
                System.Diagnostics.Debug.Assert(exprSeq != null);

                var returnMember = methodInfo.Extra.MethodReturnMember;
                var retTypeCode = methodInfo.Extra.MethodReturnTypeCode;

                // prepare return
                if (returnMember != null)
                { methodInfo.Extra.cppFuncCode.FuncBodyCodes.Add($"{retTypeCode} {returnMember.Name};"); }

                // generate c++ codes per ST line.
                foreach (ISyntaxTreeNode stnode in exprSeq.NodeList)
                {
                    string code = _GenCodeForExpr(methodInfo, stnode);
                    methodInfo.Extra.cppFuncCode.FuncBodyCodes.Add(code + ";");
                }

                // finish return
                if (returnMember != null)
                { methodInfo.Extra.cppFuncCode.FuncBodyCodes.Add($"return {returnMember.Name};"); }

                methodInfo.Extra.MemberFunctions = new List<FunctionCode>();
                methodInfo.Extra.MemberFunctions.Add(methodInfo.Extra.cppFuncCode);
            });
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
