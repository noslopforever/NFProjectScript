﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        public void Translate(ProjectInfo InProjProj)
        {
            // gather translation informations.
            foreach (Info InInfo in InProjProj.SubInfos)
            {
                // generate class from the model
                if (InInfo is TypeInfo)
                {
                    TryProcessInfoAsClass(InInfo as TypeInfo);
                }
            }

            // do translating after all informations gathered.
            foreach (Info InInfo in InProjProj.SubInfos)
            {
                // generate class from the model
                if (InInfo is TypeInfo)
                {
                    // generate header files:
                    Console.WriteLine($">>>> HEADER : {InInfo.Extra.HeaderFilename}");
                    {

                        Console.WriteLine($"    class {InInfo.Extra.ClassFullname}");
                        if (InInfo.Extra.ClassBase != "")
                            Console.WriteLine($"        : public {InInfo.Extra.BaseClass}");
                        Console.WriteLine("    {");
                        Console.WriteLine("        GENERATED_BODY()");
                        Console.WriteLine("    public:");

                        // Handle members
                        InInfo.ForeachSubInfo<MemberInfo>(info =>
                        {
                            if (info.IsExtraContains("MemberDeclCodes"))
                            {
                                foreach (string decl in info.Extra.MemberDeclCodes)
                                { Console.WriteLine("        " + decl); }
                            }

                            if (info.IsExtraContains("MemberFunctions"))
                            {
                                foreach (FunctionCode func in info.Extra.MemberFunctions)
                                {
                                    Console.Write("        ");
                                    WriteFunctionDecl(InInfo.Extra.ClassFullname, func, Console.Out, true);
                                    Console.WriteLine("");
                                }
                            }
                        });

                        Console.WriteLine("    };");
                    }

                    // generate cpp files:
                    Console.WriteLine($">>>> CPP : {InInfo.Extra.CppFilename}");
                    {
                        Console.WriteLine($"#include \"InInfo.Extra.HeaderFilename\"");
                        Console.WriteLine($"");
                        Console.WriteLine($"{InInfo.Extra.ClassFullname}::{InInfo.Extra.ClassFullname}()");
                        Console.WriteLine("{");

                        // Handle members
                        InInfo.ForeachSubInfo<MemberInfo>(info =>
                        {
                            if (info.IsExtraContains("MemberInitCodes"))
                            {
                                foreach (string init in info.Extra.MemberInitCodes)
                                { Console.WriteLine("    " + init); }
                            }
                        });

                        Console.WriteLine("}");
    
                        Console.WriteLine($"");

                        // function implements
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
            }


        }

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
                if (info.Archetype is FunctionInfo)
                {
                    throw new NotImplementedException();
                    //TryProcessInfoAsFunction(info);
                }
                // Handle common TypeInfo
                else
                {
                    TryProcessInfoAsProperty(InInfo, info);
                }
            });

        }

        /// <summary>
        /// Try process a sub-info of a type to gather enough informations for translating.
        /// </summary>
        /// <param name="InInfo"></param>
        private void TryProcessInfoAsProperty(TypeInfo InParentTypeInfo, MemberInfo InInfo)
        {
            // Generate different prop codes by info's type and attributes.
            string typeCode = $"{InInfo.Archetype.Name}";
            if (CommonTypeInfos.IsInteger32Type(InInfo.Archetype))
            {
                typeCode = "int32";
            }
            else if (CommonTypeInfos.IsStringType(InInfo.Archetype))
            {
                typeCode = "FString";
            }

            // Common prop access codes:
            {
                InInfo.Extra.MemberTypeCode = typeCode;
                InInfo.Extra.MemberName = InInfo.Name;
                InInfo.Extra.MemberInitCode = $"{InInfo.Name} = $RHS";
                InInfo.Extra.MemberGetCode = $"{InInfo.Name}";
                InInfo.Extra.MemberRefToSetCode = $"{InInfo.Name}";
                InInfo.Extra.MemberCallerCode = $"{InInfo.Name}.$RHS";
                InInfo.Extra.MemberAccessCode = $"{InInfo.Name}.$RHS";

                // - if : Attribute access:
                //InInfo.HasSubInfoWithName<AttributeInfo>("AsProperty");
                bool isAttribute = false;
                if (isAttribute)
                {
                    InInfo.Extra.MemberFunctions = new List<FunctionCode>();

                    FunctionCode setter = new FunctionCode()
                    {
                        FuncName = $"set{InInfo.Name}",
                        FuncReturn = "void",
                        FuncConst = false,
                    };
                    setter.FuncParams.Add(new FunctionCode.FuncParam($"{typeCode}", "rhs"));
                    setter.FuncBodyCodes.Add("// ??? ");
                    InInfo.Extra.MemberFunctions.Add(setter);

                    FunctionCode getter = new FunctionCode()
                    {
                        FuncName = $"get{InInfo.Name}",
                        FuncReturn = $"{typeCode}",
                        FuncConst = true,
                    };
                    getter.FuncBodyCodes.Add("// ???");
                    getter.FuncBodyCodes.Add($"return {typeCode}(0);");
                    InInfo.Extra.MemberFunctions.Add(getter);
                }
                else
                {
                    InInfo.Extra.MemberDeclCodes = new List<string>();
                    InInfo.Extra.MemberDeclCodes.Add($"{typeCode} {InInfo.Name};");
                }

                //InInfo.Extra["PropSetCode"] = $"set{InInfo.Name}($RHS)";
                //InInfo.Extra["PropGetCode"] = $"get{InInfo.Name}()";

                //InInfo.Extra["PropRefToSetCode"] = $"ref{InInfo.Name}()";
                // or
                //InInfo.Extra["PropRefToSetCode"] = $"$TEMP = set{InInfo.Name}()";
                //InInfo.Extra["PropRefToSetCode"] = $"$TEMP = $RHS";
                //InInfo.Extra["PropRefToSetCode"] = $"set{InInfo.Name}($TEMP)";
            }


        }

    }
}
