using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Galateia.Infra.Graphics
{
    public class Renderer : IDisposable
    {
        private bool _disposed;
        private bool _run = true;
        private IRenderable[] _renderables = new IRenderable[0];
        private readonly ManualResetEventSlim _continue = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _waitContinue = new ManualResetEventSlim(false);
        private readonly Thread _workerThread;

        public FPSCounterWithLimiter FpsCounter { get; private set; }

        public Renderer()
        {
            FpsCounter = new FPSCounterWithLimiter();
            _workerThread = new Thread(WorkerThread);
            _workerThread.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Renderer()
        {
            Dispose(false);
        }

        public void Add(IRenderable renderable)
        {
            bool prevContinue = Continue;
            Continue = false;

            var list = new List<IRenderable>(_renderables);
            list.Add(renderable);
            _renderables = list.ToArray();

            Continue = prevContinue;
        }

        public void Remove(IRenderable renderable)
        {
            bool prevContinue = Continue;
            Continue = false;
            
            var list = new List<IRenderable>(_renderables);
            list.Remove(renderable);
            _renderables = list.ToArray();

            Continue = prevContinue;
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
                _continue.Set();
                _workerThread.Join();

                _continue.Dispose();
                _renderables = null;

                _disposed = true;
            }
        }

        /// <summary>
        /// レンダリングを実行するか否かを表す値を取得または設定します．
        /// </summary>
        public bool Continue
        {
            get
            {
                return !_waitContinue.IsSet || _workerThread.ThreadState != ThreadState.WaitSleepJoin;
            }
            set
            {
                if (value)
                    _continue.Set();
                else
                {
                    _continue.Reset();
                    SpinWait.SpinUntil(() => !Continue);
                }
            }
        }

        private void WorkerThread()
        {
            _waitContinue.Reset();
            FpsCounter.Start();
            _waitContinue.Set();
            _continue.Wait();
            _waitContinue.Reset();

            do
            {
                if (FpsCounter.ShouldRender)
                {
                    FpsCounter.CountFrame();
                    foreach (var r in _renderables)
                        r.Render();
                }
                else
                {
                    Thread.Sleep(1);
                }

                _waitContinue.Set();
                _continue.Wait();
                _waitContinue.Reset();
            } while (_run);
        }

    }
}
