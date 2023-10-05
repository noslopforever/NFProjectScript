using nf.protoscript.syntaxtree;
using nf.protoscript.translator.expression;
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
            var translator = InHolderSchemeInstance.HostTranslator;
            ITranslatingContext ctx = InHolderSchemeInstance.Context;

            // Exact parameters from the context.
            // This element must be called when the context is a 'TypeInfo' context.
            TypeInfo typeInfo = (ctx as ITranslatingInfoContext)?.TranslatingInfo as TypeInfo;
            ProjectInfo globalInfo = typeInfo.FindTheFirstParent<ProjectInfo>();

            Debug.Assert(typeInfo != null);
            Debug.Assert(globalInfo != null);

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

            // Create the sub scheme-instance and bind it with Ctor-method's context.
            var subSI = SubScheme.CreateInstance(translator, mtdCtx);
            var subResults = subSI.GetResult();
            return subResults;
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
            var translator = InHolderSchemeInstance.HostTranslator;
            ITranslatingInfoContext ctx = InHolderSchemeInstance.Context as ITranslatingInfoContext;

            // Exact parameters from the context.
            // This element must be called when the context is a 'TypeInfo' context.
            ElementInfo elemInfo = ctx?.TranslatingInfo as ElementInfo;
            Debug.Assert(elemInfo != null);
            if (elemInfo.InitSyntax == null)
            {
                return new string[0];
            }

            IMethodBodyContext mtdCtx = TranslatingContextFinder.FindAncestor(ctx, context => context is IMethodBodyContext) as IMethodBodyContext;
            Debug.Assert(mtdCtx != null);

            // Load ExpressionTranslator to translate the target expressions.
            ExprTranslatorAbstract exprTranslator = translator.LoadExprTranslator("");

            // Wrap init syntax and do translate.
            var initSyntax = new STNodeMemberInit(elemInfo, elemInfo.InitSyntax);
            var subResults = exprTranslator.Translate(mtdCtx, initSyntax);
            return subResults;
        }

    }

}