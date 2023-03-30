using nf.protoscript.parser.token;
using System;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Try parse tokens as a function-def statement.
    /// 
    /// Like method definitions:
    ///     +n getHP = return 100
    ///     + getHP():n = return 100
    /// 
    /// </summary>
    class ASTParser_StatementDefFunction
        : ASTParser_Base<STNode_FunctionDef>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ASTParser_StatementDefFunction(STNode_TypeSignature InStartType)
        {
            StartType = InStartType;
        }

        /// <summary>
        /// The start type like 'integer' in "-integer HP = 100".
        /// Pass null means there are no start type codes. e.g. "- HP = 100" or "-HP = 100".
        /// </summary>
        public STNode_TypeSignature StartType { get; }

        public override STNode_FunctionDef Parse(TokenList InTokenList)
        {
            // - getHP  (     )      :integer     = return 100        @Min=0 @Max=100        # Health-point
            //   ^------------^      ^------^     ^----------^        ^-------------^        ^------------^
            //   BlockFunctionDef    [o]TypeSig   [o]Expr-Statement   [o]LineEndAttributes   [o]LineEndComments
            //
            if (!InTokenList.CheckToken(ETokenType.ID))
            {
                return null;
            }

            // Parse function def
            STNode_FunctionDef result = new ASTParser_BlockFunctionDef().Parse(InTokenList);
            if (result == null)
            {
                // TODO log error
                throw new NotImplementedException();
            }

            // Handle StartType/PostType.
            if (StartType != null)
            {
                result._Internal_SetType(StartType);
            }
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

            // Parse inline function body (expression statement)
            StaticParseAST(new ASTParser_BlockInlineFunctionBody(), InTokenList,
                expr => result._Internal_SetInitExpr(expr)
                );

            // Parse line-end blocks
            StaticParseAST(new ASTParser_BlockLineEndAttributes(), InTokenList,
                attrs => result._Internal_AddAttributes(attrs)
                );

            StaticParseAST(new ASTParser_BlockLineEndComments(), InTokenList,
                comments => result._Internal_AddComments(comments)
                );

            // if not end, there is an unexpected token
            if (!InTokenList.IsEnd)
            {
                // TODO log error
                throw new NotImplementedException();
                return null;
            }

            return result;
        }
    }

}