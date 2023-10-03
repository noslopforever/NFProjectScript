using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Xml.Linq;


namespace nf.protoscript.translator.expression
{
    /// <summary>
    /// Default implementation of ExprTranslateEnvironment
    /// </summary>
    public class ExprTranslateEnvironmentDefault
        : IExprTranslateEnvironment
    {
        public ExprTranslateEnvironmentDefault(
            Info InHostInfo
            , IEnumerable<ScopeBase> InScopeChain
            )
        {
            HostInfo = InHostInfo;
            ScopeChain = InScopeChain;
        }

        /// <summary>
        /// Base of all Scopes provided by this Env class .
        /// </summary>
        public abstract class ScopeBase
            : IExprTranslateEnvironment.IScope
        {
            public ScopeBase(string InScopeHostName, string InScopeHostPresent)
            {
                ScopeName = InScopeHostName;
                ScopePresentCode = InScopeHostPresent;
            }
            // Begin IScope interfaces
            public string ScopeName { get; }
            public string ScopePresentCode { get; }
            // ~ End IScope interfaces

            internal IExprTranslateEnvironment.IVariable _FindVarInScope(string InName)
                => FindVarInScope(InName);

            /// <summary>
            /// Try find a variable in this scope.
            /// </summary>
            /// <kvp InName="InName"></kvp>
            /// <returns></returns>
            protected abstract IExprTranslateEnvironment.IVariable FindVarInScope(string InName);

        }

        /// <summary>
        /// scope bound with an info to find variables.
        /// </summary>
        /// TODO Exact Scope to GlobalScope, ThisTypeScope, MethodScope etc...
        public class Scope
            : ScopeBase
            , IExprTranslateEnvironment.IInfoScope
        {
            public Scope(Info InScopeInfo, string InScopeHostName, string InScopeHostPresent)
                :base(InScopeHostName, InScopeHostPresent)
            {
                ScopeInfo = InScopeInfo;
            }

            // Begin IInfoScope interfaces
            public Info ScopeInfo { get; }
            // ~ End IInfoScope interfaces

            protected override IExprTranslateEnvironment.IVariable FindVarInScope(string InName)
            {
                if (ScopeInfo is TypeInfo)
                {
                    var propertyInfo = InfoHelper.FindPropertyOfType(ScopeInfo as TypeInfo, InName);
                    if (propertyInfo != null)
                    {
                        return new ElementInfoVar(propertyInfo, this);
                    }
                }
                // Try find archetype and global variables.
                else if (ScopeInfo is ElementInfo)
                {
                    var varInfo = InfoHelper.FindPropertyInArchetype(ScopeInfo as ElementInfo, InName);
                    if (varInfo != null)
                    {
                        return new ElementInfoVar(varInfo, this);
                    }
                }
                // Try find variable in the global scope
                else if (ScopeInfo is ProjectInfo)
                {
                    var globalInfo = InfoHelper.FindPropertyInGlobal(ScopeInfo as ProjectInfo, InName);
                    if (globalInfo != null)
                    {
                        return new ElementInfoVar(globalInfo, this);
                    }
                }
                return null;
            }

        }

        /// <summary>
        /// Variable from ElementInfo
        /// </summary>
        public class ElementInfoVar
            : IExprTranslateEnvironment.IVariable
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
            public IExprTranslateEnvironment.IScope HostScope { get; }
            public ElementInfo ElementInfo { get { return VarElement; } }
            // ~ End IVariable interfaces.
        }

        public Info HostInfo { get; }
        public IEnumerable<ScopeBase> ScopeChain { get; }

        public IExprTranslateEnvironment.IVariable FindVariable(string InName)
        {
            // TODO Find local vars first

            // Find in scope chain.
            foreach (ScopeBase scope in ScopeChain)
            {
                var var = scope._FindVarInScope(InName);
                if (var != null)
                    return var;
            }

            return null;
        }

