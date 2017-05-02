using System;
using System.Collections.Generic;
using System.Threading;
using Galateia.CSC;
using Galateia.Infra.Windows;

namespace Galateia.Infra
{
    /// <summary>
    ///     要素が自動的に逐次送出される，オブジェクトの先入れ先出しコレクションを表します．
    /// </summary>
    /// <typeparam name="T">キュー内の要素の型を指定します．</typeparam>
    public class SerialDispatchQueue<T> : IDisposable
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly ManualResetEventSlim _queued = new ManualResetEventSlim();
        private readonly Thread _workerThread;
        private bool _disposed;
        private bool _run = true;

        /// <summary>
        ///     新しい空のコレクションを初期化します．
        /// </summary>
        public SerialDispatchQueue()
        {
            _workerThread = new Thread(WorkerThread);
            _workerThread.Start();
        }

        /// <summary>
        ///     新しい空のコレクションを初期化します．
        /// </summary>
        /// <param name="state">コレクションの要素を送出するイベントのハンドラが実行されるスライドのアパートメント状態を指定します．</param>
        public SerialDispatchQueue(ApartmentState state)
        {
            _workerThread = new Thread(WorkerThread);
            _workerThread.SetApartmentState(state);
            _workerThread.Start();
        }

        /// <summary>
        ///     使用されているすべてのリソースを解放します．
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SerialDispatchQueue()
        {
            Dispose(false);
        }

        /// <summary>
        ///     使用されているすべてのリソースを解放します．
        /// </summary>
        /// <param name="disposing">マネージリソースを解放する必要があるかどうか指定します．</param>
        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _run = false;
                lock (_queue)
                    _queued.Set();
                
                SpinWaitWithMessagePump.SpinUntil(() => !_workerThread.IsAlive);
                _workerThread.Join();

                _queued.Dispose();

                _disposed = true;
            }
        }

        private void WorkerThread()
        {
            do
            {
                // キューから値を取り出す
                bool dequeued = false;
                T value = default(T);
                lock (_queue)
                {
                    if (_queue.Count > 0)
                    {
                        value = _queue.Dequeue();
                        dequeued = true;
                    }
                    if (_queue.Count == 0)
                        _queued.Reset();
                }

                // 値がとりだされた場合には処理
                if (dequeued)
                {
                    EventHandler<EventArgs<T>> handler = Dispatched;
                    if (handler != null)
                        handler(this, new EventArgs<T>(value));
                }

                // キューに値が追加されるのを待つ
                _queued.Wait();
            } while (_run);
        }

        /// <summary>
        ///     キューの最後尾に要素を追加します．
        /// </summary>
        /// <param name="obj">追加する要素を指定します．</param>
        public void Enqueue(T obj)
        {
            lock (_queue)
            {
                _queue.Enqueue(obj);
                _queued.Set();
            }
        }

        /// <summary>
        /// キューを空にします．
        /// </summary>
        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        /// <summary>
        ///     キューの先頭にある要素が渡されます．すべてのハンドラが制御を返すと，次の要素が処理されます．
        /// </summary>
        public event EventHandler<EventArgs<T>> Dispatched;
    }
}