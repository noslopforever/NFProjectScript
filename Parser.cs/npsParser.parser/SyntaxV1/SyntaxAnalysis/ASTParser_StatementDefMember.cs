using nf.protoscript.parser.token;
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
        : ASTParser_Base<STNode_ElementDef>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ASTParser_StatementDefMember(STNode_TypeSignature InStartType, bool InParseLineEndBlocks)
        {
            StartType = InStartType;
            ParseLineEndBlocks = InParseLineEndBlocks;
        }

        /// <summary>
        /// The start type like 'integer' in "-integer HP = 100".
        /// Pass null means there are no start type codes. e.g. "- HP = 100" or "-HP = 100".
        /// </summary>
        public STNode_TypeSignature StartType { get; }

        /// <summary>
        /// Determine if the parser handles line-end attributes and line-end comments.
        /// like "-HP = 100 @Min=0 # Health point."
        /// </summary>
        public bool ParseLineEndBlocks { get; }


        public override STNode_ElementDef Parse(TokenList InTokenList)
        {
            // - HP    :integer = 100 @Min=0 @Max=100     # Health-point
            //   ^^    ^------^ ^---^ ^-------------^     ^------------^
            // BlockID  TypeSig  Expr LineEndAttributes   LineEndComments

            if (!InTokenList.CheckToken(ETokenType.ID))
            {
                return null;
            }

            // Parse block ID
            var idToken = InTokenList.Consume();
            var result = new STNode_ElementDef(idToken.Code);
            if (StartType != null)
            {
                result._Internal_SetType(StartType);
            }
            // Try parse TypeSig
            else
            {
                if (InTokenList.CheckToken(ETokenType.Colon))
                {
                    InTokenList.Consume();
                    StaticParseAST(new ASTParser_BlockType(), InTokenList,
                        typeSig => result._Internal_SetType(typeSig)
                        );
                }
            }

            // Try parse expr
            StaticParseAST(new ASTParser_BlockInitExpr(), InTokenList,
                expr => result._Internal_SetInitExpr(expr)
                );

            // Try parse line-end blocks if needed.
            if (ParseLineEndBlocks)
            {
                StaticParseAST(new ASTParser_BlockLineEndAttributes(), InTokenList,
                    attrs => result._Internal_AddAttributes(attrs)
                    );

                StaticParseAST(new ASTParser_BlockLineEndComments(), InTokenList,
                    comments => result._Internal_AddComments(comments)
                    );
            }

            return result;
        }
    }

}