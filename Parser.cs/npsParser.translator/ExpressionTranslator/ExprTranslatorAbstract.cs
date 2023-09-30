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
        public IReadOnlyList<string> Translate(IExprTranslateEnvironment InEnvironment, ISyntaxTreeNode InSyntaxTree)
        {
            // Gather schemeInstances.
            STNodeVisitor_FunctionBody visitor = new STNodeVisitor_FunctionBody(this, InEnvironment);
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
            List<string> codes = new List<string>();
            TranslateOneStatement(codes, InSchemeInstanceOfStatement);
            return codes;
        }

        /// <summary>
        /// Translate only one statement, Get result by a ref-list parameter.
        /// </summary>
        /// <param name="RefResultCodes"></param>
        /// <param name="InSchemeInstanceOfStatement"></param>
        public void TranslateOneStatement(List<string> RefResultCodes, ISTNodeTranslateSchemeInstance InSchemeInstanceOfStatement)
        {
            // Gather all SIs
            List<ISTNodeTranslateSchemeInstance> allSubSIs = new List<ISTNodeTranslateSchemeInstance>();
            _RecursivePrerequisite(InSchemeInstanceOfStatement, si => allSubSIs.Add(si));

            // Construct reverse Sub SIs
            var revSubSIs = new List<ISTNodeTranslateSchemeInstance>(allSubSIs);
            revSubSIs.Reverse();

            // Gather all Pre-statement codes from sub-SIs
            foreach (var si in allSubSIs)
            {
                RefResultCodes.AddRange(si.GetResult("PreStatement"));
            }

            var presentResult = InSchemeInstanceOfStatement.GetResult("Present");
            RefResultCodes.AddRange(presentResult);

            // Gather all Post-statement(Rev) codes from sub-SIs
            foreach (var si in allSubSIs)
            {
                RefResultCodes.AddRange(si.GetResult("PostStatement"));
            }
            foreach (var si in revSubSIs)
            {
                RefResultCodes.AddRange(si.GetResult("PostStatementRev"));
            }
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
                _RecursivePrerequisite(preSI, InFunc);
                InFunc(preSI);
            }
        }



        //
        // System schemes
        //

        public static string SystemScheme_Error { get; } = "SYS_ERROR";
        public static string SystemScheme_Null { get; } = "SYS_NULL";
        public static string SystemScheme_Const { get; } = "SYS_CONST";
        public static string SystemScheme_VarGet { get; } = "SYS_VAR_GET";
        public static string SystemScheme_VarSet { get; } = "SYS_VAR_SET";
        public static string SystemScheme_VarRef { get; } = "SYS_VAR_REF";
        public static string SystemScheme_BinOp { get; } = "SYS_BINOP";
        public static string SystemScheme_UnaryOp { get; } = "SYS_UNARYOP";
        public static string SystemScheme_Call { get; } = "SYS_CALL";

        public static string SystemScheme_VarAccess(EExprVarAccessType InAccessType)
        {
            switch (InAccessType)
            {
                case EExprVarAccessType.Get:
                    return SystemScheme_VarGet;
                case EExprVarAccessType.Set:
                    return SystemScheme_VarSet;
                case EExprVarAccessType.Ref:
                    return SystemScheme_VarRef;
            }
            throw new ArgumentException();
            return "<<INVALID ACCESS TYPE>>";
        }

        /// <summary>
        /// Find the best scheme fit the translating context by name.
        /// </summary>
        /// <param name="InTranslateContext"></param>
        /// <param name="InSchemeName"></param>
        /// <returns></returns>
        public abstract ISTNodeTranslateScheme FindBestScheme(ITranslatingContext InTranslatingContext, string InSchemeName);



        /// <summary>
        /// Context which preserves states for a translating process.
        /// Translating context will be organized in a tree.
        /// </summary>
        public interface ITranslatingContext
        {

            /// <summary>
            /// The root ExprTranslateEnvironment assigned by external.
            /// </summary>
            IExprTranslateEnvironment RootEnvironment { get; }

            /// <summary>
            /// The parent context of this context.
            /// </summary>
            ITranslatingContext ParentContext { get; }

            /// <summary>
            /// Element Info bound with this context.
            /// </summary>
            ElementInfo BoundElementInfo { get; }

            /// <summary>
            /// Get value string by InKey from this context.
            /// </summary>
            /// <param name="InKey"></param>
            /// <returns></returns>
            string GetContextValueString(string InKey);

        }

        /// <summary>
        /// Context for a translating block to indicate states of the block.
        /// </summary>
        public interface IBlockContext
            : ITranslatingContext
        {
            int BlockDepth { get; }

            // TODO if-block? while-block?
        }

        /// <summary>
        /// Context for a translating statement.
        /// </summary>
        public interface IStatementContext
            : ITranslatingContext
        {
            /// <summary>
            /// Root node bound with the statement.
            /// </summary>
            ISyntaxTreeNode RootNode { get; }

            // TODO if-condition-statement? or normal statement?

        }

        /// <summary>
        /// Context for a translating node.
        /// </summary>
        public interface INodeContext
            : ITranslatingContext
        {

            ISyntaxTreeNode TranslatingNode { get; }

            // TODO usage of the node (GET/SET/REF), or something like that.

        }



        ////
        //// System Schemes
        //// 

        ///// <summary>
        ///// Return the scheme when error occurs in translating.
        ///// </summary>
        ///// <param name="InErrorNode"></param>
        ///// <returns></returns>
        //protected abstract ISTNodeTranslateScheme ErrorScheme(STNodeBase InErrorNode);
        ////
        //// Constant access Schemes
        ////

        //protected abstract ISTNodeTranslateScheme QueryNullScheme(TypeInfo InConstType);

        //protected abstract ISTNodeTranslateScheme QueryConstGetInfoScheme(
        //    TypeInfo InConstType
        //    , Info InInfo
        //    );

        //protected abstract ISTNodeTranslateScheme QueryConstGetStringScheme(
        //    TypeInfo InConstType
        //    , string InString
        //    );

        //protected abstract ISTNodeTranslateScheme QueryConstGetScheme(
        //    TypeInfo InConstType
        //    , string InValueString
        //    );


        ////
        //// var access Schemes
        ////

        ///// <summary>
        ///// Find Scheme for translating member-accesses.
        ///// </summary>
        ///// <param name="InAccessType"></param>
        ///// <param name="InContextType"></param>
        ///// <param name="InMemberID"></param>
        ///// <param name="OutMemberType"></param>
        ///// <returns></returns>
        //protected abstract ISTNodeTranslateScheme QueryMemberAccessScheme(
        //    EExprVarAccessType InAccessType
        //    , TypeInfo InContextType
        //    , string InMemberID
        //    , out TypeInfo OutMemberType
        //    );

        ///// <summary>
        ///// Find Scheme for translating global-var accesses.
        ///// </summary>
        ///// <param name="InAccessType"></param>
        ///// <param name="InGlobalInfo"></param>
        ///// <param name="InVarName"></param>
        ///// <param name="OutVarType"></param>
        ///// <returns></returns>
        //protected abstract ISTNodeTranslateScheme QueryGlobalVarAccessScheme(
        //    EExprVarAccessType InAccessType
        //    , Info InGlobalInfo
        //    , string InVarName
        //    );

        ///// <summary>
        ///// Find Scheme for translating method-scope vars (parameters and locals).
        ///// </summary>
        ///// <param name="InAccessType"></param>
        ///// <param name="InTranslatingContext"></param>
        ///// <param name="InMethodInfo"></param>
        ///// <param name="InVarName"></param>
        ///// <param name="OutVarType"></param>
        ///// <returns></returns>
        //protected abstract ISTNodeTranslateScheme QueryMethodVarAccessScheme(
        //    EExprVarAccessType InAccessType
        //    , IExprTranslateEnvironment InTranslatingContext
        //    , ElementInfo InMethodInfo
        //    , string InVarName
        //    );

        ////
        //// Operation Schemes.
        ////

        ///// <summary>
        ///// Query scheme to generate Bin-Op codes.
        ///// </summary>
        ///// <param name="InBinOpNode"></param>
        ///// <param name="InLhsType"></param>
        ///// <param name="InRhsType"></param>
        ///// <param name="OutResultType"></param>
        ///// <returns></returns>
        //protected abstract ISTNodeTranslateScheme QueryBinOpScheme(
        //    STNodeBinaryOp InBinOpNode
        //    , TypeInfo InLhsType
        //    , TypeInfo InRhsType
        //    , out TypeInfo OutResultType
        //    );

        ///// <summary>
        ///// Query scheme to generate host-access codes like "Host.Member"
        ///// </summary>
        ///// <param name="InMemberAccessNode"></param>
        ///// <param name="InHostElement"></param>
        ///// <returns></returns>
        //protected abstract ISTNodeTranslateScheme QueryHostAccessScheme(
        //    STNodeMemberAccess InMemberAccessNode
        //    , TypeInfo InHostType
        //    );

    }


}
