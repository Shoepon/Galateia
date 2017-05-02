using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphrodite.MeCab
{
    /// <summary>
    /// 形態素
    /// </summary>
    public class Morpheme : IMorpheme
    {
        /// <summary>
        /// 表層形
        /// </summary>
        public string Surface { get; private set; }

        /// <summary>
        /// 品詞,品詞細分類1,品詞細分類2,品詞細分類3
        /// </summary>
        public string[] PartOfSpeech { get; private set; }

        /// <summary>
        /// 活用形
        /// </summary>
        public string TypeOfConjugation { get; private set; }

        /// <summary>
        /// 活用型
        /// </summary>
        public string Conjugation { get; private set; }

        /// <summary>
        /// 原形
        /// </summary>
        public string OriginalForm { get; private set; }

        /// <summary>
        /// 読み
        /// </summary>
        public string Reading { get; private set; }

        /// <summary>
        /// 発音
        /// </summary>
        public string Pronunciation { get; private set; }

        public Morpheme(MeCabNode node)
        {
            Surface = node.Surface;
            var features = node.Feature.Split(new[] {','}, StringSplitOptions.None);
            // 品詞,品詞細分類1,品詞細分類2,品詞細分類3,活用形,活用型,原形,読み,発音
            PartOfSpeech = new[] { features[0], features[1], features[2], features[3] };
            TypeOfConjugation = features[4];
            Conjugation = features[5];
            OriginalForm = features[6];
            Reading = features[7];
            Pronunciation = features[8];
        }

    }
}
