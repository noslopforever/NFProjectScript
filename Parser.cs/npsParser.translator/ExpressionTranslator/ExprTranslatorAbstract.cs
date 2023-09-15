using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace nf.protoscript.translator.expression
{


    public abstract partial class ExprTranslatorAbstract
    {

 

        /// <summary>
        /// Translate syntax tree into codes.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> Translate(IExprTranslateContext InContext, ISyntaxTreeNode InSyntaxTree)
        {
            // Gather schemeInstances.
            STNodeVisitor_FunctionBody visitor = new STNodeVisitor_FunctionBody(this, InContext);
            VisitByReflectionHelper.FindAndCallVisit(InSyntaxTree, visitor);

            // Create runtime context and do apply schemeInstances.
            var schemeInstances = visitor.TranslateSchemeInstances;
            List<string> codes = new List<string>();
            _HandleSchemeInstances(codes, schemeInstances);

            return codes;
        }

        //
        // Constant access schemeInstances
        //

        protected abstract ISTNodeTranslateScheme NewErrorScheme(STNodeBase InErrorNode);

        protected abstract ISTNodeTranslateScheme QueryNullScheme(TypeInfo InConstType);

        protected abstract ISTNodeTranslateScheme QueryConstGetInfoScheme(
            TypeInfo InConstType
            , Info InInfo
            );

        protected abstract ISTNodeTranslateScheme QueryConstGetStringScheme(
            TypeInfo InConstType
            , string InString
            );

        protected abstract ISTNodeTranslateScheme QueryConstGetScheme(
            TypeInfo InConstType
            , string InValueString
            );


        //
        // Var access schemeInstances
        //

        protected abstract ISTNodeTranslateScheme QueryVarGetScheme(
            TypeInfo InContextScope
            , string InMemberID
            , out TypeInfo OutMemberType
            );

        protected abstract ISTNodeTranslateScheme QueryVarSetScheme(
            TypeInfo InContextScope
            , string InMemberID
            , out TypeInfo OutMemberType
            );

        protected abstract ISTNodeTranslateScheme QueryVarRefScheme(
            TypeInfo InContextScope
            , string InMemberID
            , out TypeInfo OutMemberType
            );
        

        //
        // Operation schemeInstances.
        //

        protected abstract ISTNodeTranslateScheme QueryBinOpScheme(
            STNodeBinaryOp InBinOpNode
            , TypeInfo InLhsType
            , TypeInfo InRhsType
            , out TypeInfo OutResultType
            );



        private void _HandleSchemeInstances(
            List<string> OutCodes
            , IEnumerable<ISTNodeTranslateSchemeInstance> InSchemeInstances
            )
        {
            foreach (var schemeInst in InSchemeInstances)
            {
                var presentResult = schemeInst.GetResult("Present");
                OutCodes.AddRange(presentResult);
            }
        }

    }


}
