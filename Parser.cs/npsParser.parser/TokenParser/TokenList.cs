using System;
using System.Collections.Generic;
using System.Linq;

namespace nf.protoscript.parser.token
{

    /// <summary>
    /// TokenList: to help accessing and iterating tokens.
    /// </summary>
    public class TokenList
    {
        public TokenList(IReadOnlyList<Token> InTokens)
        {
            Tokens = InTokens;
        }

        /// <summary>
        /// Tokens
        /// </summary>
        public IReadOnlyList<Token> Tokens { get; private set; }

        /// <summary>
        /// Access index.
        /// </summary>
        private int _TokenIndex = 0;

        /// <summary>
        /// Is seek end?
        /// </summary>
        public bool IsEnd { get { return _TokenIndex == Tokens.Count; } }

        /// <summary>
        /// Get current token
        /// </summary>
        public Token CurrentToken { get { return GetToken(0); } }

        /// <summary>
        /// Get the next token
        /// </summary>
        public Token NextToken { get { return GetToken(1); } }

        /// <summary>
        /// Seek to the target index.
        /// </summary>
        /// <param name="InIndex"></param>
        /// <returns></returns>
        public Token Seek(int InIndex)
        {
            _TokenIndex = InIndex;
            return CurrentToken;
        }

        /// <summary>
        /// Get token with delta
        /// </summary>
        /// <param name="InDelta"></param>
        /// <returns></returns>
        Token GetToken(int InDelta = 0)
        {
            var index = _TokenIndex + InDelta;
            if (index < Tokens.Count)
                return Tokens[index];
            return null;
        }

        /// <summary>
        /// "consume" a token
        /// </summary>
        /// <returns></returns>
        public Token Consume()
        {
            if (_TokenIndex >= Tokens.Count)
            { return null; }

            var token = Tokens[_TokenIndex];
            _TokenIndex += 1;
            return token;
        }

        /// <summary>
        /// Check if token matches the InType/InCheckCodes.
        /// </summary>
        /// <param name="InCheckingToken"></param>
        /// <param name="InType"></param>
        /// <param name="InCheckCodes"></param>
        /// <returns></returns>
        public static bool CheckTokenMatch(Token InCheckingToken, ETokenType InType, string[] InCheckCodes)
        {
            if (InCheckingToken.TokenType == InType)
            {
                if (InCheckCodes == null || InCheckCodes.Length == 0)
                {
                    return true;
                }

                return InCheckCodes.Contains(InCheckingToken.Code);
            }
            return false;
        }

        /// <summary>
        /// Check if the target token matches InCheckTypes.
        /// </summary>
        /// <param name="InCheckTypes"></param>
        /// <param name="InCodes"></param>
        /// <param name="InDelta"></param>
        /// <returns></returns>
        public bool CheckTokens(IEnumerable<(ETokenType, string[])> InCheckTypeAndCodes, int InDelta = 0)
        {
            Token curToken = GetToken(InDelta);
            if (curToken == null)
            {
                return false;
            }

            foreach (var tnc in InCheckTypeAndCodes)
            {
                if (CheckTokenMatch(curToken, tnc.Item1, tnc.Item2))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check types
        /// </summary>
        /// <param name="InCheckTypes"></param>
        /// <param name="InDelta"></param>
        /// <returns></returns>
        public bool CheckTokens(IEnumerable<ETokenType> InCheckTypes, int InDelta = 0)
        {
            var checkTypeAndCodes = new List<(ETokenType, string[])>();
            foreach (var ct in InCheckTypes)
            {
                checkTypeAndCodes.Add((ct, new string[0]));
            }
            return CheckTokens(checkTypeAndCodes, InDelta);
        }

        /// <summary>
        /// Check if the target token matches the InTokenType.
        /// </summary>
        /// <param name="InTokenType"></param>
        /// <param name="InCodes"></param>
        /// <param name="InDelta"></param>
        /// <returns></returns>
        public bool CheckToken(ETokenType InTokenType, string[] InCodes, int InDelta = 0)
        {
            return CheckTokens(new (ETokenType, string[])[] { (InTokenType, InCodes) }, InDelta);
        }

        /// <summary>
        /// Check if the target token matches the InTokenType.
        /// </summary>
        /// <param name="InTokenType"></param>
        /// <param name="InCode"></param>
        /// <param name="InDelta"></param>
        /// <returns></returns>
        public bool CheckToken(ETokenType InTokenType, string InCode = null, int InDelta = 0)
        {
            var checkCodes = InCode != null ? new string[] { InCode } : new string[0];
            return CheckToken(InTokenType, checkCodes, InDelta);
        }

        /// <summary>
        /// Check if the next token matches the InTokenType
        /// </summary>
        /// <param name="InTokenType"></param>
        /// <param name="InCode"></param>
        /// <returns></returns>
        public bool CheckNextToken(ETokenType InTokenType, string InCode = null)
        {
            return CheckToken(InTokenType, InCode, 1);
        }

        /// <summary>
        /// Check if the current token matches InToken.
        /// If matches, return true immediately.
        /// If not, consume all tokens until the current token matches to the InTokens, then return false.
        /// </summary>
        /// <param name="InToken"></param>
        /// <returns></returns>
        public bool EnsureOrConsumeTo(ETokenType InToken)
        {
            if (CheckToken(InToken))
            {
                return true;
            }

            // TODO log error: No expected tokens.
            throw new NotImplementedException();

            ConsumeTo(InToken);
            return false;
        }

        /// <summary>
        /// Check if the current token contains in InTokens.
        /// If contains, return true immediately.
        /// If not, consume all tokens until the current token contains in InTokens, then return false.
        /// </summary>
        /// <param name="InToken"></param>
        /// <returns></returns>
        public bool EnsureOrConsumeTo(IEnumerable<ETokenType> InTokens)
        {
            if (CheckTokens(InTokens))
            {
                return true;
            }

            // TODO log error: No expected tokens.
            throw new NotImplementedException();

            ConsumeTo(InTokens);

            return false;
        }

        /// <summary>
        /// Consume all tokens until the current token equals to the InToken.
        /// </summary>
        /// <param name="InToken"></param>
        private void ConsumeTo(ETokenType InToken)
        {
            // Consume to the next 'InToken' or the end.
            while (CurrentToken != null && CurrentToken.TokenType != InToken)
            {
                Consume();
            }
        }

        /// <summary>
        /// Consume all tokens until the current token contains in the InTokens.
        /// </summary>
        /// <param name="InToken"></param>B
        private void ConsumeTo(IEnumerable<ETokenType> InTokens)
        {
            // Consume to the next 'InToken' or the end.
            while (CurrentToken != null
                && !InTokens.Contains(CurrentToken.TokenType)
                )
            {
                Consume();
            }
        }



    }


}