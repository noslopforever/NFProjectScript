using nf.protoscript.syntaxtree;
using nf.protoscript.translator.DefaultScheme;
using nf.protoscript.translator.DefaultScheme.Elements;
using nf.protoscript.translator.DefaultScheme.Elements.Internal;
using nf.protoscript.translator.SchemeSelectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{
    /// <summary>
    /// Represents a translator that uses default schemes and selectors for translation.
    /// </summary>
    public class InfoTranslatorDefault : InfoTranslatorAbstract
    {
        public InfoTranslatorDefault()
        {
            // Begin Default selector for expression nodes
            AddExprScheme<STNodeConstant>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalConstant()"))
                );
            AddExprSelector<STNodeConstant>("Get", 0
                , expr => { return expr.ValueType == CommonTypeInfos.String || expr.ValueType == CommonTypeInfos.TypeRef; }
                , new InfoTranslateSchemeDefault(new ElementExpr("EvalConstString()"))
                );
            AddExprScheme<STNodeBinaryOp>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalBinOp()"))
                );
            AddExprScheme<STNodeUnaryOp>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalUnaryOp()"))
                );
            AddExprScheme<STNodeAssign>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalAssign()"))
                );
            AddExprScheme<STNodeVar>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalVar()"))
                );
            AddExprScheme<STNodeMemberAccess>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalMemberAccess()"))
                );
            AddExprScheme<STNodeCall>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalCall()"))
                );
            AddExprScheme<STNodeCollectionAccess>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("EvalCollAccess()"))
                );
            AddExprScheme<STNodeSequence>("Get", 0, new InfoTranslateSchemeDefault(
                new ElementExpr("ListSequence()"))
                );
            // ~ End Default selector for expression nodes

            // Constant: ${ValueString}
            AddScheme("EvalConstant", new InfoTranslateSchemeDefault(
                new ElementExpr("ValueString")
                ));

            // Constant<string>: "${ValueString}"
            AddScheme("EvalConstString", new InfoTranslateSchemeDefault(
                ElementParser.ParseElements("\"${ValueString}\"")
                ));

            // BinaryOp: ${LHS.Get()} ${OpCode} ${RHS.Get()}
            AddScheme("EvalBinOp", new InfoTranslateSchemeDefault(
                ElementParser.ParseElements("${LHS.Get()} ${OpCode} ${RHS.Get()}"))
                );

            // UnaryOp: ${OpCode} ${RHS.Get()}
            AddScheme("EvalUnaryOp", new InfoTranslateSchemeDefault(
                ElementParser.ParseElements("${OpCode}${RHS.Get()}"))
                );

            // Call: ${FuncExpr.Get()}(${For("Param", ", ", "Get")}
            AddScheme("EvalCall"
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${FuncExpr.Get()}(${For("Param", ", ", "Get")})
                        """
                        )
                    )
                );

            // Collection access: ${CollExpr.Get()}[${$For("Param", "][", "Get")}]
            AddScheme("EvalCollAccess"
                , new InfoTranslateSchemeDefault(
                    ElementParser.ParseElements(
                        """
                        ${CollExpr.Get()}[${For("Param", "][", "Get")}]
                        """
                        )
                    )
                );

            // Sequence: ${"For('', $NL, 'Get')"}
            AddScheme("ListSequence", new InfoTranslateSchemeDefault(
                ElementParser.ParseElements("${For('', $NL, 'Get')}"))
                );

        }

        /// <summary>
        /// Finds the best scheme for the given translating context and scheme name.
        /// </summary>
        /// <param name="InTranslatingContext">The translating context.</param>
        /// <param name="InSchemeName">The name of the scheme to find.</param>
        /// <returns>The best matching scheme, or null if none found.</returns>
        public override IInfoTranslateScheme FindBestScheme(ITranslatingContext InTranslatingContext, string InSchemeName)
        {
            if (_schemeGroups.TryGetValue(InSchemeName, out var schemeGroup))
            {
                return schemeGroup.FindBestScheme(InTranslatingContext);
            }
            return null;
        }

        /// <summary>
        /// Adds a generic scheme to the translator.
        /// </summary>
        /// <param name="InKey">The unique identifier for the scheme group.</param>
        /// <param name="InScheme">The scheme to add.</param>
        public void AddScheme(string InKey, IInfoTranslateScheme InScheme)
        {
            EnsureGroup(InKey).DefaultScheme = InScheme;
        }

        /// <summary>
        /// Adds a scheme selector to the translator.
        /// </summary>
        /// <param name="InKey">The unique identifier for the scheme group.</param>
        /// <param name="InSchemeSelector">The scheme selector to add.</param>
        public void AddSelector(string InKey, IInfoTranslateSchemeSelector InSchemeSelector)
        {
            EnsureGroup(InKey).AddSelector(InSchemeSelector);
        }

        /// <summary>
        /// Adds a scheme for expressions (syntax tree nodes) using a generic type.
        /// </summary>
        /// <typeparam name="T">The type of the syntax tree node.</typeparam>
        /// <param name="InKey">The unique identifier for the scheme group.</param>
        /// <param name="InPriority">The priority of the scheme. The higher the priority, the earlier the scheme is applied during the translation process.</param>
        /// <param name="InScheme">The scheme to add.</param>
        public void AddExprScheme<T>(string InKey, int InPriority, IInfoTranslateScheme InScheme)
            where T : ISyntaxTreeNode
        {
            AddSelector(InKey, new TranslateSchemeSelector_Lambda(InPriority,
                ctx =>
                {
                    var exprCtx = ctx as ITranslatingExprContext;
                    if (exprCtx != null && exprCtx.TranslatingExprNode != null)
                    {
                        return exprCtx.TranslatingExprNode is T;
                    }
                    return false;
                },
                InScheme
                ));
        }

        /// <summary>
        /// Adds a scheme for expressions (syntax tree nodes) using a custom selector.
        /// </summary>
        /// <typeparam name="T">The type of the syntax tree node.</typeparam>
        /// <param name="InKey">The unique identifier for the scheme group.</param>
        /// <param name="InPriority">The priority of the scheme.</param>
        /// <param name="InSelector">The selector function for the syntax tree node.</param>
        /// <param name="InScheme">The scheme to add.</param>
        public void AddExprSelector<T>(string InKey, int InPriority, Func<T, bool> InSelector, IInfoTranslateScheme InScheme)
            where T : class, ISyntaxTreeNode
        {
            AddSelector(InKey, new TranslateSchemeSelector_Lambda(InPriority,
                ctx =>
                {
                    var exprCtx = ctx as ITranslatingExprContext;
                    if (exprCtx != null && exprCtx.TranslatingExprNode != null)
                    {
                        T exprNode = exprCtx.TranslatingExprNode as T;
                        if (exprNode != null)
                        {
                            return InSelector(exprNode);
                        }
                    }
                    return false;
                },
                InScheme
                ));
        }

        /// <summary>
        /// Ensures a scheme group exists for the given scheme name.
        /// </summary>
        /// <param name="InSchemeName">The name of the scheme.</param>
        /// <returns>The scheme group for the given scheme name.</returns>
        protected SchemeGroup EnsureGroup(string InSchemeName)
        {
            if (_schemeGroups.TryGetValue(InSchemeName, out var group))
            {
                return group;
            }
            var newGrp = new SchemeGroup(InSchemeName);
            _schemeGroups.Add(InSchemeName, newGrp);
            return newGrp;
        }

        /// <summary>
        /// Represents a group of schemes with the same name but different trigger conditions (selectors).
        /// </summary>
        public class SchemeGroup
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SchemeGroup"/> class.
            /// </summary>
            /// <param name="InSchemeName">The name of the scheme group.</param>
            internal SchemeGroup(string InSchemeName)
            {
                SchemeName = InSchemeName;
            }

            /// <summary>
            /// Gets the name of the scheme group.
            /// </summary>
            public string SchemeName { get; }

            /// <summary>
            /// Gets or sets the default scheme. Only one default scheme can be assigned per group.
            /// </summary>
            /// <exception cref="InvalidOperationException">Thrown if the default scheme is set twice.</exception>
            public IInfoTranslateScheme DefaultScheme
            {
                get { return _default; }
                internal set
                {
                    if (_default != null)
                    {
                        // TODO: Implement logging
                        throw new InvalidOperationException("Cannot set default scheme twice!");
                    }
                    _default = value;
                }
            }
            private IInfoTranslateScheme _default = null;

            /// <summary>
            /// Adds a selector to the group.
            /// </summary>
            /// <param name="InSelector">The scheme selector to add.</param>
            internal void AddSelector(IInfoTranslateSchemeSelector InSelector)
            {
                _selectors.Add(InSelector.Priority, InSelector);
            }

            /// <summary>
            /// Finds the best scheme to match the given context.
            /// </summary>
            /// <param name="InContext">The translating context.</param>
            /// <returns>The best matching scheme, or the default scheme if none matched.</returns>
            internal IInfoTranslateScheme FindBestScheme(ITranslatingContext InContext)
            {
                foreach (var selectorKvp in _selectors)
                {
                    if (selectorKvp.Value.IsMatch(InContext))
                    {
                        return selectorKvp.Value.Scheme;
                    }
                }

                // Return the default scheme if no selector matched
                return DefaultScheme;
            }

            // Selectors sorted by priority
            private SortedList<int, IInfoTranslateSchemeSelector> _selectors
                = new SortedList<int, IInfoTranslateSchemeSelector>(new DuplicateKeyComparer<int>());
        }

        // Scheme groups dictionary
        private Dictionary<string, SchemeGroup> _schemeGroups = new Dictionary<string, SchemeGroup>();

        // A comparer that handles duplicate keys by treating them as greater than each other.
        private class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
        {
            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                {
                    // Treat equal keys as greater to allow duplicates
                    return 1;
                }
                return -result;
            }
        }
    }
}