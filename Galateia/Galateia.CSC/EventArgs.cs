using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galateia.CSC
{
    /// <summary>
    /// 特定の型のデータを持つイベント引数．
    /// </summary>
    /// <typeparam name="T">データの型を指定します．</typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// データを指定して新しいインスタンスを初期化します．
        /// </summary>
        /// <param name="value">データを指定します．</param>
        public EventArgs(T value)
        {
            Value = value;
        }

        /// <summary>
        ///    データを取得します．
        /// </summary>
        public T Value { get; private set; }
    }
}