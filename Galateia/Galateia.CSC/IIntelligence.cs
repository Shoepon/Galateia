using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galateia.CSC
{
    /// <summary>
    /// 知能を司る部分のインタフェースです．
    /// </summary>
    public interface IIntelligence
    {
        /// <summary>
        /// システムのロードが完了したときに呼び出されます．
        /// </summary>
        void Loaded();

        /// <summary>
        /// システムが終了しようとする際に呼び出されます．
        /// </summary>
        void Disposing();

        /// <summary>
        /// ユーザーからの文字列入力を受け付けます．
        /// </summary>
        /// <param name="text">入力された文字列を指定します．</param>
        void TextInput(string text);

        // マウス・タッチなどによるおさわり入力も受け付けたい．
        // void TouchInput(?);

        /// <summary>
        /// 発言・挙動を表すためのスクリプトを出力します．
        /// </summary>
        event EventHandler<EventArgs<string>> Output;
    }
}
