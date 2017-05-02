using System;

namespace Aphrodite.ReversePolishNotation
{
    /// <summary>
    ///     括弧
    /// </summary>
    public class Parenthesis : IToken
    {
        public enum Directions
        {
            Left,
            Right
        }

        public Parenthesis(string token)
        {
            Token = token;
            switch (token)
            {
                case "(":
                    Direction = Directions.Left;
                    break;
                case ")":
                    Direction = Directions.Right;
                    break;
                default:
                    throw new ArgumentException("Unknown parenthesis: " + token);
            }
        }

        public Directions Direction { get; private set; }

        public TokenTypes Type
        {
            get { return TokenTypes.Parenthesis; }
        }

        public string Token { get; private set; }
    }
}