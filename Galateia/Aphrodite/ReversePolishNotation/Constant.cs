using System.Globalization;

namespace Aphrodite.ReversePolishNotation
{
    /// <summary>
    ///     定数
    /// </summary>
    public class Constant : IToken
    {
        public Constant(string token)
        {
            Token = token;
            Value = double.Parse(token);
        }

        public Constant(double value)
        {
            Token = value.ToString(CultureInfo.InvariantCulture);
            Value = value;
        }

        public double Value { get; private set; }

        public TokenTypes Type
        {
            get { return TokenTypes.Constant; }
        }

        public string Token { get; private set; }
    }
}