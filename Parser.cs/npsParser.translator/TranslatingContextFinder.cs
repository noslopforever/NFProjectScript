using System;
using System.Collections.Generic;

namespace nf.protoscript.translator
{
    /// <summary>
    /// Helper functions for Constructing/Searching contexts.
    /// </summary>
    public static class TranslatingContextFinder
    {

        /// <summary>
        /// Parameters to find target contexts for the next call.
        /// </summary>
        public abstract class FindParam
        {
            internal IEnumerable<ITranslatingContext> _Find(ITranslatingContext InCurrentContext) => Find(InCurrentContext);
            protected abstract IEnumerable<ITranslatingContext> Find(ITranslatingContext InCurrentContext);
        }

        /// <summary>
        /// Datas to decide which context should be used.
        /// </summary>
        public class AncestorInfo
            : FindParam
        {
            public AncestorInfo(Type InInfoTypeToCheck, int InAncestorLevel = 1)
            {
                AncestorLevel = InAncestorLevel;
                AncestorType = InInfoTypeToCheck;
            }
            public AncestorInfo(string InHeaderToCheck, int InAncestorLevel = 1)
            {
                AncestorLevel = InAncestorLevel;
                AncestorHeader = InHeaderToCheck;
            }

            public int AncestorLevel { get; } = 1;

            /// <summary>
            /// Ancestor Type to check.
            /// </summary>
            public Type AncestorType { get; } = null;

            /// <summary>
            /// Header to check.
            /// </summary>
            public string AncestorHeader { get; } = null;

            protected override IEnumerable<ITranslatingContext> Find(ITranslatingContext InCurrentContext)
            {
                if (AncestorType != null)
                {
                    var targetCtx = TranslatingContextFinder.FindAncestorWithInfoType(InCurrentContext, AncestorType);
                    return new ITranslatingContext[] { targetCtx };
                }
                else if (AncestorHeader != null)
                {
                    throw new NotImplementedException();
                }
                return new ITranslatingContext[] { null };
            }

        }


        /// <summary>
        /// Find Contexts by FindParam.
        /// </summary>
        /// <param name="InContext"></param>
        /// <param name="InParam"></param>
        /// <returns></returns>
        public static IEnumerable<ITranslatingContext> Find(ITranslatingContext InContext, FindParam InParam)
        {
            return InParam._Find(InContext);
        }

        /// <summary>
        /// Find ancestor by predict.
        /// </summary>
        /// <param name="InContext"></param>
        /// <param name="InPred"></param>
        /// <returns></returns>
        public static ITranslatingContext FindAncestor(ITranslatingContext InContext, Func<ITranslatingContext, bool> InPred)
        {
            var checkingCtx = InContext;
            while (checkingCtx != null)
            {
                if (InPred(checkingCtx))
                {
                    return checkingCtx;
                }
                checkingCtx = checkingCtx.ParentContext;
            }
            return null;
        }

        /// <summary>
        /// Find ancestor which is a InfoTranslatingContext and it's InfoType matches the generic param 'T'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="InContext"></param>
        /// <returns></returns>
        public static ITranslatingContext FindAncestorWithInfoType<T>(ITranslatingContext InContext)
            where T: Info
        {
            return FindAncestor(InContext, ctx => 
            {
                var infoCtx = ctx as ITranslatingInfoContext;
                if (infoCtx != null)
                {
                    return infoCtx.TranslatingInfo is T;
                }
                return false;
            }
            );
        }

        public static ITranslatingContext FindAncestorWithInfoType(ITranslatingContext InContext, Type InCheckingInfoType)
        {
            return FindAncestor(InContext, ctx =>
            {
                var infoCtx = ctx as ITranslatingInfoContext;
                if (infoCtx != null)
                {
                    var infoType = infoCtx.TranslatingInfo.GetType();

                    return infoType == InCheckingInfoType
                        || infoType.IsSubclassOf(InCheckingInfoType);
                }
                return false;
            }
            );
        }

    }


}
