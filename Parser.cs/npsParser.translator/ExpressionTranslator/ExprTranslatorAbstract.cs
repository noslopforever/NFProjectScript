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

        /// <summary>
        /// Find Scheme for translating member-accesses.
        /// </summary>
        /// <param name="InAccessType"></param>
        /// <param name="InContextType"></param>
        /// <param name="InMemberID"></param>
        /// <param name="OutMemberType"></param>
        /// <returns></returns>
        protected abstract ISTNodeTranslateScheme QueryMemberAccessScheme(
            EExprVarAccessType InAccessType
            , TypeInfo InContextType
            , string InMemberID
            , out TypeInfo OutMemberType
            );

        /// <summary>
        /// Find Scheme for translating global-var accesses.
        /// </summary>
        /// <param name="InAccessType"></param>
        /// <param name="InGlobalInfo"></param>
        /// <param name="InVarName"></param>
        /// <param name="OutVarType"></param>
        /// <returns></returns>
        protected abstract ISTNodeTranslateScheme QueryGlobalVarAccessScheme(
            EExprVarAccessType InAccessType
            , Info InGlobalInfo
            , string InVarName
            );

        /// <summary>
        /// Find Scheme for translating method-scope vars (parameters and locals).
        /// </summary>
        /// <param name="InAccessType"></param>
        /// <param name="InTranslatingContext"></param>
        /// <param name="InMethodInfo"></param>
        /// <param name="InVarName"></param>
        /// <param name="OutVarType"></param>
        /// <returns></returns>
        protected abstract ISTNodeTranslateScheme QueryMethodVarAccessScheme(
            EExprVarAccessType InAccessType
            , IExprTranslateContext InTranslatingContext
            , ElementInfo InMethodInfo
            , string InVarName
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

    }


}
