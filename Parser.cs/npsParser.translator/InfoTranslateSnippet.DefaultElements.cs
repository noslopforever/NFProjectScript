using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

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
    /// A snippet element to get value strings from the parameter.
    /// </summary>
    public class ElementParamValue
        : InfoTranslateSnippet.IElement
    {
        public ElementParamValue(string InKey)
        {
            Key = InKey;
        }

        public string Key { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            // Find data value from SI's context
            var ctxValStr = InHolderSchemeInstance.GetParamValue(Key);
            return ctxValStr;
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
    /// Base class of all elements with sub-snippets
    /// </summary>
    public abstract class ElementWithSubSnippets
    {
        public ElementWithSubSnippets(params InfoTranslateSnippet.IElement[] InSubElements)
        {
            SubSnippet = new InfoTranslateSnippet(InSubElements);
            SubScheme = new InfoTranslateSchemeDefault(SubSnippet);
        }

        public InfoTranslateSnippet SubSnippet { get; }

        public IInfoTranslateScheme SubScheme { get; }

    }

    /// <summary>
    /// Change current context and call sub-snippets with the new Context.
    /// TODO replace with scope change.
    /// </summary>
    public class ElementChangeContext
        : ElementWithSubSnippets
        , InfoTranslateSnippet.IElement
    {
        public ElementChangeContext(string InNewContextName, params InfoTranslateSnippet.IElement[] InSubElements)
            : base(InSubElements)
        {
            NewContextName = InNewContextName;
        }

        /// <summary>
        /// The new context 
        /// </summary>
        public string NewContextName { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;

            IEnumerable<ITranslatingContext> targetContexts = null;
            if (NewContextName != null && NewContextName != "")
            {
                // TODO Unify constructions of all contexts into a unique factory.

                // Get the caller object by Name.
                if (InHolderSchemeInstance.Context.TryGetContextValue(NewContextName, out var val))
                {
                    // construct the context for the caller-object
                    if (val is Info)
                    {
                        var newCtx = new TranslatingInfoContext(InHolderSchemeInstance.Context, val as Info);
                        targetContexts = new ITranslatingContext[] { newCtx };
                    }
                    else if (val is ISyntaxTreeNode)
                    {
                        var newCtx = new TranslatingExprContext(InHolderSchemeInstance.Context, val as ISyntaxTreeNode);
                        targetContexts = new ITranslatingContext[] { newCtx };
                    }
                }
            }

            if (targetContexts != null)
            {
                List<string> results = new List<string>();
                foreach (var targetContext in targetContexts)
                {
                    var si = SubScheme.CreateInstance(translator, targetContext);
                    results.AddRange(si.GetResult());
                }
                return results;
            }

            // TODO log error.
            throw new InvalidCastException();
            return new string[0];
        }

    }

    /// <summary>
    /// Call another scheme for the current context.
    /// </summary>
    public class ElementCall
        : InfoTranslateSnippet.IElement
    {
        public ElementCall(string InSchemeName, params (string, InfoTranslateSnippet.IElement)[] InParams)
        {
            SchemeName = InSchemeName;
            foreach (var item in InParams)
            {
                _params.Add(item.Item1, new InfoTranslateSnippet(item.Item2));
            }
        }

        /// <summary>
        /// The scheme to call
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// Pass in parameters.
        /// </summary>
        public IReadOnlyDictionary<string, InfoTranslateSnippet> Params { get { return _params; } }
        Dictionary<string, InfoTranslateSnippet> _params = new Dictionary<string, InfoTranslateSnippet>();

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;

            var scheme = translator.FindBestScheme(context, SchemeName);
            var si = scheme.CreateInstance(translator, context);

            // collect parameter scheme instances from parameter snippets.
            foreach (var item in _params)
            {
                InfoTranslateSnippet snippet = item.Value;
                var paramScheme = new InfoTranslateSchemeDefault(snippet);
                var paramSI = paramScheme.CreateInstance(translator, context);
                si.AddParam(item.Key, paramSI);
            }

            return si.GetResult();
        }

    }

    /// <summary>
    /// For each sub info and call another scheme for it.
    /// </summary>
    public class ElementForeachSubCall
        : InfoTranslateSnippet.IElement
    {
        public ElementForeachSubCall(string InSchemeName, string InHeaderFilter, string InSeperator = "")
        {
            SchemeName = InSchemeName;
            HeaderFilter = InHeaderFilter;
            Seperator = InSeperator;
        }

        /// <summary>
        /// The scheme to call for each sub Info
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// Which infos should be selected
        /// </summary>
        public string HeaderFilter { get; }

        /// <summary>
        /// Seperator between sub items.
        /// </summary>
        public string Seperator { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            var translator = InHolderSchemeInstance.HostTranslator;
            var context = InHolderSchemeInstance.Context;
            List<string> result = new List<string>();
            result.Add("");
            if (context is ITranslatingInfoContext)
            {
                (context as ITranslatingInfoContext)?.TranslatingInfo.ForeachSubInfo<Info>(
                    info =>
                    {
                        TranslatingInfoContext infoCtx = new TranslatingInfoContext(context, info);
                        var scheme = translator.FindBestScheme(infoCtx, SchemeName);
                        var si = scheme.CreateInstance(translator, infoCtx);
                        var subResult = si.GetResult();
                        if (subResult.Count > 0)
                        {
                            result[^1] += subResult[0];
                        }
                        for (int i = 1; i < subResult.Count; i++)
                        {
                            result.Add(subResult[i]);
                        }
                        result[^1] += Seperator;
                    }
                    , info => info.Header.Contains(HeaderFilter)
                    );
                // Remove the last seperator
                result[^1] = result[^1].Remove(result[^1].Length - Seperator.Length);
            }
            else if (context is ITranslatingExprContext)
            {
                (context as ITranslatingExprContext).TranslatingExprNode.ForeachSubNodes(
                    (key, stNode) =>
                    {
                        if (key.StartsWith(HeaderFilter))
                        {
                            TranslatingExprContext exprCtx = new TranslatingExprContext(context, stNode);
                            var scheme = translator.FindBestScheme(exprCtx, SchemeName);
                            var si = scheme.CreateInstance(translator, exprCtx);
                            var subResult = si.GetResult();
                            if (subResult.Count > 0)
                            {
                                result[^1] += subResult[0];
                            }
                            for (int i = 1; i < subResult.Count; i++)
                            {
                                result.Add(subResult[i]);
                            }
                            result[^1] += Seperator;
                        }
                        return true;
                    }
                    );
                // Remove the last seperator
                result[^1] = result[^1].Remove(result[^1].Length - Seperator.Length);
            }
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
            throw new NotImplementedException();
            //var translator = InHolderSchemeInstance.HostTranslator;
            //ITranslatingContext ctx = InHolderSchemeInstance.Context;

            //// Exact parameters from the context.
            //// This element must be called when the context is a 'MethodInfo' context.
            //ElementInfo mtdInfo = (ctx as TranslatingInfoContext)?.TranslatingInfo as ElementInfo;
            //TypeInfo mtdHostType = mtdInfo.FindTheFirstParent<TypeInfo>();
            //ProjectInfo globalInfo = mtdInfo.FindTheFirstParent<ProjectInfo>();

            //Debug.Assert(mtdInfo != null);
            //Debug.Assert(mtdHostType != null);
            //Debug.Assert(globalInfo != null);

            //// Create a new scope (local-scope) for the method.
            //var scopeCtx = translator.AllocScopeContext(ctx, mtdInfo);
            //// Create the expression context and translate it.
            //var mtdExprCtx = translator.AllocExpressionContext(scopeCtx, mtdInfo.InitSyntax);
            //var codes = translator.TranslateInfo(mtdExprCtx, "");

            //return codes;
        }

    }

    /// <summary>
    /// Register a new method to the translating Type.
    /// </summary>
    public class ElementNewMethod
        : ElementWithSubSnippets
        , InfoTranslateSnippet.IElement
    {
        public ElementNewMethod(string InMethodName, params InfoTranslateSnippet.IElement[] InSubElements)
            : base(InSubElements)
        {
            MethodName = InMethodName;
        }

        /// <summary>
        /// Name of the method
        /// </summary>
        public string MethodName { get; }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            throw new NotImplementedException();

            //var translator = InHolderSchemeInstance.HostTranslator;
            //ITranslatingContext ctx = InHolderSchemeInstance.Context;

            //// Exact parameters from the context.
            //// This element must be called when the context is a 'TypeInfo' context.
            //TypeInfo typeInfo = ctx?.TranslatingInfo as TypeInfo;
            //ProjectInfo globalInfo = typeInfo.FindTheFirstParent<ProjectInfo>();

            //Debug.Assert(typeInfo != null);
            //Debug.Assert(globalInfo != null);

            //// Create Method Body Context
            //var ctorEnv = new expression.ExprTranslateEnvironmentDefault(typeInfo
            //    , new expression.ExprTranslateEnvironmentDefault.ScopeBase[]
            //    {
            //        // Special method scope with the param-list which was set manually.
            //        new expression.ExprTranslateEnvironmentDefault.VirtualMethodScope(MethodName, "")
            //        , new expression.ExprTranslateEnvironmentDefault.Scope(typeInfo, "this", "this->")
            //        , new expression.ExprTranslateEnvironmentDefault.Scope(globalInfo, "global", "::")
            //    }
            //);
            //var scopeCtx = new expression.VirtualMethodBodyContext(ctx, typeInfo, "ctor", ctorEnv);

            //// Create the sub scheme-instance and bind it with Ctor-method's context.
            //var subSI = SubScheme.CreateInstance(translator, scopeCtx);
            //var subResults = subSI.GetResult();
            //return subResults;
        }

    }

    /// <summary>
    /// Element that takes the context's expression as init-expressions which means to wrap it with 'STNodeMemberInit' and then translate it.
    /// </summary>
    public class ElementInitExpression
        : InfoTranslateSnippet.IElement
    {
        public ElementInitExpression()
        {
        }

        public IReadOnlyList<string> Apply(IInfoTranslateSchemeInstance InHolderSchemeInstance)
        {
            throw new NotImplementedException();

            //var translator = InHolderSchemeInstance.HostTranslator;
            //var ctx = InHolderSchemeInstance.Context;

            //// Exact parameters from the context.
            //// This element must be called when the context is a 'ElementInfo' context.
            //ElementInfo elemInfo = ctx?.TranslatingInfo as ElementInfo;
            //Debug.Assert(elemInfo != null);
            //if (elemInfo.InitSyntax == null)
            //{
            //    return new string[0];
            //}

            //IMethodBodyContext scopeCtx = TranslatingContextFinder.FindAncestor(ctx, context => context is IMethodBodyContext) as IMethodBodyContext;
            //Debug.Assert(scopeCtx != null);

            //// Load ExpressionTranslator to translate the target expressions.
            //ExprTranslatorAbstract exprTranslator = translator.LoadExprTranslator("");

            //// Wrap init syntax and do translate.
            //var initSyntax = new STNodeMemberInit(elemInfo, elemInfo.InitSyntax);
            //var subResults = exprTranslator.Translate(scopeCtx, initSyntax);
            //return subResults;
        }

    }

}