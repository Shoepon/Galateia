using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aphrodite;
using Galateia.CSC;
using Galateia.Infra.Graphics;
using Galateia.Infra.Windows;
using Galateia.Infra.WindowsAPI;
using MMDFileParser.PMXModelParser;
using MMF;
using MMF.Bone;
using MMF.DeviceManager;
using MMF.Light;
using MMF.Matricies;
using MMF.Matricies.Camera;
using MMF.Matricies.World;
using MMF.Model.PMX;
using MMF.Motion;
using SlimDX;
using SlimDX.DXGI;
using D2D = SlimDX.Direct2D;
using DW = SlimDX.DirectWrite;
using TextureTargetContext = Galateia.Infra.Graphics.TextureTargetContext;

namespace Galateia.Shell
{
    public class ShellWindow : FramelessLayeredWindow, IShell
    {
        private bool _disposed;

        // 設定
        private readonly ShellGlobalConfig _globalConfig;

        public ShellGlobalConfig GlobalConfig
        {
            get { return _globalConfig; }
        }

        /// <summary>
        /// バルーン基準点のスクリーン座標を取得します．
        /// </summary>
        public Point BaloonReferencePoint { get; private set; }

        private readonly ShellHookWindow _shellHook;

        // 描画に必要なDirectX コンテクスト
        private readonly GraphicDeviceManager _deviceManager;
        private readonly GdiCompatibleContext _gdiContext;
        private readonly RenderContext _renderContext;
        private readonly TextureTargetContext _textureTargetContext;
#if DEBUG
        // 文字描画用のリソース
        private D2D.SolidColorBrush _textBrush;
        private DW.TextFormat _textFormat;
#endif
        /// <summary>
        ///     ウィンドウのサイズを取得または設定します．
        /// </summary>
        public new Size Size
        {
            get { return base.Size; }
            protected set
            {
                base.Size = _gdiContext.Size = _textureTargetContext.Size = value;
            }
        }

        public WorldSpace WorldSpace
        {
            get { return _textureTargetContext.WorldSpace; }
        }

        /// <summary>
        ///     デバイスマネージャと設定を指定して新しいShellウィンドウを作成します．
        /// </summary>
        public ShellWindow(
            GraphicDeviceManager deviceManager,
            ShellHookWindow shellHook,
            ShellGlobalConfig globalConfig)
        {
            ShowActivated = false;

            _deviceManager = deviceManager;
            _shellHook = shellHook;
            _globalConfig = globalConfig;

            globalConfig.Substituted += globalConfig_Substituted;

            // フックの登録
            shellHook.WindowActivated += shellHook_WindowActivated;

            // ウィンドウサイズの決定
            var wndSize = new Size(1, 1);

            ////
            // 描画用コンテキストの準備
            // レンダーコンテキストの作成
            _renderContext = RenderContext.CreateContext(deviceManager);
            // GDI互換コンテクストの初期化
            _gdiContext = new GdiCompatibleContext(deviceManager);
            _gdiContext.TargetsInitialized += context_TargetsInitialized;
            _gdiContext.TargetsDestroying += context_TargetsDestroying;
            // MMF描画コンテクストの初期化
            var camera = new BasicCamera(new Vector3(0, 0, -1000), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            var projection = new OrthogonalProjectionMatrixProvider();
            projection.InitializeProjection(
                wndSize.Width/globalConfig.PixelPerUnitLength,
                wndSize.Height/globalConfig.PixelPerUnitLength, 1, 2000);
            var matrixManager = new MatrixManager(new BasicWorldMatrixProvider(), camera, projection);
            _textureTargetContext = new TextureTargetContext(_renderContext, matrixManager, wndSize,
                new SampleDescription(globalConfig.MultiSampleCount, globalConfig.MultiSampleQuality))
            {
                BackgroundColor = new Color4(0)
            };
            _renderContext.LightManager = new LightMatrixManager(_textureTargetContext.MatrixManager);
            _renderContext.Timer = new MotionTimer(_renderContext);
            _renderContext.UpdateRequireWorlds.Add(_textureTargetContext.WorldSpace);

            // ウィンドウサイズの適用
            Size = wndSize;
            Opacity = _globalConfig.Opacity;

            // 追加のフラグを指定して作成
            WindowStylesEx extra = WindowStylesEx.ToolWindow | WindowStylesEx.TopMost;
            if (globalConfig.PerviousMouse)
                extra |= WindowStylesEx.Transparent;
            base.Create(extra);
            
        }

        private void globalConfig_Substituted(object sender, EventArgs e)
        {
            Opacity = _globalConfig.Opacity;
            if (_globalConfig.PerviousMouse)
                WindowStylesEx |= WindowStylesEx.Transparent;
            else
                WindowStylesEx &= ~WindowStylesEx.Transparent;
            _textureTargetContext.SampleDesc = new SampleDescription(_globalConfig.MultiSampleCount, _globalConfig.MultiSampleQuality);
        }

        public void LoadModel(PMXModel model)
        {
            model.Load(_renderContext);
        }

        /// <summary>
        ///     アクティブウィンドウが変わったら最前面を取り直す．
        /// </summary>
        private void shellHook_WindowActivated(object sender, ShellHookEventArgs e)
        {
            User.SetWindowPos(Handle, new IntPtr(-1) /* HWND_TOPMOST */, 0, 0, 0, 0,
                SetWindowPosFlag.NoActivate | SetWindowPosFlag.NoMove | SetWindowPosFlag.NoSize);
        }

        ~ShellWindow()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                _shellHook.WindowActivated -= shellHook_WindowActivated;
                _globalConfig.Substituted -= globalConfig_Substituted;

                User.CloseWindow(Handle);

                _gdiContext.Dispose();
                _textureTargetContext.Dispose();
                _renderContext.Dispose();

                _disposed = true;
            }
        }

