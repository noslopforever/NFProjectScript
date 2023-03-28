using System;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Try parse tokens as a function-def statement.
    /// 
    /// Like method definitions:
    ///     +n [Min=0][Max=100] getHP = return 100
    ///     + getHP():n = return 100
    /// 
    /// or event definitions:
    ///     >> OnClick(MousePt, Ray, Hit)
    ///     >> OnNotify
    /// 
    /// </summary>
    class ASTParser_StatementDefFunction
        : ASTParser_Composite<STNode_FunctionDef>
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
        public ASTParser_StatementDefFunction(STNode_TypeSignature InStartType)
        {
            // - getHP  (     )      :integer     = return 100        @Min=0 @Max=100        # Health-point
            //   ^^     ^-----^      ^------^     ^----------^        ^-------------^        ^------------^
            // BlockID  [o]Params    [o]TypeSig   [o]Expr-Statement   [o]LineEndAttributes   [o]LineEndComments
            //
            AddSubParsers(new ASTParser_BlockID(),
                stnGetVar => _Name = stnGetVar.IDName,
                () =>
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return false;
                }
                );

            AddSubParsers(new ASTParser_BlockParamList(),
                paramList =>
                {
                    _Result = new STNode_FunctionDef(_Name, paramList);

                    if (InStartType != null)
                    { _Result._Internal_SetType(InStartType); }
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

            AddSubParsers(new ASTParser_BlockLineEndAttributes(),
                attrs => _Result._Internal_AddAttributes(attrs)
                );

            AddSubParsers(new ASTParser_BlockLineEndComments(),
                comments => _Result._Internal_AddComments(comments)
                );

        }

        private string _Name;

    }

}