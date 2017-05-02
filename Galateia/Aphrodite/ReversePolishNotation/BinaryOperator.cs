using System;

namespace Aphrodite.ReversePolishNotation
{
    /// <summary>
    ///     二項演算子
    /// </summary>
    public class BinaryOperator : IToken, IOperator
    {
        public enum Operations
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Modulo,
            Power
        }

        public BinaryOperator(string token)
        {
            Token = token;
            switch (token)
            {
                case "+":
                    Operation = Operations.Addition;
                    Priority = 0;
                    break;
                case "-":
                    Operation = Operations.Subtraction;
                    Priority = 0;
                    break;
                case "*":
                    Operation = Operations.Multiplication;
                    Priority = 1;
                    break;
                case "/":
                    Operation = Operations.Division;
                    Priority = 1;
                    break;
                case "%":
                    Operation = Operations.Modulo;
                    Priority = 1;
                    break;
                case "^":
                    Operation = Operations.Power;
                    Priority = 2;
                    break;
                default:
                    throw new ArgumentException("Unknown binary operator: " + token);
            }
        }

        public Operations Operation { get; private set; }

        public int Priority { get; private set; }

        public TokenTypes Type
        {
            get { return TokenTypes.BinaryOperator; }
        }

        public string Token { get; private set; }
    }
}