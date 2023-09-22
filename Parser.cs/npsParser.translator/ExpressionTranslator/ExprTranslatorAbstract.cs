using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Access type of the expression variable
    /// </summary>
    public enum EExprVarAccessType
    {
        Get,
        Set,
        Ref,
    }


    /// <summary>
    /// Common calls of all Expression-Translators.
    /// </summary>
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

        protected abstract ISTNodeTranslateScheme ErrorScheme(STNodeBase InErrorNode);

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
        // var access schemeInstances
        //

        protected abstract ISTNodeTranslateScheme QueryMemberAccessScheme(
            EExprVarAccessType InAccessType
            , TypeInfo InContextType
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

        /// <summary>
        /// Query scheme to generate host-access codes like "Host.Member"
        /// </summary>
        /// <param name="InMemberAccessNode"></param>
        /// <param name="InHostElement"></param>
        /// <returns></returns>
        protected abstract ISTNodeTranslateScheme QueryHostAccessScheme(
            STNodeMemberAccess InMemberAccessNode
            , TypeInfo InHostType
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
