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
        public IReadOnlyList<string> Translate(IMethodBodyContext InMethodBodyContext, ISyntaxTreeNode InSyntaxTree)
        {
            // Gather schemeInstances.
            STNodeVisitor_FunctionBody visitor = new STNodeVisitor_FunctionBody(this, InMethodBodyContext);
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
        /// Get override op code for the target context.
        /// </summary>
        /// <param name="opDef"></param>
        /// <param name="predictScope1"></param>
        /// <param name="predictScope2"></param>
        /// <returns>Return a non-empty string means we override the op code. Return an empty string if no overriding, use OpDef's default op-code in this case.</returns>
        protected virtual string GetOverrideOpCode(OpDefinition InOpDef, TypeInfo InLHSType, TypeInfo InRHSType)
        {
            return "";
        }

        /// <summary>
        /// Predict the type of a binary-operator.
        /// </summary>
        /// <param name="InOpDef"></param>
        /// <param name="InLHSType"></param>
        /// <param name="InRHSType"></param>
        /// <returns></returns>
        protected virtual TypeInfo PredictBinOpResultType(OpDefinition InOpDef, TypeInfo InLHSType, TypeInfo InRHSType)
        {
            var usage = InOpDef.Usage;
            switch (usage)
            {
                case EOpUsage.Comparer:
                    return CommonTypeInfos.Boolean;
                case EOpUsage.LOperator:
                    // TODO support overrided-operators, let the LHS/RHS to decide the result type of overrided-operators.
                    return InLHSType;
                case EOpUsage.ROperator:
                    return InRHSType;
                case EOpUsage.BooleanOperator:
                    return CommonTypeInfos.Boolean;
                case EOpUsage.BitwiseOperator:
                    return InLHSType;
                case EOpUsage.UnaryBooleanOperator:
                case EOpUsage.UnaryOperator:
                    throw new InvalidOperationException("Cannot apply bin-op actions to a unary operator");
            }
            // TODO log error
            throw new InvalidOperationException();
            return CommonTypeInfos.Unknown;
        }


        /// <summary>
        /// Context for all nodes used in expression translating.
        /// </summary>
        public interface IExprContext
            : ITranslatingContext
        {
            /// <summary>
            /// The method holds this expression context.
            /// </summary>
            public IMethodBodyContext HostMethodBody { get; }

        }

        /// <summary>
        /// Context for a translating block to indicate states of the block.
        /// </summary>
        public interface IBlockContext
            : IExprContext
        {
            int BlockDepth { get; }

            // TODO if-block? while-block?
        }

        /// <summary>
        /// Context for a translating statement.
        /// </summary>
        public interface IStatementContext
            : IExprContext
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
            : IExprContext
        {
            /// <summary>
            /// The node being translated.
            /// </summary>
            ISyntaxTreeNode TranslatingNode { get; }

            // TODO usage of the node (GET/SET/REF), or something like that.

        }

        /// <summary>
        /// Context for a translating variable-access node like STNodeVar/STNodeMemberAccess
        /// </summary>
        public interface IVariableContext
            : INodeContext
        {
            /// <summary>
            /// Info of the variable.
            /// </summary>
            ElementInfo BoundElementInfo { get; }

        }

    }


    /// <summary>
    /// Method body context
    /// </summary>
    public interface IMethodBodyContext
        : ITranslatingContext
    {
        /// <summary>
        /// Name of the method
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// Expression translate environment created for this context.
        /// </summary>
        IExprTranslateEnvironment RootEnvironment { get; }

    }

    /// <summary>
    /// Context of the 'Root' body.
    /// Root body is a special body with non-parent.
    /// </summary>
    public class FuncBodyContext
        : TranslatingContextBase
        , IMethodBodyContext
    {
        public FuncBodyContext(ITranslatingContext InParentContext, string InMethodName, IExprTranslateEnvironment InEnvironment)
            : base(InParentContext)
        {
            RootEnvironment = InEnvironment;
            MethodName = InMethodName;
        }
        public FuncBodyContext(ITranslatingContext InParentContext, ElementInfo InBoundMethodInfo, IExprTranslateEnvironment InEnvironment)
            : base(InParentContext)
        {
            MethodInfo = InBoundMethodInfo;
            MethodName = MethodInfo.Name;
            RootEnvironment = InEnvironment;
        }

        public ElementInfo MethodInfo { get; }

        // Begin ITranslatingContext interfaces
        public override string GetContextValueString(string InKey)
        {
            // TODO return value string registered in the root environment.
            return base.GetContextValueString(InKey);
        }
        // ~ End ITranslatingContext interfaces

        // Begin IMethodBodyContext interfaces
        public string MethodName { get; }
        public IExprTranslateEnvironment RootEnvironment { get; }
        // ~ End IMethodBodyContext interfaces

    }




}
