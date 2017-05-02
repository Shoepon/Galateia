using System;

namespace Galateia.Infra.Windows
{
    public class ShellHookEventArgs : EventArgs
    {
        private readonly IntPtr hWnd;

        public ShellHookEventArgs(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        /// <summary>
        ///     ウィンドウハンドル
        /// </summary>
        public IntPtr Handle
        {
            get { return hWnd; }
        }
    }
}