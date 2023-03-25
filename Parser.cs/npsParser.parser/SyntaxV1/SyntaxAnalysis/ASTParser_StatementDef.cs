using System;
using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1.analysis
{

    /// <summary>
    /// Try parse tokens as a CommonDef statement.
    /// 
    /// Like element definitions:
    ///     -n [Min=0][Max=100] HP = 100
    ///     -n [Pure] getHP() = return 100
    ///     
    ///     - HP:n = 100
    ///     +OverrideHP = 100
    ///     -getHP():n = return 100
    /// or parameter definitions:
    ///     SomeMethod(InParam0:n = 10, InParam1:s = "id")
    ///                ^-------------^  ^---------------^
    /// </summary>
    class ASTParser_StatementDef
        : ASTParser_Base<STNode_DefBase>
    {
        public ASTParser_StatementDef(EDefType InDefType, ESyntaxType InParseMode = ESyntaxType.ParsePostType)
        {
            DefType = InDefType;
            SyntaxType = InParseMode;
        }

        /// <summary>
        /// Result type of the definition.
        /// </summary>
        public enum EDefType
        {
            /// <summary>
            /// Try parse an element definition.
            /// </summary>
            Element,

            /// <summary>
            /// Try parse a function definition.
            /// </summary>
            Function,

            /// <summary>
            /// Try parse a parameter definition which appears in '()' blocks and does NOT have any line-end attributes and comments.
            /// </summary>
            Param,

        }

        /// <summary>
        /// Result type of the definition.
        /// </summary>
        public EDefType DefType { get; }


        /// <summary>
        /// Type of the syntax parsed by this parser.
        /// </summary>
        public enum ESyntaxType
        {
            /// <summary>
            /// Parse the definition in Pre-Type mode which starts with a type-block.
            /// -integer HP = 100
            ///  ^-----^
            /// </summary>
            ParseStartType,

            /// <summary>
            /// Parse the definition in Pre-Empty-Type mode.
            /// - HP = 100
            /// + getName() = return "Richard"
            /// </summary>
            EmptyStartType,

            /// <summary>
            /// Parse the definition in Post-Type mode:
            /// -HP:integer = 100
            /// +getName():string = return "Richard"
            /// </summary>
            ParsePostType,

        }

        /// <summary>
        /// Mode of the parser.
        /// </summary>
        public ESyntaxType SyntaxType { get; }


        public override STNode_DefBase Parse(TokenList InTokenList)
        {
            // sub blocks.
            ASTParser_BlockType typeBlockParser = new ASTParser_BlockType();

            STNode_TypeSignature typeSig = null;

            // If in Pre-Type mode, try parse type-block at first
            // -n [Min=0][Max=100] HP = 100.
            //  ^
            if (SyntaxType == ESyntaxType.ParseStartType)
            {
                typeSig = typeBlockParser.Parse(InTokenList);
                if (typeSig == null)
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
            }

            // Try parse prefix attributes:
            // -n [Min=0][Max=100] HP = 100.
            //    ^--------------^
            ASTParser_BlockInlineAttributes attrsParser = new ASTParser_BlockInlineAttributes();
            STNode_AttributeDefs attrs = attrsParser.Parse(InTokenList);

            // Try parse the definition body, switched by the desired DefType.
            STNode_DefBase resultDef = null;
            if (DefType == EDefType.Function)
            {
                // try parse function-def
                // -n getHP()
                //    ^-----^
                //
                ASTParser_BlockFunctionDef funcDefParser = new ASTParser_BlockFunctionDef();
                resultDef = funcDefParser.Parse(InTokenList);
            }
            else if (DefType == EDefType.Element
                || DefType == EDefType.Param
                )
            {
                // ... then try parse element-def
                // -n HP
                //    ^^
                ASTParser_BlockElemDef elemDefParser = new ASTParser_BlockElemDef();
                resultDef = elemDefParser.Parse(InTokenList);
            }

            if (resultDef == null)
            {
                // TODO log error
                throw new NotImplementedException();
                return null;
            }

            // If Post-Type mode, try parse type signature here.
            // -HP:n
            //    ^^
            if (InTokenList.CheckToken(ETokenType.Colon))
            {
                InTokenList.Consume();
                typeSig = typeBlockParser.Parse(InTokenList);
                if (typeSig == null)
                {
                    // TODO log error
                    throw new NotImplementedException();
                    return null;
                }
            }

            // Try parse init-expressions.
            if (InTokenList.CheckToken(ETokenType.Assign, "="))
            {
                syntaxtree.STNodeBase initExpr = null;
                if (DefType == EDefType.Function)
                {
                    ASTParser_BlockInlineFunctionBody initFuncParser = new ASTParser_BlockInlineFunctionBody();
                    initExpr = initFuncParser.Parse(InTokenList);
                    if (initExpr == null)
                    {
                        // TODO log error
                        throw new NotImplementedException();
                        return null;
                    }
                }
                else
                {
                    ASTParser_BlockInitExpr initExprParser = new ASTParser_BlockInitExpr();
                    initExpr = initExprParser.Parse(InTokenList);
                    if (initExpr == null)
                    {
                        // TODO log error
                        throw new NotImplementedException();
                        return null;
                    }
                }

                // Assign init-expressions.
                resultDef._Internal_SetInitExpr(initExpr);
            }

            // If Non-Param mode, Try parse line-end attributes and comment.
            STNode_AttributeDefs postAttrs = null;
            STNode_Comment comment = null;
            if (DefType != EDefType.Param)
            {
                ASTParser_BlockLineEndAttributes leAttrsParser = new ASTParser_BlockLineEndAttributes();
                postAttrs = leAttrsParser.Parse(InTokenList);
                
                ASTParser_BlockLineEndComments lecmtsParser = new ASTParser_BlockLineEndComments();
                comment = lecmtsParser.Parse(InTokenList);
            }

            // Assign type and attributes which have already been parsed
            if (typeSig != null)
            { resultDef._Internal_SetType(typeSig); }
            if (attrs != null)
            { resultDef._Internal_AddAttributes(attrs); }
            if (postAttrs != null)
            { resultDef._Internal_AddAttributes(postAttrs); }
            if (comment != null)
            { resultDef._Internal_AddComments(comment); }

            return resultDef;
        }

    }

}