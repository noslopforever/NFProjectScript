using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.test
{
    public static partial class TestCases
    {

        /// <summary>
        /// Basic language test:
        /// model classA
        ///     - propA = 100 @Property
        ///     - propB = propA + 100
        ///     methodA([Return] int return, int InParam)
        ///         propA = propB + InParam
        ///         return = propA
        /// 
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo BasicLanguage()
        {
            // Parser: parse a project from .nps script files.
            ProjectInfo testProj = new ProjectInfo("TestProj");
            {
                TypeInfo classA = new TypeInfo(testProj, "model", "classA");
                {
                    // int propA = 100
                    MemberInfo propA = new MemberInfo(classA, "property", "propA", CommonTypeInfos.Integer
                        , new STNodeConstant(STNodeConstant.Integer, "100")
                        );
                    {
                        AttributeInfo propAttr = new AttributeInfo(propA, "Property", "Anonymous_Property_Attribute");
                    }

                    // int propB = propA + 100
                    MemberInfo propB = new MemberInfo(classA, "property", "propB", CommonTypeInfos.Integer
                        , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                            , new STNodeGetVar("propA")
                            , new STNodeConstant(STNodeConstant.Integer, "100")
                            )
                        );

                    // delegate int func_I_I_Type(int InParam)
                    DelegateTypeInfo func_I_I_Type = new DelegateTypeInfo(testProj, "FuncType", "func_I_I_Type");
                    {
                        MemberInfo retVal = new MemberInfo(func_I_I_Type, "param", "___return___", CommonTypeInfos.Integer, null);
                        {
                            AttributeInfo retAttr = new AttributeInfo(retVal, "Return", "__Anonymous_Return_Property__");
                        }
                        MemberInfo inParam0 = new MemberInfo(func_I_I_Type, "param", "InParam", CommonTypeInfos.Integer, null);
                    }

                    // int TestMethodA(int InParam)
                    //      propA = propB + InParam
                    //      return propA
                    // which means TestMethodA = new func_I_I_Type { ... }
                    MethodInfo funcA = new MethodInfo(classA, "method", "TestMethodA", func_I_I_Type
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
                            // TODO a better __return__ var
                            new STNodeAssign(
                                new STNodeGetVar("___return___", true)
                                , new STNodeGetVar("propA")
                                )
                            )
                        );

                    // delegate void func_V_IR_Type(int&)
                    DelegateTypeInfo func_V_IR_Type = new DelegateTypeInfo(testProj, "FuncType", "funcV_IR_Type");
                    {
                        MemberInfo refParam0 = new MemberInfo(func_I_I_Type, "param", "RefParam", CommonTypeInfos.Integer, null);
                    }

                    //
                    // void TestMethodB(int& RefParam)
                    //
                    MethodInfo funcB = new MethodInfo(classA, "method", "TestMethodB", func_V_IR_Type
                        , new STNodeSequence(
                                // code ln 0: propA = propA + RefParam
                                new STNodeAssign(
                                    new STNodeGetVar("propA", true)
                                    , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                                        , new STNodeGetVar("propA", false)
                                        , new STNodeGetVar("RefParam")
                                        )
                                    ),
                                // code ln 1: RefParam = propA
                                new STNodeAssign(
                                    new STNodeGetVar("RefParam", true)
                                    , new STNodeGetVar("propA")
                                    )
                            )
                        );


                    // delegate void func_V_V_Type()
                    DelegateTypeInfo func_V_V_Type = new DelegateTypeInfo(testProj, "FuncType", "func_V_V_Type");

                    //
                    // void TestMethodC()
                    //
                    //
                    // Because propA is a setter property, so the results should be like:
                    //      int tmp = get_propA();
                    //      TestMethodB(tmp);
                    //      set_propA(tmp);
                    //
                    MethodInfo funcC = new MethodInfo(classA, "method", "TestMethodC", func_V_V_Type
                        , new STNodeSequence(
                            // code ln 0: TestMethodB(propA)
                            new STNodeCall("TestMethodB"
                                , new STNodeGetVar("propA", true))
                            )
                        );

                } // finish classA

            }

            return testProj;
        }


        /// <summary>
        /// Basic data-binding test:
        /// 
        /// model Character
        ///     - HP = 100
        ///     - NonBindingValue = 100
        /// 
        /// world SimpleWorld
        ///     -Character TestCharacter
        ///     
        /// editor host
        ///     -SimpleWorld TestWorld
        ///     +CharacterInfoPanel testPanel
        ///         -DataContext = $db"TestWorld.TestCharacter"
        ///         +Label
        ///             -Text = $db"HP"
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo BasicDataBinding()
        {
            ProjectInfo testProj = new ProjectInfo("TestProj");
            {
                // model Character
                //     - HP = 100
                TypeInfo characterType = new TypeInfo(testProj, "model", "Character");
                {
                    // int HP = 100
                    MemberInfo hp = new MemberInfo(characterType, "property", "HP", CommonTypeInfos.Integer
                        , new STNodeConstant(STNodeConstant.Integer, "100")
                        );
                    MemberInfo nbval = new MemberInfo(characterType, "property", "NonBindingValue", CommonTypeInfos.Integer
                        , new STNodeConstant(STNodeConstant.Integer, "100")
                        );
                } // finish Character

                /// world SimpleWorld
                ///     -Character TestCharacter
                TypeInfo simpleWorldType = new TypeInfo(testProj, "world", "SimpleWorld");
                {
                    MemberInfo testCharacter = new MemberInfo(simpleWorldType, "Character", "TestCharacter", characterType, null);
                }

                // editor host
                //     -SimpleWorld TestWorld
                // ...
                // MENTION: Here we register a inline type for 'Editor'. This process should have been done by parsers.
                TypeInfo inlineEditorType = new TypeInfo(testProj, "editor", "InlineEditor_0");
                {
                    MemberInfo testWorld = new MemberInfo(inlineEditorType, "SimpleWorld", "TestWorld", simpleWorldType, null);
                }
                Info editor = new Info(testProj, "editor", "host");
                {
                    //     +uipanel characterInfoPanel
                    //         @db = TestWorld.TestCharacter
                    //         ...
                    Info panel = new Info(editor, "uipanel", "characterInfoPanel");
                    {
                        AttributeInfo dbAttr = new AttributeInfo(panel, "db", "Anonymous_db_0"
                            , new STNodeSub(
                                new STNodeGetVar("TestWorld")
                                , new STNodeGetVar("TestCharacter")
                                )
                            );

                        //         +Label
                        //             @db=HP
                        Info label = new Info(panel, "label", "Anonymous_label_0");
                        {
                            AttributeInfo lblDbAttr = new AttributeInfo(label, "db", "Anonymous_db_0"
                                , new STNodeGetVar("HP")
                                );

                        } // end label

                    } // end panel 

                }// end editor

            }// end test proj

            return testProj;
        }


    }
}
