using nf.protoscript;
using nf.protoscript.syntaxtree;
using System;

namespace nf.protoscript.test
{

    public static partial class TestCases
    {

        public static TypeInfo __internal_UIBaseType = new TypeInfo(SystemTypePackageInfo.Instance, "ui", "uibase");
        public static TypeInfo __internal_PanelType = new TypeInfo(SystemTypePackageInfo.Instance, "ui", "panel", __internal_UIBaseType);
        public static TypeInfo __internal_LabelType = new TypeInfo(SystemTypePackageInfo.Instance, "ui", "label", __internal_UIBaseType);
        public static TypeInfo __internal_ButtonType = new TypeInfo(SystemTypePackageInfo.Instance, "ui", "button", __internal_UIBaseType);

        public static TypeInfo __internal_DataContextCallType = new TypeInfo(SystemTypePackageInfo.Instance, "data", "DataContextCall");

        public static TypeInfo __internal_EditorType = new TypeInfo(SystemTypePackageInfo.Instance, "app", "editor");
        public static TypeInfo __internal_AppletType = new TypeInfo(SystemTypePackageInfo.Instance, "app", "applet");

        public static DelegateTypeInfo func_V_V_Type = new DelegateTypeInfo(SystemTypePackageInfo.Instance, "FuncType", "func_V_V_Type");

        static TestCases()
        {
            // UIBase internal subinfos
            {
                ElementInfo dataContext = new ElementInfo(__internal_UIBaseType, "property", "dataContext"
                    , CommonTypeInfos.Any
                    , null
                    );

                ElementInfo click = new ElementInfo(__internal_UIBaseType, "event", "click"
                    , func_V_V_Type
                    , null
                    );
            }
        }


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
                    ElementInfo propA = new ElementInfo(classA, "property", "propA"
                        , CommonTypeInfos.Integer
                        , new STNodeConstant(100)
                        );
                    {
                        AttributeInfo propAttr = new AttributeInfo(propA, "Property", "Anonymous_Property_Attribute");
                    }

                    // int propB = propA + 100
                    ElementInfo propB = new ElementInfo(classA, "property", "propB"
                        , CommonTypeInfos.Integer
                        , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                            , new STNodeVar("propA")
                            , new STNodeConstant(100)
                            )
                        );

                    // delegate int func_I_I_Type(int InParam)
                    DelegateTypeInfo func_I_I_Type = new DelegateTypeInfo(testProj, "FuncType", "func_I_I_Type");
                    {
                        ElementInfo retVal = new ElementInfo(func_I_I_Type, "param", "___return___"
                            , CommonTypeInfos.Integer
                            , null
                            );
                        {
                            AttributeInfo retAttr = new AttributeInfo(retVal, "Return", "__Anonymous_Return_Property__");
                        }
                        ElementInfo inParam0 = new ElementInfo(func_I_I_Type, "param", "InParam"
                            , CommonTypeInfos.Integer
                            , null
                            );
                    }

