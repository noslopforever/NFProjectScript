using nf.protoscript.syntaxtree;
using System;
using System.Collections;
using System.Security.Cryptography;

namespace nf.protoscript.translator.expression
{


    public partial class ExprTranslatorAbstract
    {


        /// <summary>
        /// Base of all Contexts based on expression nodes.
        /// </summary>
        public abstract class ExprContextBase
            : TranslatingContextBase
            , IExprContext
        {
            public ExprContextBase(IMethodBodyContext InParentContext)
                : base(InParentContext)
            {
                HostMethodBody = InParentContext;
            }
            public ExprContextBase(ExprContextBase InParentContext)
                : base(InParentContext)
            {
                HostMethodBody = InParentContext.HostMethodBody;
            }

            /// <summary>
            /// Root Environment inherit from the last HostMethodBody parent.
            /// </summary>
            public IMethodBodyContext HostMethodBody { get; }

        }

        /// <summary>
        /// Context of a statement.
        /// </summary>
        public class StatementContext
            : ExprContextBase
        {

            public StatementContext(IMethodBodyContext InParentContext, ISyntaxTreeNode InNode)
                : base(InParentContext)
            {
                StatementRoot = InNode;
            }
            public StatementContext(ExprContextBase InParentContext, ISyntaxTreeNode InNode)
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
            : ExprContextBase
            , INodeContext
        {
            public NodeContextBase(ExprContextBase InParentContext, ISyntaxTreeNode InNode)
                : base(InParentContext)
            {
                TranslatingNode = InNode;
            }

            /// <summary>
            /// Node bound with this context.
            /// </summary>
            public ISyntaxTreeNode TranslatingNode { get; }

            public override bool TryGetContextValue(string InKey, out object OutValue)
            {
                try
                {
                    var keyProp = TranslatingNode.GetType().GetProperty(InKey);
                    if (keyProp != null)
                    {
                        OutValue = keyProp.GetValue(TranslatingNode);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // TODO log error
                }
                return base.TryGetContextValue(InKey, out OutValue);
            }

        }

        /// <summary>
        /// Context with a common STNode.
        /// </summary>
        public class OnlyNodeContext
            : NodeContextBase
        {
            public OnlyNodeContext(ExprContextBase InParentContext, ISyntaxTreeNode InNode)
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
            public ConstNodeContext(ExprContextBase InParentContext, ISyntaxTreeNode InNode)
                : base(InParentContext, InNode)
            {
            }

            public static ConstNodeContext Const(ExprContextBase InParentContext, STNodeConstant InConst, string InValue)
            {
                return new ConstNodeContext(InParentContext, InConst) { Value = InValue, ValueString = InValue };
            }

            public static ConstNodeContext Const(ExprContextBase InParentContext, STNodeConstant InConst, Info InInfo)
            {
                return new ConstNodeContext(InParentContext, InConst) { Value = InInfo, ValueString = InInfo.ToString() };
            }

            public static ConstNodeContext Null(ExprContextBase InParentContext, STNodeConstant InConst)
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
        /// Context when translating a bin-op or unary-op node
        /// </summary>
        public class OpContext
            : NodeContextBase
        {
            public OpContext(ExprContextBase InParentContext, STNodeBinaryOp InNode, string InOverrideOpCode = "")
                : base(InParentContext, InNode)
            {
                OpCode = InNode.OpDef.DefaultOpCode != null ? InNode.OpDef.DefaultOpCode : "<<INVALID OP CODE>>";
                if (InOverrideOpCode != "")
                {
                    OpCode = InOverrideOpCode;
                }
            }
            public OpContext(ExprContextBase InParentContext, STNodeUnaryOp InNode, string InOverrideOpCode = "")
                : base(InParentContext, InNode)
            {
                OpCode = InNode.OpDef.DefaultOpCode != null ? InNode.OpDef.DefaultOpCode : "<<INVALID OP CODE>>";
                if (InOverrideOpCode != "")
                {
                    OpCode = InOverrideOpCode;
                }
            }

            /// <summary>
            /// OpCode
            /// </summary>
            public string OpCode { get; }

        }

        /// <summary>
        /// Context when translating a variable-access node.
        /// </summary>
        public class VarContext
            : NodeContextBase
            , IVariableContext
        {
            public VarContext(ExprContextBase InParentContext
                , STNodeVar InNode
                , IExprTranslateEnvironment.IVariable InHostScopeVar
                ) : base(InParentContext, InNode)
            {
                BoundElementInfo = InHostScopeVar.ElementInfo;
                _variable = InHostScopeVar;
            }

            // Begin IVariableContext interfaces
            public ElementInfo BoundElementInfo { get; }
            // ~ End IVariableContext interfaces

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
        /// Context when translating a STNodeInit.
        /// </summary>
        public class InitContext
            : NodeContextBase
            , IVariableContext
        {
            public InitContext(ExprContextBase InParentContext
                , STNodeMemberInit InNode
                , IExprTranslateEnvironment.IVariable InHostScopeVar
                ) : base(InParentContext, InNode)
            {
                BoundElementInfo = InHostScopeVar.ElementInfo;
                _variable = InHostScopeVar;
            }

            // Begin IVariableContext interfaces
            public ElementInfo BoundElementInfo { get; }
            // ~ End IVariableContext interfaces

            /// <summary>
            /// InitNode bound with this context.
            /// </summary>
            public STNodeMemberInit InitNode { get { return TranslatingNode as STNodeMemberInit; } }

            //
            // Context variables.
            //

            /// <summary>
            /// Var Name registered to the context.
            /// </summary>
            public string VarName { get { return InitNode.InfoToBeInit.Name; } }

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
            , IVariableContext
        {
            public MemberContext(ExprContextBase InParentContext, STNodeMemberAccess InNode, ElementInfo InBoundElementInfo)
                : base(InParentContext, InNode)
            {
                BoundElementInfo = InBoundElementInfo;
            }

            // Begin IVariableContext interfaces
            public ElementInfo BoundElementInfo { get; }
            // ~ End IVariableContext interfaces

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
            public string VarName { get { return MemberAccessNode.IDName; } }

        }

    }



}
