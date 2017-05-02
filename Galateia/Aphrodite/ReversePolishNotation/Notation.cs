using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aphrodite.ReversePolishNotation
{
    public class Notation
    {
        private readonly List<IToken> _tokens;

        private Notation()
        {
            _tokens = new List<IToken>();
        }

        public IEnumerable<IToken> Tokens()
        {
            return _tokens;
        }

        private void AddToken(IToken token)
        {
            _tokens.Add(token);
        }

        /// <summary>
        ///     変数の値辞書を使って，数式内の変数に値を代入した新しい表現を取得します．
        /// </summary>
        public Notation Substitute(Dictionary<string, double> variables)
        {
            var notation = new Notation();
            foreach (IToken t in _tokens)
            {
                if (t.Type == TokenTypes.Variable && variables.ContainsKey(t.Token))
                    notation.AddToken(new Constant(variables[t.Token]));
                else
                    notation.AddToken(t);
            }
            return notation;
        }

        /// <summary>
        ///     逆ポーランド記法の文字列に変換します．
        /// </summary>
        /// <returns>逆ポーランド記法による数式表現．</returns>
        public override string ToString()
        {
            return string.Join(" ", from _ in _tokens select _.Token);
        }

        /// <summary>
        ///     中置記法による数式表現を逆ポーランド記法へ変換します．
        /// </summary>
        /// <param name="expression">中置記法で表現された数式．</param>
        /// <returns>逆ポーランド表現のインスタンス．</returns>
        public static Notation Parse(string expression)
        {
            var notation = new Notation();
            var stack = new Stack<IToken>();
            foreach (IToken token in EnumTokens(expression))
            {
                switch (token.Type)
                {
                    case TokenTypes.Constant:
                    case TokenTypes.Variable:
                        notation.AddToken(token);
                        break;
                    case TokenTypes.UnaryOperator:
                    case TokenTypes.BinaryOperator:
                        if (stack.Count > 0)
                        {
                            IToken peeked = stack.Peek();
                            while (peeked is IOperator)
                            {
                                if ((peeked as IOperator).Priority < ((IOperator) token).Priority)
                                    break;
                                notation.AddToken(stack.Pop());
                                peeked = (stack.Count > 0) ? stack.Peek() : null;
                            }
                        }
                        stack.Push(token);
                        break;
                    case TokenTypes.Parenthesis:
                        var p = (Parenthesis) token;
                        switch (p.Direction)
                        {
                            case Parenthesis.Directions.Left:
                                stack.Push(token);
                                break;
                            case Parenthesis.Directions.Right:
                                IToken t = stack.Pop();
                                while (t.Type != TokenTypes.Parenthesis)
                                {
                                    notation.AddToken(t);
                                    if (stack.Count == 0)
                                        throw new ArgumentException("Unclosed parenthesis");
                                    t = stack.Pop();
                                }
                                if (((Parenthesis) t).Direction != Parenthesis.Directions.Left)
                                    throw new ArgumentException("Unclosed parenthesis");
                                if (stack.Count > 0 && stack.Peek().Type == TokenTypes.Function)
                                    notation.AddToken(stack.Pop());
                                break;
                        }
                        break;
                    case TokenTypes.Comma:
                        while (stack.Count > 0 && stack.Peek().Type != TokenTypes.Parenthesis)
                        {
                            IToken t = stack.Pop();
                            notation.AddToken(t);
                            if (stack.Count == 0)
                                throw new ArgumentException("Unclosed parenthesis");
                        }
                        break;
                    case TokenTypes.Function:
                        stack.Push(token);
                        break;
                }
            }
            while (stack.Count > 0)
            {
                IToken t = stack.Pop();
                if (t.Type == TokenTypes.Parenthesis)
                    throw new ArgumentException("Unclosed parenthesis");
                notation.AddToken(t);
            }
            return notation;
        }

        private static IEnumerable<IToken> EnumTokens(string expression)
        {
            var regex = new Regex(@"^\s*((?<token>[\d\.]+((E|e)(\+|-)?\d+)?|\+|-|\*|/|%|\^|\(|\)|,|\w[\w\d]*)\s*)+\s*$");
            Match m = regex.Match(expression);
            if (!m.Success)
                throw new ArgumentException("Invalid expression: " + expression);

            string[] tokens = (from Capture _ in m.Groups["token"].Captures select _.Value).ToArray();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                switch (token)
                {
                    case "*":
                    case "/":
                    case "%":
                    case "^":
                        yield return new BinaryOperator(token);
                        break;
                    case "+":
                    case "-":
                        yield return (i == 0 || tokens[i - 1] == "(" || tokens[i - 1] == ",")
                            ? (IToken) new UnaryOperator(token)
                            : new BinaryOperator(token);
                        break;
                    case "(":
                    case ")":
                        yield return new Parenthesis(token);
                        break;
                    case ",":
                        yield return new Comma(token);
                        break;
                    default:
                        if (char.IsDigit(token[0]))
                        {
                            // 数字の場合
                            yield return new Constant(token);
                        }
                        else
                        {
                            // 文字列の場合
                            if (i < tokens.Length - 1 && tokens[i + 1] == "(")
                            {
                                // 関数の場合：引数の数を調べる必要がある
                                int n = 0;
                                for (int k = i + 1; k < tokens.Length; k++)
                                {
                                    if (tokens[k] == ",")
                                        n++;
                                    else if (tokens[k] == ")")
                                    {
                                        n++;
                                        break;
                                    }
                                }
                                yield return new Function(token, n);
                            }
                            else
                            {
                                // 変数の場合
                                yield return new Variable(token);
                            }
                        }
                        break;
                }
            }
        }
    }
}