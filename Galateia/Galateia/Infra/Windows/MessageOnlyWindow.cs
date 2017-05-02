using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Galateia.Infra.WindowsAPI;

namespace Galateia.Infra.Windows
{
    /// <summary>
    ///     メッセージの処理のみを行うウィンドウです．
    /// </summary>
    public class MessageOnlyWindow : NativeWindow
    {
        /// <summary>
        ///     ウィンドウクラスの識別子
        /// </summary>
        private static readonly IntPtr windowClassAtom;

        private bool disposed = false;

        /// <summary>
        ///     ウィンドウクラスの登録
        /// </summary>
        static MessageOnlyWindow()
        {
            var wcx = new WNDCLASSEX
            {
                cbSize = (uint) Marshal.SizeOf(typeof (WNDCLASSEX)),
                style = 0,
                lpfnWndProc = User.DefWindowProcPointer,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = Kernel.GetModuleHandle(null),
                hIcon = IntPtr.Zero,
                hIconSm = IntPtr.Zero,
                hCursor = User.LoadCursor(IntPtr.Zero, IDC.ARROW),
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = "GalateiaMessageOnlyWindow"
            };

            ushort atom = User.RegisterClassEx(ref wcx);
            windowClassAtom = new IntPtr(atom);
        }

        /// <summary>
        ///     ウィンドウを作成します
        /// </summary>
        public void CreateHandle(string title = "")
        {
            // ウィンドウの作成
            IntPtr hWnd = User.CreateWindowEx(
                WindowStylesEx.None,
                windowClassAtom, title,
                WindowStyles.Overlapped,
                0, 0, 0, 0,
                new IntPtr(-3), // HWND_MESSAGE
                IntPtr.Zero, Kernel.GetModuleHandle(null), IntPtr.Zero);

            if (hWnd == IntPtr.Zero)
                throw new Win32Exception(Kernel.GetLastError(), "Failed to create MessageOnlyWindow.");

            // ウィンドウハンドルを関連付け
            AssignHandle(hWnd);
        }
    }
}