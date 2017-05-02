namespace Aphrodite.ReversePolishNotation
{
    public interface IToken
    {
        TokenTypes Type { get; }
        string Token { get; }
    }
}