        /// <summary>
        ///     GDI互換コンテクストでレンダーターゲットが初期化された際の処理
        /// </summary>
        private void context_TargetsInitialized(object sender, EventArgs e)
        {
            _gdiContext.D2DRenderTarget.AntialiasMode = D2D.AntialiasMode.Aliased;
            _gdiContext.D2DRenderTarget.TextAntialiasMode = D2D.TextAntialiasMode.Aliased;

#if DEBUG
            _textFormat = _deviceManager.DWFactory.CreateTextFormat("MS Gothic", DW.FontWeight.Normal, DW.FontStyle.Normal,
                DW.FontStretch.Normal, 20, "");
            _textFormat.TextAlignment = DW.TextAlignment.Center;
            _textFormat.ParagraphAlignment = DW.ParagraphAlignment.Center;
            _textBrush = new D2D.SolidColorBrush(_gdiContext.D2DRenderTarget, new Color4(1.0f, 1.0f, 0.2f, 1.0f));
#endif
        }

        /// <summary>
        ///     GDI互換コンテクストでレンダーターゲットが削除される際の処理
        /// </summary>
        private void context_TargetsDestroying(object sender, EventArgs e)
        {
#if DEBUG
            _textBrush.Dispose();
            _textFormat.Dispose();
#endif
        }

        private static readonly IntPtr SPI_SETWORKAREA = new IntPtr(0x002F);

