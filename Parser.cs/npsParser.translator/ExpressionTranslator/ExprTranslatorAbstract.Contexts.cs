using nf.protoscript.syntaxtree;
using System;
using System.Collections;
using System.Security.Cryptography;
using static nf.protoscript.translator.expression.ExprTranslatorAbstract;

namespace nf.protoscript.translator.expression
{


    public partial class ExprTranslatorAbstract
    {

        /// <summary>
        /// Context of the 'Root' body.
        /// Root body is a special body with non-parent.
        /// </summary>
        public class FuncBodyContext
            : ITranslatingContext
        {
            public FuncBodyContext(IExprTranslateEnvironment InEnvironment)
            {
                RootEnvironment = InEnvironment;
            }

            public IExprTranslateEnvironment RootEnvironment { get; }
            public ITranslatingContext ParentContext { get { return null; } }
            public virtual ElementInfo BoundElementInfo { get { return null; } }

            public string GetContextValueString(string InKey)
            {
                // TODO return value string registered in the root environment.
                return $"<<NULL VALUE for {InKey}>>";
            }

        }


        /// <summary>
        /// Base of all Contexts implemented in the project.
        /// </summary>
        public abstract class ContextBase
           : ITranslatingContext
        {
            public ContextBase(ITranslatingContext InParentContext)
            {
                RootEnvironment = InParentContext.RootEnvironment;
                ParentContext = InParentContext;
            }

            public IExprTranslateEnvironment RootEnvironment { get; }
            public ITranslatingContext ParentContext { get; }
            public virtual ElementInfo BoundElementInfo { get { return ParentContext?.BoundElementInfo; } }

            public virtual string GetContextValueString(string InKey)
            {
                try
                {
                    var keyProp = GetType().GetProperty(InKey);
                    if (keyProp != null)
                    {
                        var propVal = keyProp.GetValue(this).ToString();
                        return propVal;
                    }
                }
                catch (Exception ex)
                {
                    // TODO log error
                }

                return ParentContext?.GetContextValueString(InKey);
            }

        }

        /// <summary>
        /// Context of a statement.
        /// </summary>
        public class StatementContext
            : ContextBase
        {

            public StatementContext(ITranslatingContext InParentContext, ISyntaxTreeNode InNode)
                : base(InParentContext)
            {
                StatementRoot = InNode;
            }

            public ISyntaxTreeNode StatementRoot { get; }

        }

        /// <summary>
        /// Base of all Context bound with a syntax tree node.
        /// </summary>
        public abstract class NodeContextBase
            : ContextBase
            , INodeContext
        {
            public NodeContextBase(ITranslatingContext InParentContext, ISyntaxTreeNode InNode)
                : base(InParentContext)
            {
                TranslatingNode = InNode;
            }

            /// <summary>
            /// Node bound with this context.
            /// </summary>
            public ISyntaxTreeNode TranslatingNode { get; }

            public override string GetContextValueString(string InKey)
            {
                try
                {
                    var keyProp = TranslatingNode.GetType().GetProperty(InKey);
                    if (keyProp != null)
                    {
                        var propVal = keyProp.GetValue(TranslatingNode).ToString();
                        return propVal;
                    }
                }
                catch (Exception ex)
                {
                    // TODO log error
                }
                return base.GetContextValueString(InKey);
            }

        }

        /// <summary>
        /// Context with a common STNode.
        /// </summary>
        public class OnlyNodeContext
            : NodeContextBase
        {
            public OnlyNodeContext(ITranslatingContext InParentContext, ISyntaxTreeNode InNode)
                : base(InParentContext, InNode)
            {
            }
        }

        /// <summary>
        /// Context when translating a common or constant node.
        /// </summary>
        public class ConstNodeContext
            : NodeContextBase
        {
            public ConstNodeContext(ITranslatingContext InParentContext, ISyntaxTreeNode InNode)
                : base(InParentContext, InNode)
            {
            }

            public static ITranslatingContext Const(ITranslatingContext InParentContext, STNodeConstant InConst, string InValue)
            {
                return new ConstNodeContext(InParentContext, InConst) { Value = InValue, ValueString = InValue };
            }

            public static ITranslatingContext Const(ITranslatingContext InParentContext, STNodeConstant InConst, Info InInfo)
            {
                return new ConstNodeContext(InParentContext, InConst) { Value = InInfo, ValueString = InInfo.ToString() };
            }

            public static ITranslatingContext Null(ITranslatingContext InParentContext, STNodeConstant InConst)
            {
                return new ConstNodeContext(InParentContext, InConst) { Value = null, ValueString = "null" };
            }

            /// <summary>
            /// ValueType of the constant node.
            /// </summary>
            public TypeInfo ValueType { get { return (TranslatingNode as STNodeConstant).Type; } }

            /// <summary>
            /// ValueString registered to the context.
            /// </summary>
            public string ValueString { get; private set; }

            /// <summary>
            /// Value of the constant node.
            /// </summary>
            public object Value { get; private set; }

            /// <summary>
            /// Is this constant null?
            /// </summary>
            public bool IsNull { get { return Value == null; } }

        }

        /// <summary>
        /// Context when translating a variable-access node.
        /// </summary>
        public class VarContext
            : NodeContextBase
        {
            public VarContext(ITranslatingContext InParentContext
                , STNodeVar InNode
                , IExprTranslateEnvironment.IVariable InHostScopeVar
                ) : base(InParentContext, InNode)
            {
                BoundElementInfo = InHostScopeVar.ElementInfo;
                _variable = InHostScopeVar;
            }

            public override ElementInfo BoundElementInfo { get; }

            /// <summary>
            /// VarNode bound with this context.
            /// </summary>
            public STNodeVar VarNode { get { return TranslatingNode as STNodeVar; } }

            //
            // Context variables.
            //

            /// <summary>
            /// Var Name registered to the context.
            /// </summary>
            public string VarName { get { return VarNode.IDName; } }

            /// <summary>
            /// Host Present Code defined by the scope of the variable.
            /// </summary>
            public string HostPresent { get { return _variable.HostScope.ScopePresentCode; } }

            // variable
            IExprTranslateEnvironment.IVariable _variable;

        }

        /// <summary>
        /// Context when translating a member-access node
        /// </summary>
        public class MemberContext
            : NodeContextBase
        {
            public MemberContext(ITranslatingContext InParentContext, STNodeMemberAccess InNode, ElementInfo InBoundElementInfo)
                : base(InParentContext, InNode)
            {
                BoundElementInfo = InBoundElementInfo;
            }

            public override ElementInfo BoundElementInfo { get; }

            /// <summary>
            /// Node bound with this context.
            /// </summary>
            public STNodeMemberAccess MemberAccessNode { get { return TranslatingNode as STNodeMemberAccess; } }

            //
            // Context variables.
            //

            /// <summary>
            /// Var Name registered to the context.
            /// </summary>
            public string VarName { get { return MemberAccessNode.MemberID; } }

        }

    }



}
