using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Galateia.CSC;
using Galateia.Infra;
using Galateia.Infra.Windows;
using Galateia.Infra.WindowsAPI;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Galateia.UserInput
{
    /// <summary>
    ///     UserInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UserInputWindow : Window, IDisposable
    {
        private const int LogCount = 100;

        // 最新 0 ---- last 最古
        private readonly List<string> _backlog = new List<string>();
        private readonly UserInputConfig _config;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly WindowsHookProc _hookProc;
        private readonly string _logFile;
        private readonly ShellHookWindow _shellHook;
        private int _backlogPos = -1;
        private string _backlogTemp;
        private bool _disposed;
        private IntPtr _hHook;

        /// <summary>
        ///     入力ログファイルを指定して，インスタンスを初期化します．
        /// </summary>
        /// <param name="inputLogFile">入力のログファイル</param>
        /// <param name="shellHook">シェルフックウィンドウ</param>
        /// <param name="config">設定</param>
        public UserInputWindow(string inputLogFile, ShellHookWindow shellHook, UserInputConfig config)
        {
            _logFile = inputLogFile;
            _shellHook = shellHook;
            _config = config;
            shellHook.WindowActivated += shellHook_WindowActivated;

            InitializeComponent();

            // フックの開始
            _hookProc = HookCallback; // アンマネージに渡すデリゲートのライフタイムは自分で管理する！
            _hHook = User.SetWindowsHookEx(WindowsHook.KeyboardLL, _hookProc, IntPtr.Zero, 0);
        }

        public IntPtr Handle
        {
            get
            {
                var helper = new WindowInteropHelper(this);
                return helper.Handle;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler UserInputStart;
        public event EventHandler UserInputCancel;
        public event EventHandler<EventArgs<string>> UserInput;

        // ウィンドウハンドルを取得

        ~UserInputWindow()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _shellHook.WindowActivated -= shellHook_WindowActivated;

                if (_hHook != IntPtr.Zero)
                {
                    User.UnhookWindowsHookEx(_hHook);
                    _hHook = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        private void shellHook_WindowActivated(object sender, ShellHookEventArgs e)
        {
            User.SetWindowPos(Handle, new IntPtr(-1) /* HWND_TOPMOST */, 0, 0, 0, 0,
                SetWindowPosFlag.NoActivate | SetWindowPosFlag.NoMove | SetWindowPosFlag.NoSize);
        }

        /// <summary>
        ///     フック プロシージャ
        /// </summary>
        private unsafe IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((nCode >= 0) && (wParam == (IntPtr) WindowMessages.KeyDown))
            {
                var pkbd = (KBDLLHOOKSTRUCT*) lParam;

                if (pkbd->vkCode == (uint) _config.InputVirtualKeyCode)
                {
                    Dispatcher.InvokeAsync(Show);
                    if (_config.InterceptKey)
                        return (IntPtr) 1; // これ以上処理しない
                }
            }
            return User.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _backlog.Clear();

            // ログの読みこみ
            if (File.Exists(_logFile))
                foreach (string line in File.ReadLines(_logFile, Encoding.UTF8))
                    if (!string.IsNullOrWhiteSpace(line))
                        _backlog.Insert(0, line);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // ログの書き出し
            using (var sw = new StreamWriter(_logFile, false, Encoding.UTF8))
            {
                for (int i = _backlog.Count - 1; i >= 0; i--)
                    sw.WriteLine(_backlog[i]);
                sw.Close();
            }

            _backlog.Clear();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            // ウィンドウ位置の調整
            Screen scr = Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            Left = scr.WorkingArea.X + (scr.WorkingArea.Width >> 1) - (Width/2);
            Top = scr.WorkingArea.Bottom - Height - 10;
            Topmost = true;

            // テキストボックスの初期化
            textBox.Text = "";
            textBox.Focus();

            _backlogTemp = null;
            _backlogPos = -1;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            string text = textBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    if (_backlog.Last() != text)
                    {
                        _backlog.Insert(0, text);
                        while (_backlog.Count > LogCount)
                            _backlog.RemoveAt(_backlog.Count - 1);
                    }
                }
                catch (InvalidOperationException)
                {
                    _backlog.Insert(0, text);
                }
            }
            RaiseUserInputCancel();
            textBox.Text = "";
            Hide();
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (_backlogPos < 0)
                        _backlogTemp = textBox.Text;
                    if (_backlogPos < _backlog.Count - 1)
                        textBox.Text = _backlog[++_backlogPos];
                    textBox.CaretIndex = textBox.Text.Length;
                    break;
                case Key.Down:
                    if (_backlogPos > 0)
                        textBox.Text = _backlog[--_backlogPos];
                    else if (_backlogPos == 0)
                    {
                        textBox.Text = _backlogTemp;
                        _backlogPos = -1;
                    }
                    textBox.CaretIndex = textBox.Text.Length;
                    break;
                case Key.Enter:
                    string text = textBox.Text;
                    if (string.IsNullOrWhiteSpace(text))
                        RaiseUserInputCancel();
                    else
                    {
                        try
                        {
                            if (_backlog.Last() != text)
                            {
                                _backlog.Insert(0, text);
                                while (_backlog.Count > LogCount)
                                    _backlog.RemoveAt(_backlog.Count - 1);
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            _backlog.Insert(0, text);
                        }

                        RaiseUserInput(text);
                    }
                    textBox.Text = "";
                    Hide();
                    break;
            }
        }

        public new void Show()
        {
            RaiseUserInputStart();

            base.Show();
            SetAsForeground(Handle);
            textBox.Focus();
        }

        private static void SetAsForeground(IntPtr hWnd)
        {
            IntPtr hForegroundWnd = User.GetForegroundWindow();

            int pid;
            int targetThreadId = User.GetWindowThreadProcessId(hWnd, out pid);
            int foregroundThreadId = User.GetWindowThreadProcessId(hForegroundWnd, out pid);

            User.SetForegroundWindow(hWnd);
            if (targetThreadId == foregroundThreadId)
            {
                User.BringWindowToTop(hWnd);
            }
            else
            {
                User.AttachThreadInput(targetThreadId, foregroundThreadId, true);
                try
                {
                    User.BringWindowToTop(hWnd);
                }
                finally
                {
                    User.AttachThreadInput(targetThreadId, foregroundThreadId, false);
                }
            }
        }

        private void RaiseUserInputStart()
        {
            EventHandler handler = UserInputStart;
            if (handler != null)
                handler(this, new EventArgs());
        }

        private void RaiseUserInputCancel()
        {
            EventHandler handler = UserInputCancel;
            if (handler != null)
                handler(this, new EventArgs());
        }

        private void RaiseUserInput(string input)
        {
            EventHandler<EventArgs<string>> handler = UserInput;
            if (handler != null)
                handler(this, new EventArgs<string>(input));
        }
    }
}