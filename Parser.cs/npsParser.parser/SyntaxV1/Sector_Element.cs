using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Element Sector like members, child-actors, components.
    /// </summary>
    public class ElementSector
        : Sector
    {
        public enum EType
        {
            Unknown,
            Member,
            ComponentOrChild,
            Method,
            Event,
            MethodSubParam,
            InlineTypeMember,
        }

        internal ElementSector(CodeLine InCodeLn, EType InType, STNode_DefBase InParsedResult)
            : base(InCodeLn)
        {
            Type = InType;
            _ParsedResult = InParsedResult;
        }

        internal static ElementSector NewMemberSector(CodeLine InCodeLn, STNode_ElementDef InElemDef)
        {
            return new ElementSector(InCodeLn, EType.Member, InElemDef);
        }

        internal static ElementSector NewMethodSector(CodeLine InCodeLn, STNode_FunctionDef InFuncDef)
        {
            return new ElementSector(InCodeLn, EType.Method, InFuncDef);
        }

        internal static ElementSector NewEventSector(CodeLine InCodeLn, STNode_FunctionDef InElemDef)
        {
            return new ElementSector(InCodeLn, EType.Event, InElemDef);
        }

        /// <summary>
        /// Type of the element.
        /// </summary>
        public EType Type { get; set; }

        /// <summary>
        /// Result parsed
        /// </summary>
        STNode_DefBase _ParsedResult;

        /// <summary>
        /// return func-def when the type is Method, otherwise throw Invalid-cast
        /// </summary>
        internal STNode_FunctionDef FuncDef
        {
            get
            {
                if (Type == EType.Method
                    || Type == EType.Event
                    )
                { return _ParsedResult as STNode_FunctionDef; }

                throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Name of the element.
        /// </summary>
        public string Name { get; }

        public override void AnalyzeSector()
        {
            // Method/Event: Mark parameters for sub-sectors.
            if (Type == EType.Method
                || Type == EType.Event
                )
            {
                var subElemSectors = GetSubSectors<ElementSector>();
                foreach (var elemSec in subElemSectors)
                {
                    if (elemSec.Type == EType.Member)
                    {
                        // Set sub elements as parameters
                        elemSec.Type = EType.MethodSubParam;
                    }
                }
            }
        }


        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            Info parentInfo = InParentSector.CollectedInfo;
            if (parentInfo == null)
            {
                throw new ParserException(
                    ParserErrorType.Collect_NoParentInfo
                    , CodeLn
                    );
            }

            Info chiefInfo = null;

            // Gather sub exprs which should be used in next process
            List<syntaxtree.ISyntaxTreeNode> subExprs = _TryGatherSubExprs();

            // Skip some elements because they should be handled by other processes.
            // For example:
            // - parameters should be handled by their host methods.
            // - members of inline-type should be handled by the element which introduces the inline-type.
            if (Type == EType.InlineTypeMember)
            {
                // TODO impl
                throw new NotImplementedException();
            }
            else if (Type == EType.Member
                || Type == EType.MethodSubParam
                )
            {
                var elemDef = _ParsedResult as STNode_ElementDef;

                // Let TypeSig to find the target TypeInfo.
                TypeInfo typeInfo = CommonTypeInfos.Any;
                if (elemDef.TypeSig != null)
                {
                    typeInfo = elemDef.TypeSig.LocateTypeInfo(InProjectInfo, parentInfo);
                }

                // select header name by Type
                string headerName = "";
                switch (Type)
                {
                    case EType.Member: headerName = "member"; break;
                    case EType.ComponentOrChild: headerName = "child"; break;
                    case EType.MethodSubParam: headerName = "param"; break;
                    default:
                        throw new InvalidProgramException("Unexpected Element Type");
                }

                // new the result element.
                chiefInfo = new ElementInfo(parentInfo, headerName, elemDef.DefName, typeInfo, elemDef.InitExpression);
            }
            else if (Type == EType.Method
                || Type == EType.Event
                )
            {
                // ## Let TypeSig to find the method's return TypeInfo.
                TypeInfo typeInfo = CommonTypeInfos.Any;
                if (FuncDef.TypeSig != null)
                {
                    typeInfo = FuncDef.TypeSig.LocateTypeInfo(InProjectInfo, parentInfo);
                }

                // ## Merge init-expr with sub expressions.
                if (FuncDef.InitExpression != null)
                {
                    if (FuncDef.InitExpression is syntaxtree.STNodeSequence)
                    {
                        subExprs.InsertRange(0, (FuncDef.InitExpression as syntaxtree.STNodeSequence).NodeList);
                    }
                    else
                    {
                        subExprs.Insert(0, FuncDef.InitExpression);
                    }
                }
                syntaxtree.STNodeSequence mergedSTSeq = new syntaxtree.STNodeSequence(subExprs.ToArray());

                // ## New MethodInfo instance.
                string headerName = "";
                switch (Type)
                {
                    case EType.Method: headerName = "method"; break;
                    case EType.Event: headerName = "event"; break;
                    default:
                        throw new InvalidProgramException("Unexpected Method Type");
                }
                chiefInfo = new ElementInfo(parentInfo, headerName, FuncDef.DefName, typeInfo, mergedSTSeq);

                // ## Fill parameters by this sector
                foreach (var paramDef in FuncDef.Params)
                {
                    _GenerateParamByDef(InProjectInfo, chiefInfo, paramDef);
                }
            }

            // Register inline attributes
            if (chiefInfo != null)
            {
                foreach (var attrDef in _ParsedResult.Attributes)
                {
                    var attrInfo = new AttributeInfo(chiefInfo, attrDef.DefName, attrDef.DefName, attrDef.InitExpression);
                }
            }

            return chiefInfo;
        }

        /// <summary>
        /// Generate ParamInfo by STNode_ElementDef.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InFuncInfo"></param>
        /// <param name="InSTElemDef"></param>
        private static void _GenerateParamByDef(ProjectInfo InProjectInfo, Info InFuncInfo, STNode_ElementDef InSTElemDef)
        {
            TypeInfo paramTypeInfo = CommonTypeInfos.Any;
            if (InSTElemDef.TypeSig != null)
            {
                paramTypeInfo = InSTElemDef.TypeSig.LocateTypeInfo(InProjectInfo, InFuncInfo);
            }
            ElementInfo paramInfo = new ElementInfo(InFuncInfo, "param", InSTElemDef.DefName, paramTypeInfo, InSTElemDef.InitExpression);
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
