using System.Collections.Generic;
using nf.protoscript.parser.token;

namespace nf.protoscript.parser.syntax1
{
    /// <summary>
    /// Common NPS token factory.
    /// </summary>
    internal class TokenParser_CommonNps
        : TokenParser
    {
        public TokenParser_CommonNps()
            : base(
                  (ETokenType.At, TokenParser_CommonNpsTokenRegexes.At),
                  (ETokenType.Sharp, TokenParser_CommonNpsTokenRegexes.Sharp),
                  (ETokenType.String, TokenParser_CommonNpsTokenRegexes.DoubleQuoteString),
                  (ETokenType.String, TokenParser_CommonNpsTokenRegexes.QuoteString),
                  (ETokenType.String, TokenParser_CommonNpsTokenRegexes.ApostropheString),
                  (ETokenType.Floating, TokenParser_CommonNpsTokenRegexes.FloatingNumber),
                  (ETokenType.Floating, TokenParser_CommonNpsTokenRegexes.EFloatingNumber),
                  (ETokenType.Integer, TokenParser_CommonNpsTokenRegexes.Integer ),
                  (ETokenType.ID, TokenParser_CommonNpsTokenRegexes.Identity ),
                  (ETokenType.Comma, TokenParser_CommonNpsTokenRegexes.Comma ),
                  (ETokenType.NextLine, TokenParser_CommonNpsTokenRegexes.BackwardSlash ),
                  (ETokenType.Ellipsis, TokenParser_CommonNpsTokenRegexes.Ellipsis ),
                  (ETokenType.Assign, TokenParser_CommonNpsTokenRegexes.ImplementLambda ),
                  (ETokenType.Assign, TokenParser_CommonNpsTokenRegexes.CompoundAssign ),
                  (ETokenType.Operator, TokenParser_CommonNpsTokenRegexes.Operators_Equal ),
                  (ETokenType.Operator, TokenParser_CommonNpsTokenRegexes.OperatorsAndDot ),
                  (ETokenType.Assign, TokenParser_CommonNpsTokenRegexes.Assign ),
                  (ETokenType.Colon, TokenParser_CommonNpsTokenRegexes.Colon ),
                  (ETokenType.Skip, TokenParser_CommonNpsTokenRegexes.Skips ),
                  (ETokenType.OpenParen, TokenParser_CommonNpsTokenRegexes.OpenParen ),
                  (ETokenType.CloseParen, TokenParser_CommonNpsTokenRegexes.CloseParen ),
                  (ETokenType.OpenBracket, TokenParser_CommonNpsTokenRegexes.OpenBracket ),
                  (ETokenType.CloseBracket, TokenParser_CommonNpsTokenRegexes.CloseBracket ),
                  (ETokenType.OpenBrace, TokenParser_CommonNpsTokenRegexes.OpenBrace ),
                  (ETokenType.CloseBrace, TokenParser_CommonNpsTokenRegexes.CloseBrace )
            )
        { }

        public static TokenParser_CommonNps Instance { get; } = new TokenParser_CommonNps();
    }


}