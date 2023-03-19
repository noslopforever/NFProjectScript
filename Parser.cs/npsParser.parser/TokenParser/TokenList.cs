﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace nf.protoscript.parser.token
{

    /// <summary>
    /// TokenList: to help accessing and iterating tokens.
    /// </summary>
    class TokenList
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
            var token = Tokens[_TokenIndex];
            _TokenIndex += 1;
            return token;
        }

        /// <summary>
        /// Check if the target token matches InCheckTypes.
        /// </summary>
        /// <param name="InCheckTypes"></param>
        /// <param name="InDelta"></param>
        /// <returns></returns>
        public bool CheckToken(IEnumerable<ETokenType> InCheckTypes, int InDelta = 0)
        {
            Token curToken = GetToken(InDelta);
            if (curToken == null)
            { return false; }

            if (InCheckTypes.Contains(curToken.TokenType))
            { return true; }

            return false;
        }

        /// <summary>
        /// Check if the target token matches the InTokenType.
        /// </summary>
        /// <param name="InTokenType"></param>
        /// <param name="InDelta"></param>
        /// <returns></returns>
        public bool CheckToken(ETokenType InTokenType, int InDelta = 0)
        {
            return CheckToken(new ETokenType[] { InTokenType }, InDelta);
        }

        /// <summary>
        /// Check if the next token matches the InTokenType
        /// </summary>
        /// <param name="InTokenType"></param>
        /// <returns></returns>
        public bool CheckNextToken(ETokenType InTokenType)
        {
            return CheckToken(InTokenType, 1);
        }

        /// <summary>
        /// Try consume the current token if it matches InToken.
        /// If failed, consume all tokens until the current token equals to the InToken.
        /// </summary>
        /// <param name="InToken"></param>
        /// <returns></returns>
        public bool EnsureConsumeTheToken(ETokenType InToken)
        {
            if (CheckToken(InToken))
            {
                Consume();
                return true;
            }

            // TODO log error: No expected tokens.
            throw new NotImplementedException();

            ConsumeTo(InToken);

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


    }


}