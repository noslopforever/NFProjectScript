using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.translator.expression
{


    public class ExprTranslatorDefault
        : ExprTranslatorAbstract
    {


        protected override ISTNodeTranslateScheme ErrorScheme(STNodeBase InErrorNode)
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

        protected override ISTNodeTranslateScheme QueryMemberAccessScheme(EExprVarAccessType InAccessType, TypeInfo InContextType, string InMemberID, out TypeInfo OutMemberType)
        {
            var elemInfo = InfoHelper.FindPropertyOfType(InContextType, InMemberID);
            if (elemInfo == null)
            {
                OutMemberType = CommonTypeInfos.Any;
            }
            else
            {
                OutMemberType = elemInfo.ElementType;

                // TODO find Member specific accessors

            }

            switch (InAccessType)
            {
                case EExprVarAccessType.Get:
                    return DefaultVarGetScheme;
                case EExprVarAccessType.Set:
                    return DefaultVarSetScheme;
                case EExprVarAccessType.Ref:
                    return DefaultVarRefScheme;
            }
            return null;
        }

        protected override ISTNodeTranslateScheme QueryBinOpScheme(STNodeBinaryOp InBinOpNode, TypeInfo InLhsType, TypeInfo InRhsType, out TypeInfo OutResultType)
        {
            throw new System.NotImplementedException();
        }

        protected override ISTNodeTranslateScheme QueryHostAccessScheme(STNodeMemberAccess InMemberAccessNode, TypeInfo InHostType)
        {
            return DefaultHostAccessScheme;
        }

        public STNodeTranslateSchemeDefault DefaultVarGetScheme { get; set; }
        public STNodeTranslateSchemeDefault DefaultVarRefScheme { get; set; }
        public STNodeTranslateSchemeDefault DefaultVarSetScheme { get; set; }
        public STNodeTranslateSchemeDefault DefaultConstScheme { get; set; }
        public STNodeTranslateSchemeDefault DefaultHostAccessScheme { get; set; }
    }



}
