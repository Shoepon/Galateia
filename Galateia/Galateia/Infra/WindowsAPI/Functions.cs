using System;
using System.Runtime.InteropServices;

namespace Galateia.Infra.WindowsAPI
{
    public delegate IntPtr WindowProc(IntPtr hWnd, WindowMessages Msg, UIntPtr wParam, IntPtr lParam);

    public delegate IntPtr WindowsHookProc(int nCode, IntPtr wParam, IntPtr lParam);

    public static class User
    {
        private static readonly WindowProc defWindowProc = DefWindowProc;

        public static IntPtr DefWindowProcPointer
        {
            get { return Marshal.GetFunctionPointerForDelegate(defWindowProc); }
        }

        [DllImport("user32.dll", EntryPoint = "AttachThreadInput", SetLastError = true)]
        private static extern bool _AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

        public static bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach)
        {
            return _AttachThreadInput(idAttach, idAttachTo, fAttach);
        }

        [DllImport("user32.dll", EntryPoint = "BringWindowToTop", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "CallNextHookEx", SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "ClientToScreen", SetLastError = true)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [DllImport("user32.dll", EntryPoint = "CloseWindow", SetLastError = true)]
        public static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, IntPtr classAtom,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth,
            int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu,
            IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", EntryPoint = "DefWindowProc", SetLastError = true)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, WindowMessages Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "DeregisterShellHookWindow", SetLastError = true)]
        public static extern bool DeregisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        public static extern WindowStylesEx GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int dwProcessId);

        [DllImport("user32.dll", EntryPoint = "IsWindow", SetLastError = true)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "IsWindowVisible", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "LoadCursorW", SetLastError = true)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

        [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true)]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX wcx);

        [DllImport("user32.dll", EntryPoint = "RegisterShellHookWindow", SetLastError = true)]
        public static extern bool RegisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW", SetLastError = true)]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport("user32.dll", EntryPoint = "ScreenToClient", SetLastError = true)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT point);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetParent", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        public static extern WindowStylesEx SetWindowLong(IntPtr hWnd, int nIndex, WindowStylesEx styles);

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(WindowsHook idHook, WindowsHookProc lpfn, IntPtr hMod,
            int dwThreadId);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
            SetWindowPosFlag uFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowTextW", SetLastError = true)]
        public static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindow nCmdShow);

        [DllImport("user32.dll", EntryPoint = "UnhookWindowsHookEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", EntryPoint = "UpdateLayeredWindow", SetLastError = true)]
        public static extern bool UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, ref POINT ptDst, ref SIZE size,
            IntPtr hdcSrc, ref POINT ptSrc, COLORREF crKey, ref BLENDFUNCTION pblend, UpdateLayeredWindowFlags flags);
    }

    [BestFitMapping(false, ThrowOnUnmappableChar = true)]
    public static class Kernel
    {
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = true)]
        public static extern int GetLastError();

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "GlobalAlloc", SetLastError = true)]
        public static extern IntPtr GlobalAlloc(GlobalMemoryFlags flags, IntPtr dwBytes);

        [DllImport("kernel32.dll", EntryPoint = "GlobalFree", SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
    }
}