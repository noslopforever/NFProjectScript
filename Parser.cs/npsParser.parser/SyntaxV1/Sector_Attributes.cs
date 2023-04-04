using nf.protoscript.parser.syntax1.analysis;
using nf.protoscript.parser.token;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Attribute sector like @Attr=Value or @Attr={P0=Value, P1=Value}
    /// </summary>
    class AttributesSector
        : Sector
    {
        public AttributesSector(CodeLine InCodeLn, STNode_AttributeDefs InAttrs, STNode_Comment InComment)
            : base(InCodeLn)
        {
            AttrDefs = InAttrs;
            CommentDefs = InComment;
        }

        /// <summary>
        /// AttributeDefs parsed by factory.
        /// </summary>
        internal STNode_AttributeDefs AttrDefs { get; }

        /// <summary>
        /// Line end comments parsed by factory.
        /// </summary>
        internal STNode_Comment CommentDefs { get; }

        protected override Info CollectInfosImpl(ProjectInfo InProjectInfo, Sector InParentSector)
        {
            if (AttrDefs == null)
            {
                return null;
            }

            Info parentInfo = InParentSector.CollectedInfo;
            if (parentInfo == null)
            {
                throw new ParserException(
                    ParserErrorType.Collect_NoParentInfo
                    , CodeLn
                    );
            }

            // register attributes to the parent Info.
            // TODO is sub-infos describe all attributes or only the last attribute?
            AttributeInfo lastInfo = null;
            foreach (var attrDef in AttrDefs)
            {
                var attrInfo = new AttributeInfo(parentInfo, attrDef.DefName, attrDef.DefName, attrDef.InitExpression);
                // TODO impl
                //throw new NotImplementedException();
                //new CommentInfo(attrInfo, comment);

                lastInfo = attrInfo;
            }

            return lastInfo;
        }
    }


}
