using System;
using System.Drawing;
using Galateia.Infra.WindowsAPI;

namespace Galateia.Infra.Windows
{
    public unsafe class ShellHookInfoEventArgs : EventArgs
    {
        private readonly SHELLHOOKINFO* lpShellHookInfo;

        public ShellHookInfoEventArgs(IntPtr lpShellHookInfo)
        {
            this.lpShellHookInfo = (SHELLHOOKINFO*) lpShellHookInfo;
        }

        /// <summary>
        ///     ウィンドウハンドル
        /// </summary>
        public IntPtr Handle
        {
            get { return lpShellHookInfo->hwnd; }
            set { lpShellHookInfo->hwnd = value; }
        }

        /// <summary>
        ///     レクト
        /// </summary>
        public Rectangle Rect
        {
            get { return lpShellHookInfo->rc; }
            set { lpShellHookInfo->rc = new RECT(value); }
        }
    }
}