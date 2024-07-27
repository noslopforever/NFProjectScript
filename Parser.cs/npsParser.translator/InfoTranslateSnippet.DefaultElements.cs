﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace nf.protoscript.translator.DefaultSnippetElements
{

    /// <summary>
    /// Base class of all elements with sub-snippets
    /// </summary>
    public abstract class ElementWithSubSnippets
    {
        public ElementWithSubSnippets(params InfoTranslateSchemeDefault.IElement[] InSubElements)
        {
            SubScheme = new InfoTranslateSchemeDefault(InSubElements);
        }

        /// <summary>
        /// Scheme applied to each sub-info.
        /// </summary>
        public IInfoTranslateScheme SubScheme { get; }

    }

    /// <summary>
    /// Element which creates a MethodBodyContext and translate expressions of the method.
    /// The context of this Element must be a method context (Context with a valid MethodInfo).
    /// </summary>
    public class ElementMethodBody
        : InfoTranslateSchemeDefault.IElement
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
        : InfoTranslateSchemeDefault.IElement
    {
        public ElementNewMethod(string InMethodName, params InfoTranslateSchemeDefault.IElement[] InSubElements)
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
        : InfoTranslateSchemeDefault.IElement
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