                    // int TestMethodA(int InParam)
                    //      propA = propB + InParam
                    //      return propA
                    // which means TestMethodA = new func_I_I_Type { ... }
                    ElementInfo funcA = new ElementInfo(classA, "method", "TestMethodA", func_I_I_Type
                        , new STNodeSequence(
                            // code ln 0: propA = propB + InParam.
                            new STNodeAssign(
                                new STNodeVar("propA", true)
                                , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                                    , new STNodeVar("propB")
                                    , new STNodeVar("InParam")
                                    )
                                ),
                            // code ln 1: return propA (return = propA)
                            // TODO a better __return__ var
                            new STNodeAssign(
                                new STNodeVar("___return___", true)
                                , new STNodeVar("propA")
                                )
                            )
                        );

                    // delegate void func_V_IR_Type(int&)
                    DelegateTypeInfo func_V_IR_Type = new DelegateTypeInfo(testProj, "FuncType", "funcV_IR_Type");
                    {
                        ElementInfo refParam0 = new ElementInfo(func_I_I_Type, "param", "RefParam"
                            , CommonTypeInfos.Integer
                            , null
                            );
                    }

                    //
                    // void TestMethodB(int& RefParam)
                    //
                    ElementInfo funcB = new ElementInfo(classA, "method", "TestMethodB"
                        , func_V_IR_Type
                        , new STNodeSequence(
                                // code ln 0: propA = propA + RefParam
                                new STNodeAssign(
                                    new STNodeVar("propA", true)
                                    , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                                        , new STNodeVar("propA", false)
                                        , new STNodeVar("RefParam")
                                        )
                                    ),
                                // code ln 1: RefParam = propA
                                new STNodeAssign(
                                    new STNodeVar("RefParam", true)
                                    , new STNodeVar("propA")
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
                    ElementInfo funcC = new ElementInfo(classA, "method", "TestMethodC"
                        , func_V_V_Type
                        , new STNodeSequence(
                            // code ln 0: TestMethodB(propA)
                            new STNodeCall(
                                new STNodeVar("TestMethodB")
                                , new STNodeVar("propA", true))
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
        ///     +[cmd] HpUp()
        ///         Hp += 1
        /// 
        /// editor CharacterEditor
        ///     -Model = new testCharacterTemplate();
        ///     +panel CharacterInfoPanel
        ///         -DataContext = $db"Src=ancestor:CharacterEditor, Path=Model"
        ///         --Label
        ///             -Text = $db"HP"
        ///         --Button upBtn
        ///             -Text = "+1"
        ///             ~Click += new DataContextCall("HpUp")
        ///         --Button downBtn
        ///             -Text = "-1"
        ///             ~Click
        ///                 dataContext.HpUp -= 1
        ///                 
        /// $applet
        ///     -CharacterEditor characterEditor
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
                    ElementInfo hp = new ElementInfo(characterType, "property", "HP"
                        , CommonTypeInfos.Integer
                        , new STNodeConstant(100)
                        );
                    ElementInfo nbval = new ElementInfo(characterType, "property", "NonBindingValue"
                        , CommonTypeInfos.Integer
                        , new STNodeConstant(100)
                        );
                    // +cmd HpUp()
                    //     Hp += 1
                    ElementInfo HpUp = new ElementInfo(characterType, "command", "HPUp"
                        , func_V_V_Type
                        , new STNodeSequence(new ISyntaxTreeNode[] {
                            new STNodeAssign(
                                new STNodeVar("HP", true)
                                , new STNodeBinaryOp(STNodeBinaryOp.Def.Add
                                    , new STNodeVar("HP")
                                    , new STNodeConstant(1)
                                )
                            )
                        })
                        );
                } // finish Character

                // editor CharacterEditor
                //     -Model = new testCharacterTemplate();
                // ...
                // override properties of the host.
                TypeInfo chrEditorInlineType = new TypeInfo(testProj, "model", "CharacterEditor", __internal_EditorType);
                {
                    //     -Model: testCharacterTemplate
                    // ----
                    // Override base's Model by a new object-template.
                    ElementInfo ovrModel = new ElementInfo(chrEditorInlineType, "ovr-property", "Model"
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

                        // +Label
                        //     -Text=$db"HP"
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
                        //     -Text = "+1"
                        //     -Click = new dataContextCall("HpUp")
                        ElementInfo upBtn = new ElementInfo(panel, "ui", "upBtn"
                            , __internal_ButtonType
                            , null
                            );
                        {
                            ElementInfo text = new ElementInfo(upBtn, "ovr-property", "Text"
                                , CommonTypeInfos.String
                                , new STNodeConstant("+1")
                                );

                            ElementInfo clickEvtHandler = new ElementInfo(upBtn, "event-impl", "click"
                                , func_V_V_Type
                                , new STNodeNew(__internal_DataContextCallType
                                    , new ISyntaxTreeNode[]
                                        {
                                            new STNodeConstant("HPUp")
                                        }
                                    )
                                );
                        }
                        // +Button downBtn
                        //     -Text = "-1"
                        //     -Click
                        //         dataContext.HpUp -= 1
                        ElementInfo downBtn = new ElementInfo(panel, "ui", "downBtn"
                            , __internal_ButtonType
                            , null
                            );
                        {
                            ElementInfo text = new ElementInfo(downBtn, "ovr-property", "Text"
                                , CommonTypeInfos.String
                                , new STNodeConstant("-1")
                                );

                            ElementInfo clickMtd = new ElementInfo(downBtn, "event-impl", "click"
                                , func_V_V_Type
                                // dataContext.Hp = dataContext.Hp - 1
                                , new STNodeSequence(new ISyntaxTreeNode[] {
                                    new STNodeAssign(
                                        new STNodeSub(
                                            new STNodeVar("dataContext")
                                            , new STNodeVar("HP", true)
                                            )
                                        , new STNodeBinaryOp(STNodeBinaryOp.Def.Sub
                                            , new STNodeSub(
                                                new STNodeVar("dataContext")
                                                , new STNodeVar("HP", true)
                                                )
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
                TypeInfo testAppInlineType = new TypeInfo(testProj, "model", "InlineApplet_0", __internal_AppletType);
                {
                    // ...
                    //     -CharacterEditor characterEditor
                    ElementInfo hostInstance = new ElementInfo(testAppInlineType, "ovr-property", "characterEditor"
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
