using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace nf.protoscript.translator.expression
{


    public class ExprTranslatorDefault
        : ExprTranslatorAbstract
    {

        // Begin ExprTranslatorAbstract interfaces

        public override ISTNodeTranslateScheme FindBestScheme(ITranslatingContext InContext, string InSchemeName)
        {
            if (_schemeGroups.TryGetValue(InSchemeName, out var schemeGroup))
            {
                return schemeGroup.FindBestScheme(InContext);
            }
            return null;
        }

        // ~ End ExprTranslatorAbstract interfaces


        /// <summary>
        /// Register a SchemeSelector.
        /// </summary>
        /// <param name="InSelector"></param>
        public void AddSchemeSelector(string InKey, ISTNodeTranslateSchemeSelector InSelector)
        {
            EnsureGroup(InKey).AddSelector(InSelector.Priority, InSelector);
        }

        /// <summary>
        /// Add generic schemes
        /// </summary>
        /// <param name="InKey"></param>
        /// <param name="InSnippet"></param>
        public void AddScheme(string InKey, STNodeTranslateSnippet InSnippet)
        {
            var newScheme = new STNodeTranslateSchemeDefault(InSnippet);
            EnsureGroup(InKey).DefaultScheme = newScheme;
        }

        /// <summary>
        /// Add generic scheme with multiple snippets.
        /// </summary>
        /// <param name="InKey"></param>
        /// <param name="InSnippet"></param>
        public void AddScheme(string InKey, Dictionary<string, STNodeTranslateSnippet> InSnippets)
        {
            var newScheme = new STNodeTranslateSchemeDefault(InSnippets);
            EnsureGroup(InKey).DefaultScheme = newScheme;
        }

        /// <summary>
        /// Find Or Add a scheme group with InSchemeName.
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
            public ISTNodeTranslateScheme DefaultScheme
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
            ISTNodeTranslateScheme _default = null;

            /// <summary>
            /// Add a selector.
            /// </summary>
            /// <param name="InPriority"></param>
            /// <param name="InSelector"></param>
            internal void AddSelector(int InPriority, ISTNodeTranslateSchemeSelector InSelector)
            {
                _selectors.Add(InPriority, InSelector);
            }

            /// <summary>
            /// Find a best scheme to match the InContext.
            /// </summary>
            /// <param name="InContext"></param>
            /// <returns></returns>
            internal ISTNodeTranslateScheme FindBestScheme(ITranslatingContext InContext)
            {
                // Find special MemberAccess schemes by (HostType, PropertyName)
                foreach (var selectorKvp in _selectors)
                {
                    if (selectorKvp.Value.IsMatch(InContext))
                    {
                        return selectorKvp.Value.Scheme;
                    }
                }

                // return default scheme if have
                return DefaultScheme;
            }

            // selectors
            SortedList<int, ISTNodeTranslateSchemeSelector> _selectors
                = new SortedList<int, ISTNodeTranslateSchemeSelector>(new DuplicateKeyComparer<int>());

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
