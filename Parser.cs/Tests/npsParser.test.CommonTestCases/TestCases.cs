using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.test
{
    public static class TestCases
    {
        public static ProjectInfo BasicLanguage()
        {
            // Parser: parse a project from .nps script files.
            ProjectInfo testProj = new ProjectInfo("TestProj");
            {
                TypeInfo classA = new TypeInfo(testProj, "model", "classA");
                {
                    MemberInfo propA = new MemberInfo(classA, "property", "propA", CommonTypeInfos.Integer
                        , new STNodeConstant(STNodeConstant.Integer, "100")
                        );
                    {
                        AttributeInfo propAttr = new AttributeInfo(propA, "Property", "Anonymous_Property_Attribute");
                    }

                    MemberInfo propB = new MemberInfo(classA, "property", "propB", CommonTypeInfos.Integer
                        , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                            , new STNodeGetVar("propA")
                            , new STNodeConstant(STNodeConstant.Integer, "100")
                            )
                        );

                    // delegate int MethodType(int InParam)
                    DelegateTypeInfo funcAType = new DelegateTypeInfo(testProj, "FuncType", "funcAType");
                    {
                        MemberInfo retVal = new MemberInfo(funcAType, "param", "___return___", CommonTypeInfos.Integer, null);
                        {
                            AttributeInfo retAttr = new AttributeInfo(retVal, "Return", "Anonymous_Return_Property");
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
                                new STNodeGetVar("___return___", true)
                                , new STNodeGetVar("propA")
                                )
                            )
                        );

                } // finish classA

            }

            return testProj;
        }

    }
}
