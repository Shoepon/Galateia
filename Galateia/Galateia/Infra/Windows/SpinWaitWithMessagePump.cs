using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Galateia.Infra.Windows
{
    public static class SpinWaitWithMessagePump
    {
        /// <summary>
        ///     メッセージポンプ付のスピンウェイトを行います．
        /// </summary>
        /// <param name="condition">呼び出し元とは異なるスレッドで実行される，終了条件判定．</param>
        public static void SpinUntil(Func<bool> condition)
        {
            var frame = new DispatcherFrame();
            Task.Run(() =>
            {
                SpinWait.SpinUntil(condition);
                frame.Continue = false;
            });
            Dispatcher.PushFrame(frame);
        }
    }
}