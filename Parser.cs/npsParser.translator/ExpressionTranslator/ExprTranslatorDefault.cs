using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{


    public class ExprTranslatorDefault
        : ExprTranslatorAbstract
    {


        protected override ISTNodeTranslateScheme NewErrorScheme(STNodeBase InErrorNode)
        {
            throw new System.NotImplementedException();
        }

        protected override ISTNodeTranslateScheme QueryNullScheme(TypeInfo InConstType)
        {
            throw new System.NotImplementedException();
        }

        protected override ISTNodeTranslateScheme QueryConstGetScheme(TypeInfo InConstType, string InValueString)
        {
            return DefaultConstScheme;
        }

        protected override ISTNodeTranslateScheme QueryConstGetStringScheme(TypeInfo InConstType, string InString)
        {
            return DefaultConstScheme;
        }

        protected override ISTNodeTranslateScheme QueryConstGetInfoScheme(TypeInfo InConstType, Info InInfo)
        {
            return DefaultConstScheme;
        }



        protected override ISTNodeTranslateScheme QueryMemberGetScheme(TypeInfo InContextScope, string InMemberID, out TypeInfo OutMemberType)
        {
            var elemInfo = InfoHelper.FindPropertyOfType(InContextScope, InMemberID);
            if (elemInfo == null)
            {
                OutMemberType = CommonTypeInfos.Any;
            }
            else
            {
                OutMemberType = elemInfo.ElementType;

                // TODO find Member specific getters

            }

            return DefaultVarGetScheme;
        }

        protected override ISTNodeTranslateScheme QueryMemberRefScheme(TypeInfo InContextScope, string InMemberID, out TypeInfo OutMemberType)
        {
            var elemInfo = InfoHelper.FindPropertyOfType(InContextScope, InMemberID);
            if (elemInfo == null)
            {
                OutMemberType = CommonTypeInfos.Any;
            }
            else
            {
                OutMemberType = elemInfo.ElementType;

            }
            return DefaultVarRefScheme;
        }

        protected override ISTNodeTranslateScheme QueryMemberSetScheme(TypeInfo InContextScope, string InMemberID, out TypeInfo OutMemberType)
        {
            var elemInfo = InfoHelper.FindPropertyOfType(InContextScope, InMemberID);
            if (elemInfo == null)
            {
                OutMemberType = CommonTypeInfos.Any;
            }
            else
            {
                OutMemberType = elemInfo.ElementType;

            }
            return DefaultVarSetScheme;
        }


        protected override ISTNodeTranslateScheme QueryBinOpScheme(STNodeBinaryOp InBinOpNode, TypeInfo InLhsType, TypeInfo InRhsType, out TypeInfo OutResultType)
        {
            throw new System.NotImplementedException();
        }


        public STNodeTranslateSchemeDefault DefaultVarGetScheme { get; set; }
        public STNodeTranslateSchemeDefault DefaultVarRefScheme { get; set; }
        public STNodeTranslateSchemeDefault DefaultVarSetScheme { get; set; }
        public STNodeTranslateSchemeDefault DefaultConstScheme { get; set; }
    }



}