        /// <summary>
        ///     ウィンドウプロシージャ
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch ((WindowMessages) m.Msg)
            {
                case WindowMessages.SettingChange:
                    if (m.WParam == SPI_SETWORKAREA)
                        RaiseScreenChanged();
                    break;
                case WindowMessages.DisplayChange:
                    RaiseScreenChanged();
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///     表示領域が変化した場合に呼び出されます．
        /// </summary>
        public event EventHandler<ScreenChangedEventArgs> ScreenChanged;

        private void RaiseScreenChanged()
        {
            Func<Screen, RectangleF> screenToArea = s => new RectangleF(
                s.WorkingArea.X/_globalConfig.PixelPerUnitLength,
                0,
                s.WorkingArea.Width/_globalConfig.PixelPerUnitLength,
                s.WorkingArea.Height/_globalConfig.PixelPerUnitLength);

            if (ScreenChanged != null)
                ScreenChanged(this, new ScreenChangedEventArgs(from scr in Screen.AllScreens select screenToArea(scr)));
        }

        /// <summary>
        ///     描画処理を完了した際に呼び出されます．
        /// </summary>
        public event EventHandler Rendered;

        /// <summary>
        ///     描画処理
        /// </summary>
        public void Render(IDemeanor demeanor)
        {
            // ワールドを更新
            _renderContext.UpdateWorlds();

            if (IsVisible)
            {
                // 描画前の処理を行います．
                RectangleF regionInWorld;
                demeanor.PreRender(out regionInWorld);
                var workingArea =
                    Screen.FromPoint(new Point((int) (regionInWorld.X*_globalConfig.PixelPerUnitLength), 0)).WorkingArea;
                var regionInScreen = WorldToScreen(regionInWorld, workingArea);
                // ウィンドウの位置
                var windowPos = new Point((int) regionInScreen.X, (int) regionInScreen.Y);
                var windowSize = new Size(
                    (int) Math.Ceiling(regionInScreen.Right) - windowPos.X,
                    (int) Math.Ceiling(regionInScreen.Bottom) - windowPos.Y);
                if (Size != windowSize)
                {
                    //int deltaWidth = Size.Width - windowSize.Width;
                    //int deltaHeight = Size.Height - windowSize.Height;
                    //if (deltaWidth < -10 || deltaWidth > 0
                    //    || deltaHeight < -10 || deltaHeight > 0)
                    {
                        Size = windowSize;
                        _textureTargetContext.MatrixManager.ProjectionMatrixManager.InitializeProjection(
                            windowSize.Width/_globalConfig.PixelPerUnitLength,
                            windowSize.Height/_globalConfig.PixelPerUnitLength, 1, 2000);
                    }
                    // else
                    // {
                    //     windowSize = Size;
                    // }
                }
                // カメラ位置
                var cameraInScreen = new PointF(
                    windowPos.X + windowSize.Width/2.0f,
                    windowPos.Y + windowSize.Height/2.0f);
                var cameraInWorld = ScreenToWorld(cameraInScreen, workingArea);
                // レンダーターゲットサイズ・カメラ位置を設定
                Position = windowPos;
                _textureTargetContext.MatrixManager.ViewMatrixManager.CameraPosition
                    = new Vector3(cameraInWorld.X, cameraInWorld.Y, -1000);
                _textureTargetContext.MatrixManager.ViewMatrixManager.CameraLookAt
                    = new Vector3(cameraInWorld.X, cameraInWorld.Y, 0);

                // 描画
                _textureTargetContext.Render();

                // バルーン基準点（描画した後）
                BaloonReferencePoint = WorldToScreen(demeanor.BaloonReferencePoint, workingArea);

                // 描画内容をGDI互換コンテクストにコピー
                D2D.RenderTarget target = _gdiContext.D2DRenderTarget;
                target.BeginDraw();
                if (_textureTargetContext.SampleDesc.Count > 1)
                    _gdiContext.DeviceManager.Context.ResolveSubresource(
                        _textureTargetContext.RenderTarget, 0,
                        _gdiContext.D3DRenderTarget, 0,
                        _gdiContext.D3DRenderTarget.Description.Format);
                else
                    _gdiContext.DeviceManager.Context.CopyResource(
                        _textureTargetContext.RenderTarget, _gdiContext.D3DRenderTarget);

#if DEBUG
                // Direct2Dの描画
                target.DrawText(string.Format("{0:0.0}FPS", App.Current.Renderer.FpsCounter.FPS), _textFormat,
                    new Rectangle(0, 0, Size.Width, (int) (_textFormat.FontSize + 1)), _textBrush);
#endif

                // ウィンドウに表示
                using (var dc = new RenderTargetDC(_gdiContext.D2DGdiInteropRenderTarget))
                    UpdateLayeredWindow(dc);
                target.EndDraw();
            }

            if (Rendered != null)
                Rendered(this, new EventArgs());
        }

        private PointF ScreenToWorld(PointF point, Rectangle area)
        {
            return new PointF(
                point.X / _globalConfig.PixelPerUnitLength,
                (area.Bottom - point.Y) / _globalConfig.PixelPerUnitLength);
        }

        private Point WorldToScreen(PointF point, Rectangle area)
        {
            return new Point(
                (int)(point.X * _globalConfig.PixelPerUnitLength),
                (int)(area.Bottom - point.Y * _globalConfig.PixelPerUnitLength));
        }

        private RectangleF WorldToScreen(RectangleF rect, Rectangle area)
        {
            return new RectangleF(
                rect.Left * _globalConfig.PixelPerUnitLength,
                area.Bottom - rect.Bottom * _globalConfig.PixelPerUnitLength,
                rect.Width * _globalConfig.PixelPerUnitLength,
                rect.Height * _globalConfig.PixelPerUnitLength);
        }
    }
}