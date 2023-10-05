using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace nf.protoscript.translator.DefaultSnippetElements
{

    /// <summary>
    /// A special 'NewLine' element.
    /// </summary>
    public sealed class ElementNewLine
        : InfoTranslateSnippet.IElement
    {
        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// A snippet element which contains only constant-string.
    /// </summary>
    public class ElementConstString
        : InfoTranslateSnippet.IElement
    {
        public string Value { get; }
        public ElementConstString(string InValue)
        {
            Value = InValue;
        }
        public override string ToString()
        {
            return Value;
        }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            return new string[] { Value };
        }
    }


    /// <summary>
    /// A snippet element to get value strings from the translating Context.
    /// </summary>
    public class ElementNodeValue
        : InfoTranslateSnippet.IElement
    {
        public ElementNodeValue(string InKey)
        {
            Key = InKey;
        }

        public string Key { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Find data value from SI's context
            var ctxValStr = InHolderSchemeInstance.Context.GetContextValueString(Key);
            return new string[] { ctxValStr };
        }
    }


    /// <summary>
    /// Element to write all sub-elements in a new block which has a increment indent than the current block.
    /// </summary>
    public class ElementIndentBlock
        : InfoTranslateSnippet.IElement
    {
        public ElementIndentBlock(params InfoTranslateSnippet.IElement[] InSubElements)
        {
            SubSnippet = new InfoTranslateSnippet(InSubElements);
        }

        /// <summary>
        /// Sub snippet
        /// </summary>
        public InfoTranslateSnippet SubSnippet { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Get sub results, insert spaces at the very first of each line, and then return.

            var subResults = SubSnippet.Apply(InHolderSchemeInstance);
            List<string> results = new List<string>(subResults.Count);
            foreach (var item in subResults)
            {
                results.Add(item.Insert(0, "    "));
            }
            return results;
        }
    }

    /// <summary>
    /// Call another scheme with the specific context.
    /// </summary>
    public class ElementCall
        : InfoTranslateSnippet.IElement
    {
        public ElementCall(params InfoTranslateSnippet.IElement[] InElements)
        {
            Snippet = new InfoTranslateSnippet(InElements);
            ContextFindParam = null;
        }

        public ElementCall(TranslatingContextFinder.FindParam InContextName
            , params InfoTranslateSnippet.IElement[] InElements
            )
        {
            Snippet = new InfoTranslateSnippet(InElements);
            ContextFindParam = InContextName;
        }

        /// <summary>
        /// The scheme to call.
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// The Context's name.
        /// </summary>
        public TranslatingContextFinder.FindParam ContextFindParam { get; }

        /// <summary>
        /// The calling snippet
        /// </summary>
        public InfoTranslateSnippet Snippet { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            IEnumerable<ITranslatingContext> targetContexts = new ITranslatingContext[] { InHolderSchemeInstance.Context };
            if (ContextFindParam != null)
            {
                targetContexts = TranslatingContextFinder.Find(InHolderSchemeInstance.Context, ContextFindParam);
            }

            var translator = InHolderSchemeInstance.HostTranslator;
            var scheme = new InfoTranslateSchemeDefault(Snippet);

            List<string> results = new List<string>();
            foreach (var targetContext in targetContexts)
            {
                var si = scheme.CreateInstance(translator, targetContext);
                results.AddRange(si.GetResult());
            }
            return results;
        }

    }


    /// <summary>
    /// For each sub info and call another scheme for it.
    /// </summary>
    public class ElementForeachSubCall
        : InfoTranslateSnippet.IElement
    {
        public ElementForeachSubCall(string InSchemeName, string InHeaderFilter)
        {
            SchemeName = InSchemeName;
            HeaderFilter = InHeaderFilter;
        }

        /// <summary>
        /// The scheme to call for each sub Info
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// Which infos should be selected
        /// </summary>
        public string HeaderFilter { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;
            List<string> result = new List<string>();
            (context as ITranslatingInfoContext)?.TranslatingInfo.ForeachSubInfo<Info>(
                info =>
                {
                    TranslatingInfoContext infoCtx = new TranslatingInfoContext(context, info);
                    var scheme = translator.FindBestScheme(infoCtx, SchemeName);
                    var si = scheme.CreateInstance(translator, infoCtx);
                    result.AddRange(si.GetResult());
                }
                , info => info.Header.Contains(HeaderFilter)
                );
            return result;
        }

    }

    /// <summary>
    /// Element which creates a MethodBodyContext and translate expressions of the method.
    /// The context of this Element must be a method context (Context with a valid MethodInfo).
    /// </summary>
    public class ElementMethodBody
        : InfoTranslateSnippet.IElement
    {
        public ElementMethodBody()
        {
        }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            ITranslatingContext ctx = InHolderSchemeInstance.Context;

            // Exact parameters from the context.
            // This element must be called when the context is a 'ElementInfo' context.
            ElementInfo mtdInfo = (ctx as ITranslatingInfoContext)?.TranslatingInfo as ElementInfo;
            TypeInfo mtdHostType = mtdInfo.FindTheFirstParent<TypeInfo>();
            ProjectInfo globalInfo = mtdInfo.FindTheFirstParent<ProjectInfo>();

            Debug.Assert(mtdInfo != null);
            Debug.Assert(mtdHostType != null);
            Debug.Assert(globalInfo != null);

            // Create Method Body Context
            var env = new expression.ExprTranslateEnvironmentDefault(mtdInfo
                , new expression.ExprTranslateEnvironmentDefault.Scope[]
                {
                    new expression.ExprTranslateEnvironmentDefault.Scope(mtdInfo, "local", "")
                    , new expression.ExprTranslateEnvironmentDefault.Scope(mtdHostType, "this", "this->")
                    , new expression.ExprTranslateEnvironmentDefault.Scope(globalInfo, "global", "::")
                }
            );
            var mtdCtx = new expression.MethodBodyContext(ctx, mtdInfo, env);

            // TODO let the translator decide how to create a MethodBodyContext.
            //expression.MethodBodyContext mtdCtx = translator.AllocBodyContextForMethod(ctx);

            // Select an expr-translator and do translating.
            var exprTranslator = translator.LoadExprTranslator("");
            var codes = exprTranslator.Translate(mtdCtx, mtdInfo.InitSyntax);

            return codes;
        }

    }

    /// <summary>
    /// Register a new method to the translating Type.
    /// </summary>
    public class ElementNewMethod
        : InfoTranslateSnippet.IElement
    {
        public ElementNewMethod(string InMethodName, params InfoTranslateSnippet.IElement[] InSubElements)
        {
            MethodName = InMethodName;
            SubSnippet = new InfoTranslateSnippet(InSubElements);
        }

        /// <summary>
        /// Name of the method
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Sub snippet
        /// </summary>
        public InfoTranslateSnippet SubSnippet { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            ITranslatingContext ctx = InHolderSchemeInstance.Context;

            // Exact parameters from the context.
            // This element must be called when the context is a 'TypeInfo' context.
            TypeInfo typeInfo = (ctx as ITranslatingInfoContext)?.TranslatingInfo as TypeInfo;
            ProjectInfo globalInfo = typeInfo.FindTheFirstParent<ProjectInfo>();

            Debug.Assert(typeInfo != null);
            Debug.Assert(globalInfo != null);

            // TODO Create the special ctor method.

            // Create Method Body Context
            var ctorEnv = new expression.ExprTranslateEnvironmentDefault(typeInfo
                , new expression.ExprTranslateEnvironmentDefault.ScopeBase[]
                {
                    // Special method scope with the param-list which was set manually.
                    new expression.ExprTranslateEnvironmentDefault.VirtualMethodScope(MethodName, "")
                    , new expression.ExprTranslateEnvironmentDefault.Scope(typeInfo, "this", "this->")
                    , new expression.ExprTranslateEnvironmentDefault.Scope(globalInfo, "global", "::")
                }
            );
            var mtdCtx = new expression.VirtualMethodBodyContext(ctx, typeInfo, "ctor", ctorEnv);

            // Create and cache the scheme by SubSnippet.
            if (_cachedScheme == null)
            {
                _cachedScheme = new InfoTranslateSchemeDefault(SubSnippet);
            }
            // Create the sub scheme-instance and set its context to the Ctor-method's context.
            var subSI = _cachedScheme.CreateInstance(translator, mtdCtx);

            var subResults = subSI.GetResult();
            return subResults;
        }

        // Scheme constructed by the SubSnippet
        InfoTranslateSchemeDefault _cachedScheme = null;

    }



}