using System.Collections.Generic;

namespace nf.protoscript.parser.token
{
    /// <summary>
    /// Common NPS token factory.
    /// </summary>
    internal class TokenParser_CommonNps : TokenParser
    {
        public TokenParser_CommonNps()
            : base(
                  (ETokenType.At, new string[] { CommonTokenRegexes.At }),
                  (ETokenType.Sharp, new string[] { CommonTokenRegexes.Sharp }),
                  (ETokenType.String, new string[]
                  {
                      CommonTokenRegexes.DoubleQuoteString,
                      CommonTokenRegexes.QuoteString,
                      CommonTokenRegexes.ApostropheString,
                  }
                  ),
                  (
                  ETokenType.Floating, new string[]
                  {
                      CommonTokenRegexes.FloatingNumber,
                      CommonTokenRegexes.EFloatingNumber,
                  }
                  ),
                  (ETokenType.Integer, new string[] { CommonTokenRegexes.Integer }),
                  (ETokenType.ID, new string[] { CommonTokenRegexes.Identity }),
                  (ETokenType.Comma, new string[] { CommonTokenRegexes.Comma }),
                  (ETokenType.NextLine, new string[] { CommonTokenRegexes.BackwardSlash }),
                  (ETokenType.Ellipsis, new string[] { CommonTokenRegexes.Ellipsis }),
                  (ETokenType.Operator, new string[] { CommonTokenRegexes.OperatorsAndDot }),
                  (ETokenType.Assign, new string[] { CommonTokenRegexes.Assign }),
                  (ETokenType.Colon, new string[] { CommonTokenRegexes.Colon }),
                  (ETokenType.Skip, new string[] { CommonTokenRegexes.Skips }),
                  (ETokenType.OpenParen, new string[] { CommonTokenRegexes.OpenParen }),
                  (ETokenType.CloseParen, new string[] { CommonTokenRegexes.CloseParen }),
                  (ETokenType.OpenBracket, new string[] { CommonTokenRegexes.OpenBracket }),
                  (ETokenType.CloseBracket, new string[] { CommonTokenRegexes.CloseBracket }),
                  (ETokenType.OpenBrace, new string[] { CommonTokenRegexes.OpenBrace }),
                  (ETokenType.CloseBrace, new string[] { CommonTokenRegexes.CloseBrace })
            )
        { }

        public static TokenParser_CommonNps Instance { get; } = new TokenParser_CommonNps();
    }


}