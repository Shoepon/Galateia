using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Galateia.Infra.Windows;
using Galateia.Infra.WindowsAPI;

namespace Galateia.Baloon
{
    /// <summary>
    ///     BaloonWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BaloonWindow : Window, INotifyPropertyChanged
    {
        public enum Directions
        {
            Left,
            Right
        }

        /// <summary>
        ///     単語の自動ハイフネーションが有効かどうかを示す値を取得または設定します．
        /// </summary>
        public static readonly DependencyProperty IsHyphenationEnabledProperty = DependencyProperty.Register(
            "IsHyphenationEnabled",
            typeof (bool),
            typeof (BaloonWindow),
            new PropertyMetadata(true,
                (d, e) => ((BaloonWindow) d).RaisePropertyChanged("IsHyphenationEnabled"))
            );

        /// <summary>
        ///     各コンテンツ行の高さを取得または設定します．
        /// </summary>
        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register(
            "LineHeight",
            typeof (double),
            typeof (BaloonWindow),
            new PropertyMetadata(1.0,
                (d, e) => ((BaloonWindow) d).RaisePropertyChanged("LineHeight"))
            );

        /// <summary>
        ///     各テキスト行の行ボックスを決定する際に使用する機構を取得または設定します．
        /// </summary>
        public static readonly DependencyProperty LineStackingStrategyProperty = DependencyProperty.Register(
            "LineStackingStrategy",
            typeof (LineStackingStrategy),
            typeof (BaloonWindow),
            new PropertyMetadata(LineStackingStrategy.MaxHeight,
                (d, e) => ((BaloonWindow) d).RaisePropertyChanged("LineStackingStrategy"))
            );

        /// <summary>
        ///     アンカーのIDと，ハンドラ＆イベントデータの連想辞書です．
        /// </summary>
        private readonly Dictionary<Guid, Tuple<EventHandler<AnchorClickEventArgs>, AnchorClickEventArgs>> _anchors =
            new Dictionary<Guid, Tuple<EventHandler<AnchorClickEventArgs>, AnchorClickEventArgs>>();

        private readonly BaloonConfig _config;
        private readonly BaloonGlobalConfig _globalConfig;

        /// <summary>
        ///     ドキュメント
        /// </summary>
        private readonly Paragraph _paragraph;

        private readonly ShellHookWindow _shellHook;

        /// <summary>
        ///     現在の操作対称のInline要素の先祖要素のスタック．
        /// </summary>
        private readonly Stack<Inline> _stack = new Stack<Inline>();

        private readonly DispatcherTimer _timeoutTimer;
        private BitmapImage _bodyBrushImage;

        private Directions _direction = Directions.Left;
        private BitmapImage _footerImage;
        private BitmapImage _headerImage;
        private BitmapImage _leftSideImage;
        private BitmapImage _rightSideImage;
        private Thickness _viewerMargin = new Thickness(3);

        /// <summary>
        ///     新しいインスタンスを初期化します．
        /// </summary>
        public BaloonWindow(BaloonConfig config, ShellHookWindow shellHook, BaloonGlobalConfig globalConfig)
        {
            this._globalConfig = globalConfig;
            this._config = config;
            this._shellHook = shellHook;

            InitializeComponent();

            // 段落を設定
            _paragraph = new Paragraph();
            document.Blocks.Add(_paragraph);

            // タイムアウトタイマーの初期化
            _timeoutTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(globalConfig.TimeoutInSecond),
                DispatcherPriority.Normal,
                (object _, EventArgs __) =>
                {
                    _timeoutTimer.Stop();
                    Hide();
                },
                Dispatcher) {IsEnabled = false};
            GetStoryboard("fadeout").Completed += Fadeout_Completed;
        }

        /// <summary>
        ///     単語の自動ハイフネーションが有効かどうかを示す値を取得または設定します．
        /// </summary>
        public bool IsHyphenationEnabled
        {
            get { return (bool) GetValue(IsHyphenationEnabledProperty); }
            set { SetValue(IsHyphenationEnabledProperty, value); }
        }

