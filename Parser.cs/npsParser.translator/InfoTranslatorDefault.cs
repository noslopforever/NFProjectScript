using nf.protoscript.syntaxtree;
using nf.protoscript.translator.SchemeSelectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{
    public class InfoTranslatorDefault
        : InfoTranslatorAbstract
    {

        public override IInfoTranslateScheme FindBestScheme(ITranslatingContext InTranslatingContext, string InSchemeName)
        {
            if (_schemeGroups.TryGetValue(InSchemeName, out var schemeGroup))
            {
                return schemeGroup.FindBestScheme(InTranslatingContext);
            }
            return null;
        }

        /// <summary>
        /// Add a generic scheme
        /// </summary>
        /// <param name="InKey"></param>
        /// <param name="InScheme"></param>
        public void AddScheme(string InKey, IInfoTranslateScheme InScheme)
        {
            EnsureGroup(InKey).DefaultScheme = InScheme;
        }

        /// <summary>
        /// Add scheme selectors.
        /// </summary>
        /// <param name="InKey"></param>
        /// <param name="InSnippet"></param>
        public void AddSelector(string InKey, IInfoTranslateSchemeSelector InSchemeSelector)
        {
            EnsureGroup(InKey).AddSelector(InSchemeSelector);
        }


        /// <summary>
        /// Add scheme for expressions (syntax tree nodes).
        /// </summary>
        /// <param name="InKey"></param>
        /// <param name="InExprType"></param>
        /// <param name="InScheme"></param>
        public void AddExprScheme(string InKey, string InExprTypename, int InPriority, IInfoTranslateScheme InScheme)
        {
            throw new NotImplementedException();
            //AddSelector(InKey, new TranslateSchemeSelector_Lambda(InPriority
            //    , ctx =>
            //    {
            //        var exprCtx = ctx as ITranslatingExprContext;
            //        if (exprCtx != null && exprCtx.TranslatingExprNode != null)
            //        {
            //            return exprCtx.TranslatingExprNode.Typename == InExprTypename;
            //        }
            //        return false;
            //    }
            //    , new InfoTranslateSchemeDefault(InSnippet)
            //    ));
        }

        /// <summary>
        /// Add scheme for expressions (syntax tree nodes).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InKey"></param>
        /// <param name="InPriority"></param>
        /// <param name="InScheme"></param>
        public void AddExprScheme<T>(string InKey, int InPriority, IInfoTranslateScheme InScheme)
            where T : ISyntaxTreeNode
        {
            AddSelector(InKey, new TranslateSchemeSelector_Lambda(InPriority
                , ctx =>
                {
                    var exprCtx = ctx as ITranslatingExprContext;
                    if (exprCtx != null && exprCtx.TranslatingExprNode != null)
                    {
                        return exprCtx.TranslatingExprNode is T;
                    }
                    return false;
                }
                , InScheme
                ));
        }

        /// <summary>
        /// Add scheme for expressions (syntax tree nodes).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InKey"></param>
        /// <param name="InPriority"></param>
        /// <param name="InSelector"></param>
        /// <param name="InScheme"></param>
        public void AddExprSelector<T>(string InKey, int InPriority, Func<T, bool> InSelector, IInfoTranslateScheme InScheme)
            where T : class, ISyntaxTreeNode
        {
            AddSelector(InKey, new TranslateSchemeSelector_Lambda(InPriority
                , ctx =>
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
                }
                , InScheme
                ));
        }




        /// <summary>
        /// Find Or Add a scheme group with SchemeName.
        /// </summary>
        /// <param name="InSchemeName"></param>
        /// <returns></returns>
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
        /// Use a group to manage all schemes with the same name but different trigger conditions (selectors).
        /// </summary>
        public class SchemeGroup
        {
            internal SchemeGroup(string InSchemeName)
            {
                SchemeName = InSchemeName;
            }

            /// <summary>
            /// Name of the scheme group
            /// </summary>
            public string SchemeName { get; }

            /// <summary>
            /// The default scheme, if no selector matched, return this scheme.
            /// Only one default scheme can be assigned to a group.
            /// </summary>
            /// <exception cref="InvalidOperationException">Triggered if the default scheme has been set twice.</exception>
            public IInfoTranslateScheme DefaultScheme
            {
                get { return _default; }
                internal set
                {
                    if (_default != null)
                    {
                        // TODO log error
                        throw new InvalidOperationException("Cannot set default scheme twice!");
                    }
                    _default = value;
                }
            }
            IInfoTranslateScheme _default = null;

            /// <summary>
            /// Add a selector.
            /// </summary>
            /// <param name="InPriority"></param>
            /// <param name="InSelector"></param>
            internal void AddSelector(IInfoTranslateSchemeSelector InSelector)
            {
                _selectors.Add(InSelector.Priority, InSelector);
            }

            /// <summary>
            /// Find a best scheme to match the InContext.
            /// </summary>
            /// <param name="InContext"></param>
            /// <returns></returns>
            internal IInfoTranslateScheme FindBestScheme(ITranslatingContext InTranslatingContext)
            {
                // Find special MemberAccess schemes by (HostType, PropertyName)
                foreach (var selectorKvp in _selectors)
                {
                    if (selectorKvp.Value.IsMatch(InTranslatingContext))
                    {
                        return selectorKvp.Value.Scheme;
                    }
                }

                // return default scheme if have
                return DefaultScheme;
            }

            // selectors
            SortedList<int, IInfoTranslateSchemeSelector> _selectors
                = new SortedList<int, IInfoTranslateSchemeSelector>(new DuplicateKeyComparer<int>());

        }

        // Scheme groups
        Dictionary<string, SchemeGroup> _schemeGroups = new Dictionary<string, SchemeGroup>();

        // TODO Move it into the 'Base' project.
        class DuplicateKeyComparer<TKey>
            : IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1; // Handle equality as being greater. Note: this will break Remove(key) or
                else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
                    return result;
            }

            #endregion
        }

    }
}