using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.test
{

    public static partial class TestCases
    {

        public static TypeInfo __internal_PanelType = new TypeInfo(SystemTypePackageInfo.Instance, "ui", "panel");
        public static TypeInfo __internal_LabelType = new TypeInfo(SystemTypePackageInfo.Instance, "ui", "label");
        public static TypeInfo __internal_ButtonType = new TypeInfo(SystemTypePackageInfo.Instance, "ui", "button");

        public static TypeInfo __internal_EditorType = new TypeInfo(SystemTypePackageInfo.Instance, "app", "editor");
        public static TypeInfo __internal_AppletType = new TypeInfo(SystemTypePackageInfo.Instance, "app", "applet");

        public static DelegateTypeInfo func_V_V_Type = new DelegateTypeInfo(SystemTypePackageInfo.Instance, "FuncType", "func_V_V_Type");

    

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
                        , new STNodeConstant(100)
                        );
                    {
                        AttributeInfo propAttr = new AttributeInfo(propA, "Property", "Anonymous_Property_Attribute");
                    }

                    // int propB = propA + 100
                    MemberInfo propB = new MemberInfo(classA, "property", "propB", CommonTypeInfos.Integer
                        , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                            , new STNodeGetVar("propA")
                            , new STNodeConstant(100)
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
        /// model testCharacterTemplate
        ///     - HP = 100
        ///     - TestNonBindingValue = Guid()
        ///     +cmd HpUp()
        ///         Hp += 1
        /// 
        /// editor CharacterEditor
        ///     -Model = new testCharacterTemplate();
        ///     +panel CharacterInfoPanel
        ///         -DataContext = $db"Src=ancestor:CharacterEditor, Path=Model"
        ///         +Label
        ///             -Text = $db"HP"
        ///         +Button upBtn
        ///             -Click = $cb"HpUp"
        ///         +Button downBtn
        ///             -Click
        ///                 HpUp -= 1
        ///                 
        /// $applet
        //     -CharacterEditor characterEditor
        ///
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo BasicDataBinding()
        {
            ProjectInfo testProj = new ProjectInfo("TestProj");
            {
                /// model testCharacterTemplate
                ///     - HP = 100
                ///     - TestNonBindingValue = 100
                ///     +cmd HpUp()
                ///         Hp += 1
                TypeInfo characterType = new TypeInfo(testProj, "model", "testCharacterTemplate");
                {
                    // int HP = 100
                    MemberInfo hp = new MemberInfo(characterType, "property", "HP", CommonTypeInfos.Integer
                        , new STNodeConstant(100)
                        );
                    MemberInfo nbval = new MemberInfo(characterType, "property", "NonBindingValue", CommonTypeInfos.Integer
                        , new STNodeConstant(100)
                        );
                    // +cmd HpUp()
                    //     Hp += 1
                    MethodInfo HpUp = new MethodInfo(characterType, "command", "HpUp"
                        , func_V_V_Type
                        , new STNodeSequence(new ISyntaxTreeNode[] {
                            new STNodeAssign(
                                new STNodeGetVar("Hp", true)
                                , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                                    , new STNodeGetVar("Hp")
                                    , new STNodeConstant(1)
                                )
                            )
                        })
                        );
                } // finish Character

                //// TODO better way of internal types
                //TypeInfo internalEditorType = new TypeInfo(null, "model", "Editor");
                //{
                //}

                // editor CharacterEditor
                //     -Model = new testCharacterTemplate();
                // ...
                // override properties of the host.
                TypeInfo chrEditorInlineType = new TypeInfo(testProj, "model", "CharacterEditor");
                {
                    // Base type of the editor.
                    AttributeInfo baseClassAttr = new AttributeInfo(chrEditorInlineType, "base", "base_0"
                        , new STNodeConstant(__internal_EditorType)
                        );

                    //     -Model: testCharacterTemplate
                    // ----
                    // Override base's Model by a new object-template.
                    MemberInfo ovrModel = new MemberInfo(chrEditorInlineType, "override", "Model"
                        , characterType
                        , new STNodeNew(characterType)
                        );

                    //     ...
                    //     -CharacterEditor characterEditor
                    //     +panel characterInfoPanel
                    //         -DataContext = $db"Src=ancestor:CharacterEditor, Path=Model"
                    //         ...
                    // ----
                    // Sub-Objects composite to the host
                    // Sub element: UI panel.
                    ElementInfo panel = new ElementInfo(chrEditorInlineType, "ui", "characterInfoPanel"
                        , __internal_PanelType
                        , null
                        );
                    {
                        AttributeInfo dbAttr = new AttributeInfo(panel, "db", "Anonymous_db_0"
                            , new STNodeDataBinding(
                                    EDataBindingObjectType.Ancestor,
                                    "CharacterEditor",
                                    "Model",
                                    EDataBindingObjectType.This,
                                    "",
                                    "DataContext"
                                )
                            );

                        //         +Label
                        //             -Text=$db"HP"
                        ElementInfo label = new ElementInfo(panel, "ui", "Anonymous_label_0"
                            , __internal_LabelType
                            , null
                            );
                        {
                            AttributeInfo lblDbAttr = new AttributeInfo(label, "db", "Anonymous_db_0"
                                , new STNodeDataBinding("HP", "Text")
                                );

                        } // end label

                        // +Button upBtn
                        //     -Click = $cb"HpUp"
                        ElementInfo upBtn = new ElementInfo(panel, "ui", "upBtn"
                            , __internal_ButtonType
                            , null
                            );
                        {
                            //MemberInfo clickEvt = new MemberInfo(upBtn, "event-impl", "click"
                            //    , __internal_CommandBindingType
                            //    , new STNodeCmdBinding("HpUp")
                            //    );
                        }
                        // +Button downBtn
                        //     -Click
                        //         HpUp -= 1
                        ElementInfo downBtn = new ElementInfo(panel, "ui", "downBtn"
                            , __internal_ButtonType
                            , null
                            );
                        {
                            MethodInfo clickMtd = new MethodInfo(downBtn, "event-impl", "click"
                                , func_V_V_Type
                                // Hp = Hp - 1
                                , new STNodeSequence(new ISyntaxTreeNode[] {
                                    new STNodeAssign(
                                        new STNodeGetVar("Hp", true)
                                        , new STNodeBinaryOp(STNodeBinaryOp.Def.Sub
                                            , new STNodeGetVar("Hp")
                                            , new STNodeConstant(1)
                                        )
                                    )
                                })
                                );
                        }

                    } // end panel 

                }// end editor

                // $applet
                //     -CharacterEditor characterEditor
                //
                // The testApp which should be taken as 'main' entry.
                TypeInfo testAppInlineType = new TypeInfo(testProj, "applet", "InlineApplet_0");
                {
                    // Base type of the editor.
                    AttributeInfo baseClassAttr = new AttributeInfo(testAppInlineType, "base", "base_0"
                        , new STNodeConstant(__internal_AppletType)
                        );

                    // ...
                    //     -CharacterEditor characterEditor
                    MemberInfo hostInstance = new MemberInfo(testAppInlineType, "CharacterEditor", "characterEditor"
                        , chrEditorInlineType
                        , new STNodeNew(chrEditorInlineType)
                        );
                }
                ElementInfo testApp = new ElementInfo(testProj, "global", "applet"
                    , testAppInlineType
                    , new STNodeNew(testAppInlineType)
                    );


            }// end test proj

            return testProj;
        }


    }
}
