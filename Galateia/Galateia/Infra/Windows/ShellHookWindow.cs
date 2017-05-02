using System;
using System.ComponentModel;
using System.Windows.Forms;
using Galateia.Infra.WindowsAPI;

namespace Galateia.Infra.Windows
{
    /// <summary>
    ///     Shellフックを提供します．
    ///     コンストラクタが実行されるスレッドでメッセージポンプが動作している必要があります．
    /// </summary>
    public class ShellHookWindow : MessageOnlyWindow, IDisposable
    {
        private readonly int WM_SHELLHOOKMESSAGE;
        private bool disposed;

        /// <summary>
        ///     Shellフックを開始します
        /// </summary>
        public ShellHookWindow()
        {
            // ShellHookメッセージのIDを取得
            WM_SHELLHOOKMESSAGE = unchecked((int) User.RegisterWindowMessage("SHELLHOOK"));
            if (WM_SHELLHOOKMESSAGE == 0)
                throw new Win32Exception(Kernel.GetLastError(), "Failed to obtain the value of WM_SHELLHOOKMESSAGE.");

            // ウィンドウの作成
            base.CreateHandle("Galateia Shell Hook");

            // シェルフックの開始
            User.RegisterShellHookWindow(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ShellHookWindow()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }

                // シェルフックの解除
                User.DeregisterShellHookWindow(Handle);
                // ウィンドウの終了
                DestroyHandle();

                disposed = true;
            }
        }

        /// <summary>
        ///     A window is being minimized or maximized. The system needs the coordinates of the minimized rectangle for the
        ///     window.
        /// </summary>
        public event EventHandler<ShellHookInfoEventArgs> GetMinRect;

        /// <summary>
        ///     The activation has changed to a different top-level, unowned window.
        /// </summary>
        public event EventHandler<ShellHookEventArgs> WindowActivated;

        public event EventHandler<ShellHookEventArgs> WindowReplacing;

        /// <summary>
        ///     A top-level window is being replaced. The window exists when the system calls this hook.
        /// </summary>
        public event EventHandler<ShellHookEventArgs> WindowReplaced;

        /// <summary>
        ///     A top-level, unowned window has been created. The window exists when the system calls this hook.
        /// </summary>
        public event EventHandler<ShellHookEventArgs> WindowCreated;

        /// <summary>
        ///     A top-level, unowned window is about to be destroyed. The window still exists when the system calls this hook.
        /// </summary>
        public event EventHandler<ShellHookEventArgs> WindowDestroyed;

        /// <summary>
        ///     The shell should activate its main window.
        /// </summary>
        public event EventHandler<EventArgs> ActivateShellWindow;

        /// <summary>
        ///     The user has selected the task list. A shell application that provides a task list should return TRUE to prevent
        ///     Windows from starting its task list.
        /// </summary>
        public event EventHandler<EventArgs> TaskMan;

        /// <summary>
        ///     The title of a window in the task bar has been redrawn.
        /// </summary>
        public event EventHandler<ShellHookEventArgs> Redraw;

        public event EventHandler<ShellHookEventArgs> EndTask;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SHELLHOOKMESSAGE)
            {
                if (m.WParam == HSHELL.GETMINRECT)
                {
                    if (GetMinRect != null)
                        GetMinRect(this, new ShellHookInfoEventArgs(m.LParam));
                }
                else if (m.WParam == HSHELL.WINDOWACTIVATED || m.WParam == HSHELL.RUDEAPPACTIVATED)
                {
                    if (WindowActivated != null)
                        WindowActivated(this, new ShellHookEventArgs(m.LParam));
                }
                else if (m.WParam == HSHELL.WINDOWREPLACING)
                {
                    if (WindowReplacing != null)
                        WindowReplacing(this, new ShellHookEventArgs(m.LParam));
                }
                else if (m.WParam == HSHELL.WINDOWREPLACED)
                {
                    if (WindowReplaced != null)
                        WindowReplaced(this, new ShellHookEventArgs(m.LParam));
                }
                else if (m.WParam == HSHELL.WINDOWCREATED)
                {
                    if (WindowCreated != null)
                        WindowCreated(this, new ShellHookEventArgs(m.LParam));
                }
                else if (m.WParam == HSHELL.WINDOWDESTROYED)
                {
                    if (WindowDestroyed != null)
                        WindowDestroyed(this, new ShellHookEventArgs(m.LParam));
                }
                else if (m.WParam == HSHELL.ACTIVATESHELLWINDOW)
                {
                    if (ActivateShellWindow != null)
                        ActivateShellWindow(this, new EventArgs());
                }
                else if (m.WParam == HSHELL.TASKMAN)
                {
                    if (TaskMan != null)
                        TaskMan(this, new EventArgs());
                }
                else if (m.WParam == HSHELL.REDRAW || m.WParam == HSHELL.FLASH)
                {
                    if (Redraw != null)
                        Redraw(this, new ShellHookEventArgs(m.LParam));
                }
                else if (m.WParam == HSHELL.ENDTASK)
                {
                    if (EndTask != null)
                        EndTask(this, new ShellHookEventArgs(m.LParam));
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}