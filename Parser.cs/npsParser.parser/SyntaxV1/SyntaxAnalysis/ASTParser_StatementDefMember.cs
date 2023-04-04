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
        public ASTParser_StatementDefMember(STNode_TypeSignature InStartType)
        {
            StartType = InStartType;
        }

        /// <summary>
        /// The start type like 'integer' in "-integer HP = 100".
        /// Pass null means there are no start type codes. e.g. "- HP = 100" or "-HP = 100".
        /// </summary>
        public STNode_TypeSignature StartType { get; }

        public override STNode_ElementDef Parse(TokenList InTokenList)
        {
            // - [UIMin = 0][UIMax = 100]   HP    :integer = 100
            //   ^----------------------^   ^^    ^------^ ^---^
            //   InlineAttributes        BlockID  TypeSig  Expr
            if (!InTokenList.CheckToken(ETokenType.ID)
                && !InTokenList.CheckToken(ETokenType.OpenBracket)
                )
            {
                return null;
            }

            // Try parse inline-attributes
            STNode_AttributeDefs inlineAttrs = null;
            StaticParseAST(new ASTParser_BlockInlineAttributes(), InTokenList,
                attrs => inlineAttrs = attrs
                );

            // Parse block ID, create element-definition by it, and set inline-attributes/StartType
            var idToken = InTokenList.Consume();
            var result = new STNode_ElementDef(idToken.Code);
            if (inlineAttrs != null)
            {
                result._Internal_AddAttributes(inlineAttrs);
            }

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

            return result;
        }
    }

}