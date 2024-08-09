using nf.protoscript.syntaxtree;
using nf.protoscript.translator.DefaultScheme;
using nf.protoscript.translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npsParser.test.ExpressionTranslator
{
    partial class Program
    {
        private static InfoTranslatorAbstract _CreateLoadFromJsonTranslator()
        {
            var translator = new InfoTranslatorDefault();

            var serialData = SchemeJsonLoader.GenerateDefaultSerializeData();
            Console.WriteLine(serialData);

            //"""
            //{
            //    "Name":"Get",
            //    "Params":["Param0"],
            //    "Condition":"${Condition}",
            //    "Code":"${Code}"
            //}
            //"""


            //SchemeJsonLoader.LoadSchemeFromJson(translator,
            //    """
            //    {
            //        "Name":"Get",
            //        "Params":[],
            //        "Condition": "Type_Name == 'Constant'",
            //        "Code":"${ValueString}"
            //    }
            //    """
            //    );

            var schemesJson =
                """
                [
                    {
                        "Name":"EvalConstant",
                        "Code":"${ValueString}"
                    },
                    {
                        "Name":"EvalConstString",
                        "Code":"'${ValueString}'"
                    },
                    {
                        "Name":"EvalBinOp",
                        "Code":"${LHS.Get()} ${OpCode} ${RHS.Get()}"
                    },
                    {
                        "Name":"EvalUnaryOp",
                        "Code":"${OpCode} ${RHS.Get()}"
                    },
                    {
                        "Name":"EvalAssign",
                        "Code":"${LHS.Set(RHS.Get())}"
                    },
                    {
                        "Name":"EvalVar",
                        "Code":"${HostPresent(IDName)}"
                    },
                    {
                        "Name":"EvalMemberAccess",
                        "Code":"${LHS.Get()}.${IDName}"
                    },
                    {
                        "Name": "EvalCall",
                        "Code":"${FuncExpr.Get()}(${For('Param', ', ', 'Get')})"
                    },
                    {
                        "Name":"EvalCollAccess",
                        "Code":"${CollExpr.Get()}[${For('Param', '][', 'Get')}]"
                    },



                    {
                        "Name":"Set",
                        "Params":[ "RHS_VALUE" ],
                        "Condition":"",
                        "Code":"<UNEXPECTED SET>"
                    },
                    {
                        "Name":"Set",
                        "Params":[ "RHS_VALUE" ],
                        "Condition":"Type_Name == 'Var'",
                        "Code":"${HostPresent(IDName)} = ${RHS_VALUE}"
                    },
                    {
                        "Name":"Set",
                        "Params":[ "RHS_VALUE" ],
                        "Condition":"Type_Name == 'MemberAccess'",
                        "Code":"${LHS.Get()}.${IDName} = ${RHS_VALUE}"
                    },



                    {
                        "Name":"HostPresent",
                        "Params":[ "VAR_NAME" ],
                        "Code":"${VAR_NAME}"
                    }
                                    
                                                                                
                ]
                """
                ;
            SchemeJsonLoader.LoadSchemes(translator, schemesJson);

            return translator;
        }

    }


}
