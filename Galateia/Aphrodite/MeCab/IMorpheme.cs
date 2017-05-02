namespace Aphrodite.MeCab
{
    public interface IMorpheme
    {
        /// <summary>
        /// 表層形
        /// </summary>
        string Surface { get; }

        /// <summary>
        /// 品詞,品詞細分類1,品詞細分類2,品詞細分類3
        /// </summary>
        string[] PartOfSpeech { get; }

        /// <summary>
        /// 活用形
        /// </summary>
        string TypeOfConjugation { get; }

        /// <summary>
        /// 活用型
        /// </summary>
        string Conjugation { get; }

        /// <summary>
        /// 原形
        /// </summary>
        string OriginalForm { get; }

        /// <summary>
        /// 読み
        /// </summary>
        string Reading { get; }

        /// <summary>
        /// 発音
        /// </summary>
        string Pronunciation { get; }
    }
}
