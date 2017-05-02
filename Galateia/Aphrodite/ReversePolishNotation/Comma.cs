using System;

namespace Aphrodite.ReversePolishNotation
{
    /// <summary>
    ///     コンマ：関数の引数区切り
    /// </summary>
    public class Comma : IToken
    {
        public Comma(string token)
        {
            Token = token;
            if (Token != ",")
                throw new ArgumentException("Unknown comma: " + token);
        }

        public TokenTypes Type
        {
            get { return TokenTypes.Comma; }
        }

        public string Token { get; private set; }
    }
}