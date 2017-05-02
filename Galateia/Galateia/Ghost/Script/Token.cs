using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Galateia.Ghost.Script
{
    /// <summary>
    ///     トークンを表します．
    /// </summary>
    public class Token
    {
        private static readonly Regex RegexTokens = new Regex(@"(?<token>\\_?[a-zA-Z\\!?](\[(\\\]|[^\]])*\])?|.)*");

        private static readonly Regex RegexCommand =
            new Regex(@"\\(?<cmd>_?[a-zA-Z\\!?])(\[(?<opt>(\\\]|[^\]])*)\])?");

        private Token(Tags tag, SpanModes span, string surface)
        {
            Tag = tag;
            Span = span;
            Surface = surface;
            Option = string.Empty;
        }

        private Token(Tags tag, SpanModes span, string surface, string option)
        {
            Tag = tag;
            Span = span;
            Surface = surface;
            Option = option;
        }

        /// <summary>
        ///     タグの種類を取得します．
        /// </summary>
        public Tags Tag { get; private set; }

        /// <summary>
        ///     スパンのモードを取得します．
        /// </summary>
        public SpanModes Span { get; private set; }

        /// <summary>
        ///     トークンの文字列を取得します．
        /// </summary>
        public string Surface { get; private set; }

        /// <summary>
        ///     設定されているオプションを取得します．
        /// </summary>
        public string Option { get; private set; }

        /// <summary>
        ///     スクリプト文字列を解析して，トークンに分解します．
        /// </summary>
        /// <param name="script">解析するスクリプト．</param>
        /// <returns>分解されたトークンのリスト．</returns>
        public static IEnumerable<Token> Parse(string script)
        {
            var m = RegexTokens.Match(script);
            // 失敗なら分解せずに一つのトークンとして処理
            if (!m.Success)
                yield return new Token(Tags.Unknown, SpanModes.None, script);

            // トークンに分解
            foreach (Capture token in m.Groups["token"].Captures)
            {
                if (token.Value.StartsWith("\\"))
                {
                    m = RegexCommand.Match(token.Value);
                    if (!m.Success)
                        yield return new Token(Tags.Unknown, SpanModes.None, token.Value);

                    var option = m.Groups["opt"].Captures.Count > 0 ?  m.Groups["opt"].Captures[0].Value.Replace("\\]", "]") : string.Empty;
                    switch (m.Groups["cmd"].Captures[0].Value)
                    {
                        case "\\":
                            yield return new Token(Tags.None, SpanModes.None, "\\");
                            break;
                        case "a":
                            yield return new Token(Tags.Anchor, SpanModes.Enter, token.Value, option);
                            break;
                        case "_a":
                            yield return new Token(Tags.Anchor, SpanModes.Exit, token.Value, option);
                            break;
                        case "c":
                            yield return new Token(Tags.Clear, SpanModes.None, token.Value, option);
                            break;
                        case "e":
                            yield return new Token(Tags.EndOfScript, SpanModes.None, token.Value, option);
                            break;
                        case "i":
                            yield return new Token(Tags.Image, SpanModes.None, token.Value, option);
                            break;
                        case "n":
                            yield return new Token(Tags.NewLine, SpanModes.None, token.Value, option);
                            break;
                        case "w":
                            yield return new Token(Tags.Wait, SpanModes.None, token.Value, option);
                            break;
                        case "!":
                            yield return new Token(Tags.Command, SpanModes.None, token.Value, option);
                            break;
                        case "?":
                            yield return new Token(Tags.Demeanor, SpanModes.None, token.Value, option);
                            break;
                        default:
                            yield return new Token(Tags.Unknown, SpanModes.None, token.Value, option);
                            break;
                    }
                }
                else
                    yield return new Token(Tags.None, SpanModes.None, token.Value);
            }
        }
    }
}