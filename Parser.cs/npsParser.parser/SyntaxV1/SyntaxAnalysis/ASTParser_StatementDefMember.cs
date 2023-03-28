using System;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Try parse tokens as a member-def statement.
    /// 
    /// Like element definitions:
    ///     -n [Min=0][Max=100] HP = 100
    ///     - HP:n = 100
    /// 
    /// or parameter definitions:
    ///     SomeMethod(InParam0:n = 10, InParam1:s = "id")
    ///                ^-------------^  ^---------------^
    /// </summary>
    class ASTParser_StatementDefMember
        : ASTParser_Composite<STNode_ElementDef>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="InStartTypeCode">
        /// The start type like 'integer' in "-integer HP = 100".
        /// Pass null means there are no start type codes. e.g. "- HP = 100" or "-HP = 100".
        /// </param>
        /// <param name="InParseLineEndBlocks">
        /// Determine if the parser handles line-end attributes and line-end comments.
        /// like "-HP = 100 @Min=0 # Health point."
        /// </param>
        /// <exception cref="NotImplementedException"></exception>
        public ASTParser_StatementDefMember(STNode_TypeSignature InStartType, bool InParseLineEndBlocks)
        {
            // - HP    :integer = 100 @Min=0 @Max=100     # Health-point
            //   ^^    ^------^ ^---^ ^-------------^     ^------------^
            // BlockID  TypeSig  Expr LineEndAttributes   LineEndComments

            AddSubParsers(new ASTParser_BlockID(),
                stnGetVar =>
                {
                    _Result = new STNode_ElementDef(stnGetVar.IDName);
                    if (InStartType != null)
                    { _Result._Internal_SetType(InStartType); }
                },
                () =>
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return false;
                }
                );

            if (InStartType == null)
            {
                AddSubParsers(new ASTParser_BlockTypeDef(),
                    typeSig => _Result._Internal_SetType(typeSig)
                    );
            }

            AddSubParsers(new ASTParser_BlockInitExpr(),
                expr => _Result._Internal_SetInitExpr(expr)
                );

            if (InParseLineEndBlocks)
            {
                AddSubParsers(new ASTParser_BlockLineEndAttributes(),
                    attrs => _Result._Internal_AddAttributes(attrs)
                    );

                AddSubParsers(new ASTParser_BlockLineEndComments(),
                    comments => _Result._Internal_AddComments(comments)
                    );
            }

        }
    }

}