namespace Aphrodite.ReversePolishNotation
{
    public enum TokenTypes
    {
        /// <summary>
        ///     単項演算子
        /// </summary>
        UnaryOperator,

        /// <summary>
        ///     二項演算子
        /// </summary>
        BinaryOperator,

        /// <summary>
        ///     関数
        /// </summary>
        Function,

        /// <summary>
        ///     定数
        /// </summary>
        Constant,

        /// <summary>
        ///     変数
        /// </summary>
        Variable,

        /// <summary>
        ///     丸括弧
        /// </summary>
        Parenthesis,

        /// <summary>
        ///     コンマ：関数の引数区切り
        /// </summary>
        Comma
    }
}