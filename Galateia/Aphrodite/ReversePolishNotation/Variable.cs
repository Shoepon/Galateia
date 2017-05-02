namespace Aphrodite.ReversePolishNotation
{
    /// <summary>
    ///     変数
    /// </summary>
    public class Variable : IToken
    {
        public Variable(string token)
        {
            Token = token;
        }

        public TokenTypes Type
        {
            get { return TokenTypes.Variable; }
        }

        public string Token { get; private set; }
    }
}