        /// <summary>
        ///     各コンテンツ行の高さを取得または設定します．
        /// </summary>
        public double LineHeight
        {
            get { return (double) GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        /// <summary>
        ///     各テキスト行の行ボックスを決定する際に使用する機構を取得または設定します．
        /// </summary>
        public LineStackingStrategy LineStackingStrategy
        {
            get { return (LineStackingStrategy) GetValue(LineStackingStrategyProperty); }
            set { SetValue(LineStackingStrategyProperty, value); }
        }

        /// <summary>
        ///     バルーンの設定を取得します．
        /// </summary>
        public BaloonConfig Config
        {
            get { return _config; }
        }

        /// <summary>
        ///     バルーンの設定を取得します．
        /// </summary>
        public BaloonGlobalConfig GlobalConfig
        {
            get { return _globalConfig; }
        }

        /// <summary>
        ///     バルーンのモデルに対する表示の側を取得または設定します．
        /// </summary>
        public Directions Direction
        {
            get { return _direction; }
            set
            {
                if (_direction != value)
                {
                    _direction = value;

                    BaloonDesign bwc = CurrentWindowConfig;
                    HeaderImage = new BitmapImage(new Uri(Path.GetFullPath(Path.Combine(_config.BaseDirectory, bwc.HeaderImage))));
                    LeftSideImage = new BitmapImage(new Uri(Path.GetFullPath(Path.Combine(_config.BaseDirectory, bwc.LeftSideImage))));
                    BodyBrushImage = new BitmapImage(new Uri(Path.GetFullPath(Path.Combine(_config.BaseDirectory, bwc.BodyBrushImage))));
                    RightSideImage = new BitmapImage(new Uri(Path.GetFullPath(Path.Combine(_config.BaseDirectory, bwc.RightSideImage))));
                    FooterImage = new BitmapImage(new Uri(Path.GetFullPath(Path.Combine(_config.BaseDirectory, bwc.FooterImage))));
                    ViewerMargin = bwc.ViewerMargin;
                    Width = HeaderImage.Width;
                    FontFamily = new FontFamily(bwc.FontFamily);
                    FontSize = bwc.FontSize;
                }
            }
        }

        /// <summary>
        ///     現在のバルーン表示位置に関連付けられた設定．
        /// </summary>
        public BaloonDesign CurrentWindowConfig
        {
            get { return _direction == Directions.Left ? _config.LeftBaloon : _config.RightBaloon; }
        }

        public BitmapImage HeaderImage
        {
            get { return _headerImage; }
            set
            {
                _headerImage = value;
                RaisePropertyChanged("HeaderImage");
            }
        }

        public BitmapImage FooterImage
        {
            get { return _footerImage; }
            set
            {
                _footerImage = value;
                RaisePropertyChanged("FooterImage");
            }
        }

        public BitmapImage LeftSideImage
        {
            get { return _leftSideImage; }
            set
            {
                _leftSideImage = value;
                RaisePropertyChanged("LeftSideImage");
            }
        }

        public BitmapImage RightSideImage
        {
            get { return _rightSideImage; }
            set
            {
                _rightSideImage = value;
                RaisePropertyChanged("RightSideImage");
            }
        }

        public BitmapImage BodyBrushImage
        {
            get { return _bodyBrushImage; }
            set
            {
                _bodyBrushImage = value;
                RaisePropertyChanged("BodyBrushImage");
            }
        }

        public Thickness ViewerMargin
        {
            get { return _viewerMargin; }
            set
            {
                _viewerMargin = value;
                RaisePropertyChanged("ViewerMargin");
            }
        }

        /// <summary>
        ///     プロパティが変更されたことを通知します．
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // サイズ変更時の処理を登録
            _shellHook.WindowActivated += shellHook_WindowActivated;
            _globalConfig.Substituted += globalConfig_Substituted;
        }

        private void globalConfig_Substituted(object sender, EventArgs e)
        {
            _timeoutTimer.Interval = TimeSpan.FromSeconds(_globalConfig.TimeoutInSecond);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _globalConfig.Substituted -= globalConfig_Substituted;
            _shellHook.WindowActivated -= shellHook_WindowActivated;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WindowMessages) msg)
            {
                case WindowMessages.WindowPosChanging:
                    unsafe
                    {
                        var p = (WINDOWPOS*) lParam;
                        if ((uint) (p->flags & SetWindowPosFlag.NoSize) == 0) // サイズ変更がある場合，ウィンドウのボトムラインを一定に保つ
                        {
                            p->x = (int) Left;
                            p->y = (int) (Top - (p->cy - ActualHeight));
                            p->flags &= ~SetWindowPosFlag.NoMove;
                        }
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void shellHook_WindowActivated(object sender, ShellHookEventArgs e)
        {
            Topmost = false;
            Topmost = true;
        }

        public new void Show()
        {
            if (!IsVisible)
            {
                Dispatcher.Invoke(() =>
                {
                    _timeoutTimer.IsEnabled = false;
                    base.Opacity = 0;
                    base.Show();
                    GetStoryboard("fadein").Begin();
                });
            }
        }

        public void StartTimeoutCountdown()
        {
            _timeoutTimer.Start();
        }

        public void StopTimeoutCountdown()
        {
            _timeoutTimer.Stop();
        }

        private void window_MouseMove(object sender, MouseEventArgs e)
        {
            // タイムアウトタイマーをリセット
            if (_timeoutTimer.IsEnabled)
            {
                _timeoutTimer.Stop();
                _timeoutTimer.Start();
            }
        }

        public new void Hide()
        {
            Dispatcher.Invoke(() => { GetStoryboard("fadeout").Begin(); });
        }

        private void Fadeout_Completed(object sender, EventArgs e)
        {
            base.Hide();
        }

        private Storyboard GetStoryboard(string key)
        {
            return (Storyboard) Resources[key];
        }

        /// <summary>
        ///     一番近いSpan要素またはParagraphのインライン コレクションを取得します．
        /// </summary>
        /// <returns>インライン コレクション．</returns>
        private InlineCollection GetNearestInlineCollection()
        {
            while (_stack.Count > 0 && !(_stack.Peek() is Span))
                _stack.Pop();
            return _stack.Count > 0 ? ((Span) _stack.Peek()).Inlines : _paragraph.Inlines;
        }

        /// <summary>
        ///     末尾に文字列を追加します．
        /// </summary>
        /// <param name="text">追加する文字列．</param>
        public void Append(string text)
        {
            Dispatcher.Invoke((Action<string>) AppendImpl, text);
        }

        private void AppendImpl(string text)
        {
            // 追加するRunを作成
            Run run, oldrun = null;
            if (_stack.Count > 0 && (_stack.Peek() is Run))
            {
                oldrun = (Run) _stack.Pop();
                run = new Run(oldrun.Text + text);
            }
            else
                run = new Run(text);

            // Runを追加するべきコレクションを取得
            InlineCollection inlines = GetNearestInlineCollection();
            inlines.Remove(oldrun);
            inlines.Add(run);
            // スタックに乗せておく
            _stack.Push(run);
        }


        /// <summary>
        /// 末尾に画像を追加します．
        /// </summary>
        /// <param name="path">画像ファイルのパス．</param>
        public void AppendImage(string path)
        {
            Dispatcher.Invoke((Action<string>)AppendImageImpl, path);
        }

        private void AppendImageImpl(string path)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = new Uri(path);
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            var image = new Image
            {
                Source = bitmapImage
            };

            if (_stack.Count > 0 && (_stack.Peek() is Run))
                _stack.Pop();

            InlineCollection inlines = GetNearestInlineCollection();
            inlines.Add(image);
        }

        /// <summary>
        ///     ユーザーがクリックできるアンカーを開始します．
        /// </summary>
        /// <param name="handler">アンカーがクリックされたときに呼び出されるハンドラ．</param>
        /// <param name="data">ハンドラに渡されるデータ．</param>
        public void BeginAnchor(EventHandler<AnchorClickEventArgs> handler, object data)
        {
            Dispatcher.Invoke((Action<EventHandler<AnchorClickEventArgs>, object>) BeginAnchorImpl, handler, data);
        }

        private void BeginAnchorImpl(EventHandler<AnchorClickEventArgs> handler, object data)
        {
            Guid id = Guid.NewGuid();

            var anchor = new Hyperlink();
            anchor.Tag = id;
            anchor.Click += anchor_Click;

            InlineCollection inlines = GetNearestInlineCollection();
            inlines.Add(anchor);

            _anchors.Add(id,
                new Tuple<EventHandler<AnchorClickEventArgs>, AnchorClickEventArgs>(handler,
                    new AnchorClickEventArgs(data)));

            _stack.Push(anchor);
        }

        private void anchor_Click(object sender, RoutedEventArgs e)
        {
            var anchor = sender as Hyperlink;
            if (anchor != null)
            {
                var tag = (Guid) anchor.Tag;
                if (_anchors.ContainsKey(tag))
                {
                    Tuple<EventHandler<AnchorClickEventArgs>, AnchorClickEventArgs> entry = _anchors[tag];
                    entry.Item1(this, entry.Item2);
                    _anchors.Remove(tag);
                }
            }
        }

        /// <summary>
        ///     アンカーから抜けます．
        /// </summary>
        public void EndAnchor()
        {
            // HyperlinkでないものはPOPする
            while (_stack.Count > 0 && !(_stack.Peek() is Hyperlink))
                _stack.Pop();

            // HyperlinkをPOPする
            if (_stack.Count > 0)
                _stack.Pop();
        }

        /// <summary>
        ///     改行します．
        /// </summary>
        public void NewLine()
        {
            Dispatcher.Invoke(NewLineImpl);
        }

        private void NewLineImpl()
        {
            while (_stack.Count > 0 && !(_stack.Peek() is Span))
                _stack.Pop();
            InlineCollection inlines = _stack.Count > 0 ? ((Span) _stack.Peek()).Inlines : _paragraph.Inlines;
            inlines.Add(new LineBreak());
        }

        /// <summary>
        ///     表示内容をクリアします．
        /// </summary>
        public void Clear()
        {
            Dispatcher.Invoke(ClearImpl);
        }

        private void ClearImpl()
        {
            _paragraph.Inlines.Clear();
            _stack.Clear();
            _anchors.Clear();
        }


        /// <summary>
        ///     プロパティが変更されたことを通知します．
        /// </summary>
        /// <param name="name">プロパティ名．</param>
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler pc = PropertyChanged;
            if (pc != null)
                pc(this, new PropertyChangedEventArgs(name));
        }
    }
}