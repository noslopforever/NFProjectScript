using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


namespace nf.protoscript.translator.expression
{
    /// <summary>
    /// Default implementation of ExprTranslateContext
    /// </summary>
    public class ExprTranslateContextDefault
        : IExprTranslateContext
    {
        public ExprTranslateContextDefault(
            Info InHostInfo
            , IEnumerable<Scope> InScopeChain
            )
        {
            HostInfo = InHostInfo;
            ScopeChain = InScopeChain;
        }

        /// <summary>
        /// scope to find variables.
        /// </summary>
        public class Scope
            : IExprTranslateContext.IScope
        {

            public Scope(Info InScopeInfo
                , string InScopeHostName
                , string InScopeHostPresent
                )
            {
                ScopeInfo = InScopeInfo;
                ScopeName = InScopeHostName;
                ScopePresentCode = InScopeHostPresent;
            }

            /// <summary>
            /// Info of the scope
            /// </summary>
            public Info ScopeInfo { get; }

            /// <summary>
            /// Host Name of the scope, <see cref="IExprTranslateContext.IVariable"/>  for more informations.
            /// </summary>
            public string ScopeName { get; }

            /// <summary>
            /// Host Present of the scope, <see cref="IExprTranslateContext.IVariable"/>  for more informations.
            /// </summary>
            public string ScopePresentCode { get; }

        }

        /// <summary>
        /// Variable from ElementInfo
        /// </summary>
        public class ElementInfoVar
            : IExprTranslateContext.IVariable
        {
            public ElementInfoVar(ElementInfo InVarElement, Scope InScope)
            {
                VarElement = InVarElement;
                HostScope = InScope;
            }

            public ElementInfo VarElement { get; }

            // Begin IVariable interfaces
            public string Name { get { return VarElement.Name; } }
            public TypeInfo VarType { get { return VarElement.ElementType; } }
            public IExprTranslateContext.IScope HostScope { get; }
            // ~ End IVariable interfaces.
        }

        public Info HostInfo { get; }
        public IEnumerable<Scope> ScopeChain { get; }

        public IExprTranslateContext.IVariable FindVariable(string InName)
        {
            // TODO Find local vars first

            // Find in scope chain.
            foreach (Scope scope in ScopeChain)
            {
                if (scope.ScopeInfo is TypeInfo)
                {
                    var propertyInfo = InfoHelper.FindPropertyOfType(scope.ScopeInfo as TypeInfo, InName);
                    if (propertyInfo != null)
                    {
                        return new ElementInfoVar(propertyInfo, scope);
                    }
                }

                // Try find archetype and global variables.
                if (scope.ScopeInfo is ElementInfo)
                {
                    var varInfo = InfoHelper.FindPropertyInArchetype(scope.ScopeInfo as ElementInfo, InName);
                    if (varInfo != null)
                    {
                        return new ElementInfoVar(varInfo, scope);
                    }
                }

                if (scope.ScopeInfo is ProjectInfo)
                {
                    var globalInfo = InfoHelper.FindPropertyInGlobal(scope.ScopeInfo as ProjectInfo, InName);
                    if (globalInfo != null)
                    {
                        return new ElementInfoVar(globalInfo, scope);
                    }
                }
            }

            return null;
        }

        public IExprTranslateContext.IVariable AddTempVar(ISyntaxTreeNode InNodeToTranslate, string InKey)
        {
            string nodeTypeName = InNodeToTranslate.GetType().Name;
            int uniqueID = _localScope.TempVarTable.Count;

            string uniqueTempVarName = $"TMP_{nodeTypeName}_{InKey}_{uniqueID}";
            var tempVar = new TempVar(_localScope, uniqueTempVarName, InNodeToTranslate, InKey);

            _localScope.AddTempVar(InNodeToTranslate, InKey, tempVar);
            return tempVar;
        }

        public IExprTranslateContext.IVariable EnsureTempVar(ISyntaxTreeNode InNodeToTranslate, string InKey)
        {
            if (_localScope.TryGetTempVar(InNodeToTranslate, InKey, out var tempVar))
            {
                return tempVar;
            }
            return AddTempVar(InNodeToTranslate, InKey);
        }


        /// <summary>
        /// The Special 'Local' Scope of the context to save local and temp variables.
        /// It should always be the first scope when finding variables.
        /// </summary>
        internal class LocalScope
            : IExprTranslateContext.IScope
        {
            internal LocalScope()
            {}

            // Begin IExprTranslateContext.IScope interfaces
            public Info ScopeInfo { get { return null; } }
            public string ScopeName { get { return "ContextLocal"; } }
            public string ScopePresentCode { get { return ""; } }
            // ~ End IExprTranslateContext.IScope interfaces

            /// <summary>
            /// Key to find a temp var.
            /// </summary>
            public struct TempVarKey
            {
                public ISyntaxTreeNode Node;
                public string Key;
            }

            /// <summary>
            /// Temp var table
            /// </summary>
            public IReadOnlyDictionary<TempVarKey, TempVar> TempVarTable { get { return _tempVarTable; } }

            /// <summary>
            /// Register one temp var to the table.
            /// </summary>
            /// <param name="InKey"></param>
            /// <param name="InTempVar"></param>
            internal void AddTempVar(ISyntaxTreeNode InNode, string InKey, TempVar InTempVar)
            {
                _tempVarTable.Add(new TempVarKey() { Node = InNode, Key = InKey }, InTempVar);
            }

            /// <summary>
            /// Try get one temp var from the table.
            /// </summary>
            /// <param name="InNodeToTranslate"></param>
            /// <param name="InKey"></param>
            /// <param name="OutTempVar"></param>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            internal bool TryGetTempVar(ISyntaxTreeNode InNode, string InKey, out TempVar OutTempVar)
            {
                return _tempVarTable.TryGetValue(new TempVarKey() { Node = InNode, Key = InKey }, out OutTempVar);
            }

            // Temporary variables table.
            Dictionary<TempVarKey, TempVar> _tempVarTable = new Dictionary<TempVarKey, TempVar>();

        }

        // the local scope bound with this context
        LocalScope _localScope = new LocalScope();

        /// <summary>
        /// Temporary variable registered in this context.
        /// </summary>
        public class TempVar
            : IExprTranslateContext.IVariable
        {
            internal TempVar(
                LocalScope InLocalScope
                , string InVarName
                , ISyntaxTreeNode InBoundNode
                , string InBoundNodeKey
                )
            {
                HostScope = InLocalScope;
                Name = InVarName;
                VarType = CommonTypeInfos.Any;
                BoundNode = InBoundNode;
                BoundNodeKey = InBoundNodeKey;
            }

            // Begin IVariable interfaces
            public string Name { get; }
            public TypeInfo VarType { get; }
            public IExprTranslateContext.IScope HostScope { get; }
            // ~ End IVariable interfaces.

            /// <summary>
            /// Node bound with the temp-var
            /// </summary>
            public ISyntaxTreeNode BoundNode { get; }

            /// <summary>
            /// Key bound with the temp-var
            /// </summary>
            public string BoundNodeKey { get; }

        }

    }


}
