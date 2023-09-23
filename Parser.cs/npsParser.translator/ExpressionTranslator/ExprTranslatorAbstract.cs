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

            // Take each scheme-instance as a statement and try to translate it.
            List<string> codes = new List<string>();
            foreach (var schemeInst in visitor.TranslateSchemeInstances)
            {
                var stmtCodes = TranslateOneStatement(schemeInst);
                codes.AddRange(stmtCodes);
            }

            return codes;
        }

        /// <summary>
        /// Translate only one statement
        /// </summary>
        /// <param name="InSchemeInstanceOfStatement"></param>
        /// <returns></returns>
        public virtual IReadOnlyList<string> TranslateOneStatement(ISTNodeTranslateSchemeInstance InSchemeInstanceOfStatement)
        {
            // Gather all SIs
            List<ISTNodeTranslateSchemeInstance> allSubSIs = new List<ISTNodeTranslateSchemeInstance>();
            _RecursivePrerequisite(InSchemeInstanceOfStatement, si => allSubSIs.Add(si));

            // Construct reverse Sub SIs
            var revSubSIs = new List<ISTNodeTranslateSchemeInstance>(allSubSIs);
            revSubSIs.Reverse();

            // Gather all Pre-statement codes from sub-SIs
            List<string> codes = new List<string>();
            foreach (var si in allSubSIs)
            {
                codes.AddRange(si.GetResult("PreStatement"));
            }

            var presentResult = InSchemeInstanceOfStatement.GetResult("Present");
            codes.AddRange(presentResult);

            // Gather all Post-statement(Rev) codes from sub-SIs
            foreach (var si in revSubSIs)
            {
                codes.AddRange(si.GetResult("PostStatement"));
            }
            foreach (var si in revSubSIs)
            {
                codes.AddRange(si.GetResult("PostStatementRev"));
            }

            return codes;
        }

        /// <summary>
        /// Iterate prerequisites recursively of InCurrentSI
        /// </summary>
        /// <param name="InCurrentSI"></param>
        /// <param name="InFunc"></param>
        private void _RecursivePrerequisite(ISTNodeTranslateSchemeInstance InCurrentSI, Action<ISTNodeTranslateSchemeInstance> InFunc)
        {
            foreach (var preSI in InCurrentSI.PrerequisiteSchemeInstances)
            {
                InFunc(preSI);
                _RecursivePrerequisite(preSI, InFunc);
            }
        }


        //
        // System Schemes
        // 

        /// <summary>
        /// Return the scheme when error occurs in translating.
        /// </summary>
        /// <param name="InErrorNode"></param>
        /// <returns></returns>
        protected abstract ISTNodeTranslateScheme ErrorScheme(STNodeBase InErrorNode);

        /// <summary>
        /// Return the scheme to init TempVars.
        /// </summary>
        /// <param name="InTranslatingNode"></param>
        /// <param name="InName"></param>
        /// <param name="InTempVarInitValue"></param>
        /// <returns></returns>
        public abstract ISTNodeTranslateScheme QueryInitTempVarScheme(
            ISyntaxTreeNode InTranslatingNode
            , string InName
            , ISTNodeTranslateSchemeInstance InTempVarInitValue
            );

        //
        // Constant access Schemes
        //

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
        // var access Schemes
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
        // Operation Schemes.
        //

        /// <summary>
        /// Query scheme to generate Bin-Op codes.
        /// </summary>
        /// <param name="InBinOpNode"></param>
        /// <param name="InLhsType"></param>
        /// <param name="InRhsType"></param>
        /// <param name="OutResultType"></param>
        /// <returns></returns>
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
