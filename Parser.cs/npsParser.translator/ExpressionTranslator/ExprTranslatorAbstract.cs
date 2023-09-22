using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace nf.protoscript.translator.expression
{


    /// <summary>
    /// Access type of the expression variable
    /// </summary>
    public enum EExprVarAccessType
    {
        Get,
        Set,
        Ref,
    }


    /// <summary>
    /// Common calls of all Expression-Translators.
    /// </summary>
    public abstract partial class ExprTranslatorAbstract
    {

        /// <summary>
        /// Translate syntax tree into codes.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> Translate(IExprTranslateContext InContext, ISyntaxTreeNode InSyntaxTree)
        {
            // Gather schemeInstances.
            STNodeVisitor_FunctionBody visitor = new STNodeVisitor_FunctionBody(this, InContext);
            VisitByReflectionHelper.FindAndCallVisit(InSyntaxTree, visitor);

            // Create runtime context and do apply schemeInstances.
            var schemeInstances = visitor.TranslateSchemeInstances;
            List<string> codes = new List<string>();
            _HandleSchemeInstances(codes, schemeInstances);

            return codes;
        }

        //
        // Constant access schemeInstances
        //

        protected abstract ISTNodeTranslateScheme ErrorScheme(STNodeBase InErrorNode);

        protected abstract ISTNodeTranslateScheme QueryNullScheme(TypeInfo InConstType);

        protected abstract ISTNodeTranslateScheme QueryConstGetInfoScheme(
            TypeInfo InConstType
            , Info InInfo
            );

        protected abstract ISTNodeTranslateScheme QueryConstGetStringScheme(
            TypeInfo InConstType
            , string InString
            );

        protected abstract ISTNodeTranslateScheme QueryConstGetScheme(
            TypeInfo InConstType
            , string InValueString
            );


        //
        // var access schemeInstances
        //

        // ~ Replaced by Context's FindVariable
        // public struct FindVarResult
        // {
        //     public FindVarResult(
        //         Info InElementScope
        //         , TypeInfo InElementHolderType
        //         , ElementInfo InElementInfo
        //         , ISTNodeTranslateSchemeInstance InHostSchemeInstance
        //         )
        //     {
        //         ElementScope = InElementScope;
        //         ElementHolderType = InElementHolderType;
        //         ElementInfo = InElementInfo;
        //         HostSchemeInstance = InHostSchemeInstance;
        //     }

        //     public Info ElementScope { get; }

        //     public TypeInfo ElementHolderType { get; }

        //     public ElementInfo ElementInfo { get; }

        //     public ISTNodeTranslateSchemeInstance HostSchemeInstance { get; }

        // }

        //public virtual FindVarResult FindVarAlongScopeChain(IEnumerable<TODOScope> InScopeChain, string InVarName)
        //{
        //    throw new NotImplementedException();

        //    if (InContextInfo is TypeInfo)
        //    {
        //        var foundInfo = InfoHelper.FindPropertyOfType(InContextInfo, InVarName);
        //        return foundInfo;
        //    }
        //    else if (InContextInfo is ElementInfo)
        //    {
        //        // Find Local property or archetype property
        //        // For Property
        //        //      1.? it's inline-settings for the property's sub-element:
        //        //          Tower
        //        //              -Pos = x + 100
        //        //                  -x = y + 100
        //        //      2.? or sub-element of the property's Archetype.
        //        //          Tower
        //        //              -Transform = 
        //        // For Method:
        //        //      1. it's local variable of the method.
        //        //          TestClass
        //        //              +TestMethod()
        //        //                  - x = 3
        //        //                  > x = x * x
        //        //          # Here x will be found in the MethodInfo's sub-element
        //        //
        //        //      2. or the parameter of the method
        //        //          TestClass
        //        //              +TestMethod(x, y)
        //        //                  > x = y + 100
        //        //          # Here x, y will be found in the MethodInfo's Archetype
        //        //
        //        var foundInfo = InfoHelper.FindLocalOrArchetype(InContextInfo, InVarName);

        //        var foundInfo = InfoHelper.FindPropertyOfType(InContextInfo.ParentInfo, InVarName);
        //        return foundInfo;
        //    }

        //    var foundInfo = InfoHelper.FindLocalOrGlobal(InContextInfo, InVarName);
        //    return foundInfo;
        //}



        protected abstract ISTNodeTranslateScheme QueryMemberAccessScheme(
            EExprVarAccessType InAccessType
            , TypeInfo InContextType
            , string InMemberID
            , out TypeInfo OutMemberType
            );
       

        //
        // Operation schemeInstances.
        //

        protected abstract ISTNodeTranslateScheme QueryBinOpScheme(
            STNodeBinaryOp InBinOpNode
            , TypeInfo InLhsType
            , TypeInfo InRhsType
            , out TypeInfo OutResultType
            );

        /// <summary>
        /// Query scheme to generate host-access codes like "Host.Member"
        /// </summary>
        /// <param name="InMemberAccessNode"></param>
        /// <param name="InHostElement"></param>
        /// <returns></returns>
        protected abstract ISTNodeTranslateScheme QueryHostAccessScheme(
            STNodeMemberAccess InMemberAccessNode
            , TypeInfo InHostType
            );


        private void _HandleSchemeInstances(
            List<string> OutCodes
            , IEnumerable<ISTNodeTranslateSchemeInstance> InSchemeInstances
            )
        {
            foreach (var schemeInst in InSchemeInstances)
            {
                var presentResult = schemeInst.GetResult("Present");
                OutCodes.AddRange(presentResult);
            }
        }

    }


}
