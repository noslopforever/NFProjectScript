using System;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Default implementation of a token.
    /// </summary>
    public class Token
        : IToken
    {
        public Token(string InTokenTypeCode, string InCode, string InDebugOutput)
        {
            TokenType = InTokenTypeCode;
            Code = InCode;
            DebugOutput = InDebugOutput;
        }

        /// <inheritdoc />
        public string TokenType { get; }

        /// <inheritdoc />
        public string Code { get; }

        /// <inheritdoc />
        public string DebugOutput { get; }

        /// <inheritdoc />
        public bool CheckToken(TokenChecker InChecker)
        {
            if (0 != string.Compare(InChecker.TokenType, TokenType, true))
            {
                return false;
            }

            foreach (var specCode in InChecker.SpecificTokenCodes)
            {
                if (0 == string.Compare(Code, specCode, InChecker.IgnoreCase))
                {
                    return true;
                }
            }
            return true;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0}['{1}']", TokenType.ToString(), Code);
        }

    }



}