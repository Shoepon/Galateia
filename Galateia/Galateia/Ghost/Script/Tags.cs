namespace Galateia.Ghost.Script
{
    /// <summary>
    ///     タグ．
    /// </summary>
    public enum Tags
    {
        /// <summary>
        ///     ただの文字です．
        /// </summary>
        None,

        /// <summary>
        ///     未知のトークンです．
        /// </summary>
        Unknown,

        /// <summary>
        ///     \a[]...\_a: アンカーです．
        /// </summary>
        Anchor,

        /// <summary>
        ///     \c: 表示内容をクリアします．
        /// </summary>
        Clear,

        /// <summary>
        ///     \e: スクリプトの終端であることを示します．
        /// </summary>
        EndOfScript,

        /// <summary>
        ///     \i[]: 画像を表示します．
        /// </summary>
        Image,

        /// <summary>
        ///     \n: 改行します．
        /// </summary>
        NewLine,

        /// <summary>
        ///     \w[]: 指定した時間（ms）待機します．
        /// </summary>
        Wait,

        /// <summary>
        ///     \![]: コマンドを実行します．
        /// </summary>
        Command,

        /// <summary>
        ///     \?[]: 振る舞いに関するコマンドを実行します．
        /// </summary>
        Demeanor
    }
}