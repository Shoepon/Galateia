using System;

namespace Aphrodite.ReversePolishNotation
{
    /// <summary>
    ///     単項演算子
    /// </summary>
    public class UnaryOperator : IToken, IOperator
    {
        public enum Operations
        {
            Plus,
            Minus,
        }

        public UnaryOperator(string token)
        {
            Token = token;
            switch (token)
            {
                case "+":
                    Operation = Operations.Plus;
                    break;
                case "-":
                    Operation = Operations.Minus;
                    break;
                default:
                    throw new ArgumentException("Unknown unary operator: " + token);
            }
        }

        public Operations Operation { get; private set; }

        public int Priority
        {
            get { return int.MaxValue; }
        }

        public TokenTypes Type
        {
            get { return TokenTypes.UnaryOperator; }
        }

        public string Token { get; private set; }
    }
}