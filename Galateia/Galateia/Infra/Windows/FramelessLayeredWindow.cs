using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Galateia.Infra.WindowsAPI;

namespace Galateia.Infra.Windows
{
    public class FramelessLayeredWindow : NativeWindow
    {
        /// <summary>
        ///     ウィンドウクラスの識別子
        /// </summary>
// ReSharper disable once InconsistentNaming
        private static readonly IntPtr windowClassAtom;

        /// <summary>
        ///     LayeredWindowの更新に利用するラッパクラス
        /// </summary>
        private LayeredWindowInfo _lwinfo;

        private IntPtr _parentWindowHandle = IntPtr.Zero;
        private Point _position = Point.Empty;
        private Size _size = Size.Empty;
        private byte _opacity = 255;
        private string _title = "";

        /// <summary>
        ///     ウィンドウクラスの登録
        /// </summary>
        static FramelessLayeredWindow()
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
                lpszClassName = "GalateiaFramelessLayeredWindow"
            };

            ushort atom = User.RegisterClassEx(ref wcx);
            windowClassAtom = new IntPtr(atom);
        }

        /// <summary>
        ///     ウィンドウクラスを取得します．
        ///     派生クラスで独自のウィンドウクラスを使用する場合はオーバーライドします
        /// </summary>
        protected virtual IntPtr WindowClassAtom
        {
            get { return windowClassAtom; }
        }

        /// <summary>
        ///     ウィンドウの位置を取得または設定します．
        /// </summary>
        public Point Position
        {
            get { return _position; }
            protected set { _position = value; }
        }

        /// <summary>
        ///     ウィンドウのサイズを取得または設定します．
        /// </summary>
        public Size Size
        {
            get { return _size; }
            protected set { _size = value; }
        }

        public byte Opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }

        /// <summary>
        ///     ウィンドウのタイトルを取得または設定します．
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                User.SetWindowText(Handle, _title);
            }
        }

        /// <summary>
        ///     親ウィンドウのハンドル
        /// </summary>
        public IntPtr ParentWindowHandle
        {
            get { return _parentWindowHandle; }
            set
            {
                _parentWindowHandle = value;
                User.SetParent(Handle, _parentWindowHandle);
            }
        }

        /// <summary>
        ///     関連付けられているハンドルが存在するウィンドウのものか否かを取得します．
        /// </summary>
        public bool IsWindow
        {
            get { return User.IsWindow(Handle); }
        }

        /// <summary>
        /// ウィンドウが表示されているか否かを取得します．
        /// </summary>
        public bool IsVisible
        {
            get { return User.IsWindowVisible(Handle); }
        }

        private const int GWL_EXSTYLE = -20;
        public WindowStylesEx WindowStylesEx
        {
            get { return User.GetWindowLong(Handle, GWL_EXSTYLE); }
            set { User.SetWindowLong(Handle, GWL_EXSTYLE, value); }
        }

        /// <summary>
        ///     ウィンドウを表示する際に，ウィンドウをアクティブにするかどうかの値を取得または設定します．
        /// </summary>
        public bool ShowActivated { get; set; }

        /// <summary>
        ///     レイヤードウィンドウの位置，サイズ，内容を更新します
        /// </summary>
        /// <param name="hdcSource">レイヤードウィンドウの内容を定義するサーフェスのDC</param>
        protected void UpdateLayeredWindow(IntPtr hdcSource)
        {
            _lwinfo.Position = new POINT(Position);
            _lwinfo.Size = new SIZE(Size);
            _lwinfo.Opacity = Opacity;
            _lwinfo.Update(hdcSource);
        }

        /// <summary>
        ///     ウィンドウを作成します．
        /// </summary>
        /// <param name="extraExStyles">WindowsStyleEx.Layered の他に指定するスタイル．</param>
        public void Create(WindowStylesEx extraExStyles)
        {
            // ウィンドウの作成
            IntPtr hWnd = User.CreateWindowEx(
                WindowStylesEx.Layered | extraExStyles,
                WindowClassAtom, Title,
                WindowStyles.Popup,
                Position.X, Position.Y,
                Size.Width, Size.Height, ParentWindowHandle, IntPtr.Zero, Kernel.GetModuleHandle(null), IntPtr.Zero);

            if (hWnd == IntPtr.Zero)
                throw new Win32Exception(Kernel.GetLastError(), "Failed to create FramelessLayeredWindow.");

            // LayeredWindowInfoを初期化
            _lwinfo = new LayeredWindowInfo(hWnd, Position.X, Position.Y, Size.Width, Size.Height);

            // ウィンドウハンドルを関連付け
            AssignHandle(hWnd);

            OnCreate();
        }

        /// <summary>
        ///     ウィンドウを表示します．
        /// </summary>
        public void Show()
        {
            User.ShowWindow(Handle, ShowActivated ? ShowWindow.Show : ShowWindow.ShowNoActivate);
        }

        /// <summary>
        ///     ウィンドウを非表示にします．
        /// </summary>
        public void Hide()
        {
            User.ShowWindow(Handle, ShowWindow.Hide);
        }

        /// <summary>
        ///     ウィンドウが作成された直後に呼び出される通知メソッドを定義します．
        /// </summary>
        protected virtual void OnCreate()
        {
        }

        /// <summary>
        ///     ウィンドウを破棄します
        /// </summary>
        public void Destroy()
        {
            DestroyHandle();
        }
    }
}