namespace nf.protoscript.parser.token
{
    /// <summary>
    /// Token
    /// </summary>
    public class Token
    {
        public Token(ETokenType InTokenType, string InCode, int InLenToTheEnd)
        {
            TokenType = InTokenType;
            Code = InCode;
            LengthToTheEnd = InLenToTheEnd;
        }

        /// <summary>
        /// Type of token
        /// </summary>
        public ETokenType TokenType { get; }

        /// <summary>
        /// Token's code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Length to the string's end.
        /// </summary>
        public int LengthToTheEnd { get; }

        public override string ToString()
        {
            return string.Format("{0}['{1}']", TokenType.ToString(), Code);
        }
    }



}