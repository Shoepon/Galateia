namespace Aphrodite.ReversePolishNotation
{
    /// <summary>
    ///     関数
    /// </summary>
    public class Function : IToken
    {
        public Function(string token, int nArgs)
        {
            Token = token;
            NumberOfArguments = nArgs;
        }

        public int NumberOfArguments { get; private set; }

        public TokenTypes Type
        {
            get { return TokenTypes.Function; }
        }

        public string Token { get; private set; }
    }
}