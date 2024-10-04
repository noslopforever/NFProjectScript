using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Parser to parse parameter-list.
    /// 
    /// Example:
    /// -n [Pure] getHP(InParam0, InParam1) = return 100
    ///                ^------------------^
    /// 
    /// </summary>
    class ASTParser_BlockParamList
        : ASTParser_ChildListBase<STNode_ElementDefs, STNode_ElementDefs>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ASTParser_BlockParamList"/> class.
        /// </summary>
        public ASTParser_BlockParamList()
            : base(CommonTokenTypes.OpenParen, CommonTokenTypes.CloseParen, CommonTokenTypes.Comma)
        {
        }

        /// <inheritdoc />
        protected override void ParseAndAddSubSTNode(STNode_ElementDefs InResult, IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
        {
            // Gets the starting token for the parameter definition.
            var paramDefStartToken = InTokens[RefStartIndex];

            // Parse the parameter definition.
            var paramDef = ASTParser_BlockDefParam.StaticParse(InTokens, ref RefStartIndex);
            if (paramDef == null)
            {
                // If the parameter definition is not valid, throw an exception.
                throw new ParserException(
                    ParserErrorType.AST_InvalidParam
                    , paramDefStartToken
                    );
            }

            // Add the parsed parameter definition to the result list.
            InResult.Add(paramDef);
        }

    }


    ///// <summary>
    ///// Parser to parse parameter-list.
    ///// 
    ///// -n [Pure] getHP(InParam0, InParam1) = return 100
    /////                ^------------------^
    ///// 
    ///// </summary>
    //class ASTParser_BlockParamList
    //    : ASTParser_Base<STNode_ElementDefs>
    //{
    //    public override STNode_ElementDefs Parse(IReadOnlyList<IToken> InTokens, ref int RefStartIndex)
    //    {
    //        STNode_ElementDefs paramDefs = new STNode_ElementDefs();

    //        // Handle parameter lists if have.
    //        // -n getSth(InParam0, InParam1)
    //        //          ^------------------^
    //        //
    //        if (InTokens[RefStartIndex].Check(CommonTokenTypes.OpenParen))
    //        {
    //            // consume the '('.
    //            RefStartIndex++;

    //            // Check -n getSth( )
    //            //                  ^
    //            if (InTokens[RefStartIndex].Check(CommonTokenTypes.CloseParen))
    //            {
    //                RefStartIndex++;
    //            }
    //            else
    //            {
    //                while (true)
    //                {
    //                    // -n getSth(InParam0, InParam1)
    //                    //                             ^
    //                    // If matches the end of the TL, mark failed and break.
    //                    if (RefStartIndex == InTokens.Count)
    //                    {
    //                        throw new ParserException(ParserErrorType.AST_UnexpectedEnd);
    //                        paramDefs = null;
    //                        break;
    //                    }

    //                    var paramDefStartToken = InTokens[RefStartIndex];
    //                    var paramDefParser = new ASTParser_BlockDefParam();
    //                    var paramDef = paramDefParser.Parse(InTokens, ref RefStartIndex);
    //                    if (paramDef == null)
    //                    {
    //                        throw new ParserException(
    //                            ParserErrorType.AST_InvalidParam
    //                            , paramDefStartToken
    //                            );
    //                    }
    //                    paramDefs.Add(paramDef);

    //                    // -n getSth(InParam0, InParam1)
    //                    //                   ^         ^
    //                    if (InTokens[RefStartIndex].Check(CommonTokenTypes.Comma))
    //                    {
    //                        // -n getSth(InParam0, InParam1)
    //                        //                   ^
    //                        ++RefStartIndex;
    //                    }
    //                    else if (InTokens[RefStartIndex].Check(CommonTokenTypes.CloseParen))
    //                    {
    //                        // Ensure and consume the 'close-paren'.
    //                        // -n getSth(InParam0, InParam1)
    //                        //                             ^
    //                        ++RefStartIndex;
    //                        break;
    //                    }
    //                    else
    //                    {
    //                        throw new ParserException(
    //                            ParserErrorType.AST_UnexpectedToken
    //                            , InTokens[RefStartIndex]
    //                            , $"{CommonTokenTypes.Comma}|{CommonTokenTypes.CloseParen}"
    //                            );
    //                    }
    //                } // ~ While true
    //            } // ~ END else

    //        }

    //        return paramDefs;
    //    }
    //}

}