        /// <summary>
        /// A virtual scope which was not bound with Infos.
        /// </summary>
        public class VirtualMethodScope
            : ScopeBase
        {
            public VirtualMethodScope(
                string InScopeName
                , string InScopeHostPresent
                , Dictionary<string, TypeInfo> InScopeParamWithTypes = null
                )
                : base(InScopeName, InScopeHostPresent)
            {
                _paramWithTypes = new Dictionary<string, VirtualVar>();
                if ( InScopeParamWithTypes != null)
                {
                    foreach ( var kvp in InScopeParamWithTypes )
                    {
                        var varName = kvp.Key;
                        var varType = kvp.Value;
                        _paramWithTypes.Add(varName, new VirtualVar(varName, varType, this));
                    }
                }
            }

            /// <summary>
            /// Parameters registered manually.
            /// </summary>
            Dictionary<string, VirtualVar> _paramWithTypes;

            protected override IExprTranslateEnvironment.IVariable FindVarInScope(string InName)
            {
                if (_paramWithTypes.TryGetValue( InName, out var virtualVar))
                {
                    return virtualVar;
                }
                return null;
            }
        }

        /// <summary>
        /// A virtual variable which is not bound with an ElementInfo.
        /// </summary>
        public class VirtualVar
            : IExprTranslateEnvironment.IVariable
        {
            public VirtualVar(string InName, TypeInfo InType, IExprTranslateEnvironment.IScope InScope)
            {
                Name = InName;
                VarType = InType;
                HostScope = InScope;
            }

            // Begin IVariable interfaces
            public string Name { get; }
            public TypeInfo VarType { get; }
            public IExprTranslateEnvironment.IScope HostScope { get; }
            public ElementInfo ElementInfo => null;
            // ~ End IVariable interfaces
        }

        public IExprTranslateEnvironment.IVariable AddTempVar(ISyntaxTreeNode InNodeToTranslate, string InKey)
        {
            int uniqueID = _localScope.TempVarTable.Count;

            //string nodeTypeName = "Null";
            //if (InNodeToTranslate != null)
            //{
            //    nodeTypeName = InNodeToTranslate.GetType().Name;
            //}
            //string uniqueTempVarName = $"TMP_{nodeTypeName}_{InKey}_{uniqueID}";
            string uniqueTempVarName = $"TMP_{InKey}{uniqueID}";
            var tempVar = new TempVar(_localScope, uniqueTempVarName, InNodeToTranslate, InKey);

            _localScope.AddTempVar(InNodeToTranslate, InKey, tempVar);
            return tempVar;
        }

        public IExprTranslateEnvironment.IVariable EnsureTempVar(ISyntaxTreeNode InNodeToTranslate, string InKey)
        {
            if (_localScope.TryGetTempVar(InNodeToTranslate, InKey, out var tempVar))
            {
                return tempVar;
            }
            return AddTempVar(InNodeToTranslate, InKey);
        }


        /// <summary>
        /// The Special 'Local' Scope of the environment to save local and temp variables.
        /// It should always be the first scope when finding variables.
        /// </summary>
        internal class LocalScope
            : IExprTranslateEnvironment.IScope
        {
            internal LocalScope()
            {}

            // Begin IExprTranslateEnvironment.IScope interfaces
            public string ScopeName { get { return "Local"; } }
            public string ScopePresentCode { get { return ""; } }
            public IExprTranslateEnvironment.IVariable FindVariable(string InVarName)
            {
                throw new NotImplementedException();
            }
            // ~ End IExprTranslateEnvironment.IScope interfaces

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
            /// <kvp InName="InKey"></kvp>
            /// <kvp InName="InTempVar"></kvp>
            internal void AddTempVar(ISyntaxTreeNode InNode, string InKey, TempVar InTempVar)
            {
                _tempVarTable.Add(new TempVarKey() { Node = InNode, Key = InKey }, InTempVar);
            }

            /// <summary>
            /// Try get one temp var from the table.
            /// </summary>
            /// <kvp InName="InNodeToTranslate"></kvp>
            /// <kvp InName="InKey"></kvp>
            /// <kvp InName="OutTempVar"></kvp>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            internal bool TryGetTempVar(ISyntaxTreeNode InNode, string InKey, out TempVar OutTempVar)
            {
                return _tempVarTable.TryGetValue(new TempVarKey() { Node = InNode, Key = InKey }, out OutTempVar);
            }

            // Temporary variables table.
            Dictionary<TempVarKey, TempVar> _tempVarTable = new Dictionary<TempVarKey, TempVar>();

        }

        // the local scope bound with this environment
        LocalScope _localScope = new LocalScope();

        /// <summary>
        /// Temporary variable registered in this environment.
        /// </summary>
        public class TempVar
            : IExprTranslateEnvironment.IVariable
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
            public IExprTranslateEnvironment.IScope HostScope { get; }
            public ElementInfo ElementInfo { get { return null; } }
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
