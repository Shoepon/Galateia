namespace Galateia.Ghost.Script
{
    /// <summary>
    ///     トークンのスパンに関する動作です．
    /// </summary>
    public enum SpanModes
    {
        /// <summary>
        ///     スパンに関する動作はありません．
        /// </summary>
        None,

        /// <summary>
        ///     スパンに入ります．
        /// </summary>
        Enter,

        /// <summary>
        ///     直近の対応するスパンから出ます．
        /// </summary>
        Exit
    }
}