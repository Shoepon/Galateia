using System;

namespace Galateia.Baloon
{
    /// <summary>
    ///     BaloonWindowに追加したアンカーがクリックされた際のイベント データを格納するクラスです．
    /// </summary>
    public class AnchorClickEventArgs : EventArgs
    {
        private readonly object _data;

        /// <summary>
        ///     イベントに関連付けるオブジェクトを指定して，新しいインスタンスを初期化します．
        /// </summary>
        /// <param name="data">イベントに関連付けるデータ．</param>
        public AnchorClickEventArgs(object data)
        {
            _data = data;
        }

        /// <summary>
        ///     ユーザー定義のデータを取得します．
        /// </summary>
        public object Data
        {
            get { return _data; }
        }
    }
}