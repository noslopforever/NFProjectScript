using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Function Sector to describe a function.
    /// </summary>
    public class FunctionSector
        : Sector
    {
        internal FunctionSector(Token[] InTokens, STNode_FunctionDef InParsedResult)
            : base(InTokens)
        {
            FuncDef = InParsedResult;
        }

        /// <summary>
        /// Result parsed
        /// </summary>
        internal STNode_FunctionDef FuncDef { get; }

        /// <summary>
        /// Name of the element.
        /// </summary>
        public string Name { get; }


        // All sub sectors which will be treated as parameters.
        private List<ElementSector> _MethodSubParamSectors = new List<ElementSector>();

        public override void AnalyzeSector()
        {
            // Method: Gather parameters and expressions form sub-sectors.
            var subElemSectors = GetSubSectors<ElementSector>();
            foreach (var elemSec in subElemSectors)
            {
                if (elemSec.Type == ElementSector.EType.Member)
                {
                    // Set the sub element  as parameter
                    elemSec.Type = ElementSector.EType.Parameter;
                    _MethodSubParamSectors.Add(elemSec);
                }
            }
        }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            Info parentInfo = InParentSector.CollectedInfo;
            if (parentInfo == null)
            {
                // TODO log error.
                throw new NotImplementedException();
                return null;
            }

            // ## Let TypeSig to find the method's return TypeInfo.
            TypeInfo typeInfo = CommonTypeInfos.Any;
            if (FuncDef.TypeSig != null)
            {
                typeInfo = FuncDef.TypeSig.LocateTypeInfo(InProjectInfo, parentInfo);
            }

            // ## Merge init-expr with sub expressions.
            List<syntaxtree.ISyntaxTreeNode> subExprs = _TryGatherSubExprs();
            if (FuncDef.InitExpression != null)
            {
                if (FuncDef.InitExpression is STNodeSequence)
                {
                    subExprs.InsertRange(0, (FuncDef.InitExpression as STNodeSequence).NodeList);
                }
                else
                {
                    subExprs.Insert(0, FuncDef.InitExpression);
                }
            }
            syntaxtree.STNodeSequence stSeq = new syntaxtree.STNodeSequence(subExprs.ToArray());

            // ## New MethodInfo instance.
            ElementInfo mtdInfo = new ElementInfo(parentInfo, "method", FuncDef.DefName, typeInfo, stSeq);
            // Register inline and line-end attributes
            foreach (var attrDef in FuncDef.Attributes)
            {
                var attrInfo = new AttributeInfo(parentInfo, attrDef.DefName, attrDef.DefName, attrDef.InitExpression);
            }

            // ## Fill parameters by this sector
            foreach (var paramDef in FuncDef.Params)
            {
                _GenerateParamByDef(InProjectInfo, mtdInfo, paramDef);
            }

            // ## Fill parameters by sub sectors
            foreach (var subParam in _MethodSubParamSectors)
            {
                var subParamDef = subParam.ParsedResult as STNode_ElementDef;
                _GenerateParamByDef(InProjectInfo, mtdInfo, subParamDef);
            }

            return mtdInfo;
        }
        

        /// <summary>
        /// Generate ParamInfo by STNode_ElementDef.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InMethodInfo"></param>
        /// <param name="InSTElemDef"></param>
        private static void _GenerateParamByDef(ProjectInfo InProjectInfo, ElementInfo InMethodInfo, STNode_ElementDef InSTElemDef)
        {
            TypeInfo paramTypeInfo = CommonTypeInfos.Any;
            if (InSTElemDef.TypeSig != null)
            {
                paramTypeInfo = InSTElemDef.TypeSig.LocateTypeInfo(InProjectInfo, InMethodInfo);
            }
            ElementInfo paramInfo = new ElementInfo(InMethodInfo, "param", InSTElemDef.DefName, paramTypeInfo, InSTElemDef.InitExpression);
        }


        /// <summary>
        /// Gather sub expressions.
        /// </summary>
        /// <returns></returns>
        private List<syntaxtree.ISyntaxTreeNode> _TryGatherSubExprs()
        {
            List<syntaxtree.ISyntaxTreeNode> exprs = new List<syntaxtree.ISyntaxTreeNode>();

            var subExprSectors = GetSubSectors<ExpressionSector>();
            foreach (var exprSec in subExprSectors)
            {
                exprs.Add(exprSec.Expr);
            }

            return exprs;
        }

    }



}
