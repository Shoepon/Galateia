using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galateia.Infra
{
    /// <summary>
    /// 最後に追加した要素を先頭とする環状バッファです．
    /// </summary>
    /// <typeparam name="T">要素の型を指定します．</typeparam>
    public class RingBuffer<T> : IList<T>
    {
        private readonly int _capacity;
        private readonly T[] _buffer;
        private int _currentZeroIndex;
        private readonly T _defaultValue;

        /// <summary>
        /// バッファの要素数を指定して新しいインスタンスを初期化します．
        /// </summary>
        /// <param name="capacity">要素の数を指定します．</param>
        public RingBuffer(int capacity)
        {
            _capacity = capacity;
            _buffer = new T[capacity];
            _currentZeroIndex = 0;
            _defaultValue = default(T);
        }

        /// <summary>
        /// バッファの要素数と，既定の値を指定して新しいインスタンスを初期化します．
        /// </summary>
        /// <param name="capacity">要素の数を指定します．</param>
        /// <param name="defaultValue">既定の値を指定します．</param>
        public RingBuffer(int capacity, T defaultValue)
            : this(capacity)
        {
            _defaultValue = defaultValue;
            for (int i = 0; i < _capacity; i++)
                _buffer[i] = _defaultValue;
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.IList`1"/> 内での指定した項目のインデックスを調べます。
        /// </summary>
        /// <returns>
        /// リストに存在する場合は <paramref name="item"/> のインデックス。それ以外の場合は -1。
        /// </returns>
        /// <param name="item"><see cref="T:System.Collections.Generic.IList`1"/> 内で検索するオブジェクト。</param>
        public int IndexOf(T item)
        {
            for (int i = _currentZeroIndex; i < _capacity; i++)
                if (item.Equals(_buffer[i]))
                    return i - _currentZeroIndex;
            for (int i = 0; i < _currentZeroIndex; i++)
                if (item.Equals(_buffer[i]))
                    return _capacity + i - _currentZeroIndex;
            return -1;
        }

        /// <summary>
        /// 指定したインデックスの <see cref="T:System.Collections.Generic.IList`1"/> に項目を挿入します。
        /// </summary>
        /// <param name="index"><paramref name="item"/> を挿入する位置の 0 から始まるインデックス。</param>
        /// <param name="item"><see cref="T:System.Collections.Generic.IList`1"/> に挿入するオブジェクト。</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> が <see cref="T:System.Collections.Generic.IList`1"/> の有効なインデックスではありません。</exception>
        public void Insert(int index, T item)
        {
            if (index >= _capacity || 0 > index)
                throw new ArgumentOutOfRangeException("index");

            // 挿入位置より後ろのデータをずらす
            for (int i = _capacity - 1; i > index; i--)
            {
                int dst = i + _currentZeroIndex;
                int src = dst - 1;

                _buffer[dst < _capacity ? dst : dst - _capacity] = _buffer[src < _capacity ? src : src - _capacity];
            }

            // データ挿入
            int target = index + _currentZeroIndex;
            _buffer[target < _capacity ? target : target - _capacity] = item;
        }

        /// <summary>
        /// 指定したインデックス位置にある <see cref="T:System.Collections.Generic.IList`1"/> 項目を削除します。
        /// </summary>
        /// <param name="index">削除する項目の 0 から始まるインデックス。</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> が <see cref="T:System.Collections.Generic.IList`1"/> の有効なインデックスではありません。</exception>
        public void RemoveAt(int index)
        {
            // 挿入位置より後ろのデータをずらす
            for (int i = _capacity - 1; i > index; i--)
            {
                int src = i + _currentZeroIndex;
                int dst = src - 1;

                _buffer[dst < _capacity ? dst : dst - _capacity] = _buffer[src < _capacity ? src : src - _capacity];
            }

            // 最後に既定のデータを挿入
            int last = _currentZeroIndex - 1;
            _buffer[last < 0? last + _capacity: last] = _defaultValue;
        }

        /// <summary>
        /// 指定したインデックスにある要素を取得または設定します。
        /// </summary>
        /// <returns>
        /// 指定したインデックスにある要素。
        /// </returns>
        /// <param name="index">取得または設定する要素の、0 から始まるインデックス番号。</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> が <see cref="T:System.Collections.Generic.IList`1"/> の有効なインデックスではありません。</exception>
        public T this[int index]
        {
            get
            {
                if (index >= _capacity || 0 > index)
                    throw new ArgumentOutOfRangeException("index");

                int i = _currentZeroIndex + index;
                return _buffer[i < _capacity ? i : i - _capacity];
            }
            set
            {
                if (index >= _capacity || 0 > index)
                    throw new ArgumentOutOfRangeException("index");

                int i = _currentZeroIndex + index;
                _buffer[i < _capacity ? i : i - _capacity] = value;
            }
        }

        /// <summary>
        /// リストの先頭に項目を追加します。
        /// </summary>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1"/> に追加するオブジェクト。</param>
        public void Add(T item)
        {
            if (--_currentZeroIndex < 0)
                _currentZeroIndex += _capacity;

            _buffer[_currentZeroIndex] = item;
        }

        /// <summary>
        /// すべての項目を既定の値に初期化します。
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _capacity; i++)
                _buffer[i] = _defaultValue;
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> に特定の値が格納されているかどうかを判断します。
        /// </summary>
        /// <returns>
        /// <paramref name="item"/> が <see cref="T:System.Collections.Generic.ICollection`1"/> に存在する場合は true。それ以外の場合は false。
        /// </returns>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1"/> 内で検索するオブジェクト。</param>
        public bool Contains(T item)
        {
            for (int i = 0; i < _capacity; i++)
                if (item.Equals(_buffer[i]))
                    return true;

            return false;
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> の要素を <see cref="T:System.Array"/> にコピーします。<see cref="T:System.Array"/> の特定のインデックスからコピーが開始されます。
        /// </summary>
        /// <param name="array"><see cref="T:System.Collections.Generic.ICollection`1"/> から要素がコピーされる 1 次元の <see cref="T:System.Array"/>。 <see cref="T:System.Array"/> には、0 から始まるインデックス番号が必要です。</param>
        /// <param name="arrayIndex">コピーの開始位置となる、<paramref name="array"/> 内の 0 から始まるインデックス。</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> は null なので、</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> が 0 未満です。</exception>
        /// <exception cref="T:System.ArgumentException">コピー元の <see cref="T:System.Collections.Generic.ICollection`1"/> の要素数が、<paramref name="arrayIndex"/> からコピー先の <paramref name="array"/> の末尾までに格納できる数を超えています。</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Length - arrayIndex < _capacity)
                throw new ArgumentException("Insufficient destination array size.");

            for (int i = _currentZeroIndex; i < _capacity; i++)
                array[arrayIndex++] = _buffer[i];
            for (int i = 0; i < _currentZeroIndex; i++)
                array[arrayIndex++] = _buffer[i];
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> に格納されている要素の数を取得します。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> に格納されている要素の数。
        /// </returns>
        public int Count
        {
            get { return _capacity; }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> が読み取り専用かどうかを示す値を取得します。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> が読み取り専用である場合は true。それ以外の場合は false。
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> 内で最初に見つかった特定のオブジェクトを削除します。
        /// </summary>
        /// <returns>
        /// <paramref name="item"/> が <see cref="T:System.Collections.Generic.ICollection`1"/> から正常に削除された場合は true。それ以外の場合は false。 このメソッドは、<paramref name="item"/> が元の <see cref="T:System.Collections.Generic.ICollection`1"/> に見つからない場合にも false を返します。
        /// </returns>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1"/> から削除するオブジェクト。</param>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// コレクションを反復処理する列挙子を返します。
        /// </summary>
        /// <returns>
        /// コレクションを反復処理するために使用できる <see cref="T:System.Collections.Generic.IEnumerator`1"/>。
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _currentZeroIndex; i < _capacity; i++)
                yield return _buffer[i];
            for (int i = 0; i < _currentZeroIndex; i++)
                yield return _buffer[i];
        }

        /// <summary>
        /// コレクションを反復処理する列挙子を返します。
        /// </summary>
        /// <returns>
        /// コレクションを反復処理するために使用できる <see cref="T:System.Collections.IEnumerator"/> オブジェクト。
        /// </returns>
        /// <filterpriority>2</filterpriority>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
