using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using nf.protoscript;
using nf.protoscript.syntaxtree;
using nf.protoscript.test;
using nf.protoscript.translator;
using nf.protoscript.translator.DefaultScheme;
using nf.protoscript.translator.DefaultScheme.Elements;
using nf.protoscript.translator.SchemeSelectors;

namespace npsParser.test.ExpressionTranslator
{

    partial class Program
    {

        static void Main(string[] args)
        {
            var testSuites = new (string, Func<InfoTranslatorAbstract>)[]
            {
                ("Hardcode [Default, Fallback]", _CreateHardcodeTranslator),
                ("Json", _CreateLoadFromJsonTranslator),
            };

            int procID = Process.GetCurrentProcess().Id;

            // Output selections and wait for 
            Console.WriteLine("Which translator do you want to test?");
            for (int i = 0; i < testSuites.Length; i++)
            {
                var testSuite = testSuites[i];
                Console.WriteLine($"    {i}: {testSuite.Item1}");
            }
            string selection = Console.ReadLine();
            int selectionIndex = 0;
            if (!int.TryParse(selection, out selectionIndex))
            {
                selectionIndex = 0;
            }
            selectionIndex = Math.Clamp(selectionIndex, 0, testSuites.Length - 1);
            Console.WriteLine($"Select Translator {selectionIndex}: {testSuites[selectionIndex].Item1}");

            // Create translator by selection.
            var translator = testSuites[selectionIndex].Item2.Invoke();

            // # Basic syntax tree nodes:

            // ## Constants
            {
                // ### Constant : integer
                _TranslateExprNode(translator, new STNodeConstant(100));

                // ### Constant : double
                _TranslateExprNode(translator, new STNodeConstant(999999999.999999999));

                // ### Constant : float
                _TranslateExprNode(translator, new STNodeConstant(66.66f));

                // ### Constant : string
                _TranslateExprNode(translator, new STNodeConstant("Test_String"));

                // ### Constant : type
                _TranslateExprNode(translator, new STNodeConstant(CommonTypeInfos.Any));
            }

            // ## Bin-Ops
            {
                {
                    var lhs = new STNodeConstant(100);
                    var rhs = new STNodeConstant(200);
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.LessThan), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.LessThanOrEqual), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.GreaterThan), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.GreaterThanOrEqual), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Equal), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.NotEqual), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Add), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Substract), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Multiply), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Divide), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Mod), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Exp), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.And), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.Or), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.BitwiseAnd), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.BitwiseOr), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.ShiftLeft), lhs, rhs));
                    _TranslateExprNode(translator, new STNodeBinaryOp(OpDefManager.Instance.Get(EOpFunction.ShiftRight), lhs, rhs));
                }
            }

            // ## Unary-Ops
            {
                {
                    var rhs = new STNodeConstant(200);
                    _TranslateExprNode(translator, new STNodeUnaryOp(OpDefManager.Instance.Get(EOpFunction.Not), rhs));
                    _TranslateExprNode(translator, new STNodeUnaryOp(OpDefManager.Instance.Get(EOpFunction.BitwiseNot), rhs));
                    _TranslateExprNode(translator, new STNodeUnaryOp(OpDefManager.Instance.Get(EOpFunction.Positive), rhs));
                    _TranslateExprNode(translator, new STNodeUnaryOp(OpDefManager.Instance.Get(EOpFunction.Negative), rhs));
                    _TranslateExprNode(translator, new STNodeUnaryOp(OpDefManager.Instance.Get(EOpFunction.Increment), rhs));
                    _TranslateExprNode(translator, new STNodeUnaryOp(OpDefManager.Instance.Get(EOpFunction.Decrement), rhs));
                }
            }

            // ## Var Get/Set - Assign
            {
                {
                    var lhs = new STNodeVar("a");
                    var rhs = new STNodeConstant(100);
                    _TranslateExprNode(translator, new STNodeAssign(lhs, rhs));
                }
                {
                    var lhs = new STNodeVar("a");
                    var rhs = new STNodeVar("r");
                    _TranslateExprNode(translator, new STNodeAssign(lhs, rhs));
                }
            }

            // ## Member Access
            {
                {
                    var lhs = new STNodeMemberAccess(new STNodeVar("a"), "b");
                    var rhs = new STNodeConstant(200);
                    _TranslateExprNode(translator, new STNodeAssign(lhs, rhs));
                }

                {
                    var a = new STNodeVar("a");
                    var a_b = new STNodeMemberAccess(a, "b");
                    var a_b_c = new STNodeMemberAccess(a_b, "c");
                    _TranslateExprNode(translator, a_b_c);
                }

            }

            // ## Call
            {
                {
                    var param0 = new STNodeConstant("Hello");
                    var param1 = new STNodeConstant("World");
                    var func = new STNodeVar("foo");
                    _TranslateExprNode(translator, new STNodeCall(func, new ISyntaxTreeNode[] { param0, param1 }));
                }
            }

            // ## Collection access
            {
                {
                    var paramIndex = new STNodeConstant(1);
                    var coll = new STNodeVar("array");
                    _TranslateExprNode(translator, new STNodeCollectionAccess(coll, new ISyntaxTreeNode[] { paramIndex }));
                }
                {
                    var paramIndex0 = new STNodeConstant(3);
                    var paramIndex1 = new STNodeConstant(5);
                    var coll = new STNodeVar("array2d");
                    _TranslateExprNode(translator, new STNodeCollectionAccess(coll, new ISyntaxTreeNode[] { paramIndex0, paramIndex1 }));
                }
            }

            // ## Sequences
            {
                {
                    List<ISyntaxTreeNode> list = new List<ISyntaxTreeNode>();
                    list.Add(new STNodeConstant("// Seq Start"));
                    list.Add(new STNodeConstant("// Seq 0"));
                    list.Add(new STNodeConstant("// Seq 1"));
                    list.Add(new STNodeConstant("// Seq 2"));
                    list.Add(new STNodeConstant("// Seq 3"));
                    list.Add(new STNodeConstant("// Seq End"));
                    _TranslateExprNode(translator, new STNodeSequence(list.ToArray()));
                }

            }

        }


        static void _TranslateExprNode(InfoTranslatorAbstract InTranslator, ISyntaxTreeNode InNode)
        {
            Console.WriteLine($"Translating {InNode}");

            var ctx = InTranslator.CreateContext(null, InNode);
            var codes = InTranslator.TranslateInfo(ctx, "Get");
            foreach (var code in codes)
            {
                Console.WriteLine($"    " + code);
            }

        }

    }



    static class TestSetBackErrors
    {
        static void foo(ref int P0, ref int P1)
        {
            P0++;
            P1++;
        }

        class A
        {
            public int _p;
            public int p { get { return _p; } set { _p = value; } }
            public ref int refp { get { return ref _p; } }
        }

        static void main()
        {
            int procID = Process.GetCurrentProcess().Id;

            {
                A a = new A();
                foo(ref a.refp, ref a.refp);
                Console.WriteLine(a.p); // 2, success
            }
            {
                //A a = new A();
                //foo(ref a.p, ref a.p);
                //Console.WriteLine(a.p);
            }
            {
                A a = new A();
                int t0 = a.p;
                int t1 = a.p;
                foo(ref t0, ref t1);
                a.p = t0;
                a.p = t1;
                Console.WriteLine($"ap {a.p}, t0 {t0}, t1 {t1}"); // 1 1 1, fail (of course)
            }

        }

    }

}