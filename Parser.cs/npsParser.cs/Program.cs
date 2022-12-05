using System;
using System.IO;
using System.Collections.Generic;
using CommandLine;
using System.Diagnostics;
using nf.protoscript;
using nf.protoscript.syntaxtree;

namespace nf.protoscript.parser.cs
{

    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option("stepmode", Required = false, HelpText = "Step to run, for debug purpose.")]
        public bool StepMode { get; set; } = false;

        [Option("showtokens", Required = false, HelpText = "Set output to verbose Tokens.")]
        public bool ShowToken { get; set; } = false;

        [Option("showstatements", Required = false, HelpText = "Set output to verbose Statements.")]
        public bool ShowStatements { get; set; } = false;

        [Option("showmodels", Required = false, HelpText = "Set output to verbose Models.")]
        public bool ShowModels { get; set; } = false;

        [Option('f', "file", Default = "", HelpText = "Implicit a file to be parsed.")]
        public string File { get; set; }

        [Option('d', "directory", Default = ".", HelpText = "Implicit a directory that all files in it will be parsed.")]
        public string Directory { get; set; }

        [Option('l', "targetlang", Default = "", HelpText = "Translate nps to the target language.")]
        public string TargetLang { get; set; }

        [Option('t', "targetdir", Default = "", HelpText = "All output files will be found in this directory.")]
        public string TargetDir { get; set; }

        /// <summary>
        /// Scheme root directories.
        /// </summary>
        [Option("schemeroot", HelpText = "Add directories which contain schemes to find.")]
        public IEnumerable<string> SchemeRoot { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            int procID = Process.GetCurrentProcess().Id;
            Console.WriteLine("Start npsc.cs compiler. PID = {0}, BASEDIR={1}", procID, AppDomain.CurrentDomain.BaseDirectory);
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    HandleStepMode(o.StepMode);

                    TestFunc();

                    TestFunc2();
                });

        }


        private static void HandleStepMode(bool InEnabled)
        {
            if (InEnabled)
            {
                Console.WriteLine("[Parser.cs]: STEP MODE, Press <ENTER> to Continue ... ");
                Console.Read();
            }
        }

        private static void TestFunc()
        {
            // a dynamic
            ProjectInfo testProj = new ProjectInfo("TestProj");
            dynamic extra = testProj.Extra;
            extra.Name = "haha";

            // set it in parser-plugin
            {
                extra.Feature = 1984;
            }

            // get it in translator
            {
                Console.WriteLine(extra.Feature);

                // missing the feature.
                try
                {
                    Console.WriteLine(extra.feature_not_exist);
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
                {
                    Console.WriteLine("Successfully catch a runtime-binder");
                }
            }

            // reflection
            {
                testProj.ForeachExtraProperties((name, val) =>
                {
                    Console.WriteLine(name + ": " + val);
                    return true;
                });
            }

        }

        private static void TestFunc2()
        {
            // Parser: parse a project from .nps script files.
            ProjectInfo testProj = new ProjectInfo("TestProj");
            {
                TypeInfo classA = new TypeInfo(testProj, "model", "classA");
                {
                    MemberInfo propA = new MemberInfo(classA, "property", "propA", CommonTypeInfos.Integer
                        , new STNodeConstant(STNodeConstant.Integer, "100")
                        );

                    MemberInfo propB = new MemberInfo(classA, "property", "propB", CommonTypeInfos.Integer
                        , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                            , new STNodeGetVar("propA")
                            , new STNodeConstant(STNodeConstant.Integer, "100")
                            )
                        );

                    // delegate int MethodType(int InParam)
                    DelegateTypeInfo funcAType = new DelegateTypeInfo(testProj, "FuncType", "funcAType");
                    {
                        MemberInfo retVal = new MemberInfo(funcAType, "param", "return", CommonTypeInfos.Integer, null);
                        {
                            AttributeInfo outAttr = new AttributeInfo(retVal, "Out", "Out");
                        }
                        MemberInfo inParam0 = new MemberInfo(funcAType, "param", "InParam", CommonTypeInfos.Integer, null);
                    }

                    // int TestMethodA(int InParam)
                    // which means TestMethodA = new MethodType_funcAType { ... }
                    MethodInfo funcA = new MethodInfo(classA, "method", "TestMethodA", funcAType
                        , new STNodeSequence(
                            // code ln 0: propA = propB + InParam.
                            new STNodeAssign(
                                new STNodeGetVar("propA", true)
                                , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                                    , new STNodeGetVar("propB")
                                    , new STNodeGetVar("InParam")
                                    )
                                ),
                            // code ln 1: return propA (return = propA)
                            new STNodeAssign(
                                new STNodeGetVar("return", true)
                                , new STNodeGetVar("propA")
                                )
                            )
                        );

                } // finish classA

            }

            // Translator: translate the project to a target development environment.
            {
                TestCppTranslator cppTranslator = new TestCppTranslator();
                cppTranslator.Translate(testProj);
            }

        }



    }
}
