namespace nf.protoscript.parser.token
{
    /// <summary>
    /// Token
    /// </summary>
    public class Token
    {
        public Token(ETokenType InTokenType, string InCode)
        {
            TokenType = InTokenType;
            Code = InCode;
        }

        /// <summary>
        /// Type of token
        /// </summary>
        public ETokenType TokenType { get; }

        /// <summary>
        /// Token's code
        /// </summary>
        public string Code { get; }

        public override string ToString()
        {
            return string.Format("{0}, CODES={1}", TokenType.ToString(), Code);
        }
    }



}