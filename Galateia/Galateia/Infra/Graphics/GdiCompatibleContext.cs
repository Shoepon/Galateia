using System;
using System.Drawing;
using SlimDX.DXGI;
using D2D = SlimDX.Direct2D;
using D3D = SlimDX.Direct3D11;

namespace Galateia.Infra.Graphics
{
    /// <summary>
    ///     GDI互換の DXGI 1.1 サーフェスおよび Direct2D 1.0, Direct3D 11.0 を提供します．
    /// </summary>
    public class GdiCompatibleContext : IDisposable
    {
        private readonly GraphicDeviceManager deviceManager;
        private D2D.GdiInteropRenderTarget d2dGdiInteropRenderTarget;
        private D2D.RenderTarget d2dRenderTarget;
        private D3D.Texture2D d3dRenderTarget;
        private D3D.RenderTargetView d3dRenderTargetView;
        private bool disposed = false;
        private Surface dxgiSurface;

        private Size size;

        /// <summary>
        ///     デバイスマネージャを指定してコンテキストを初期化します．
        /// </summary>
        /// <param name="deviceManager"></param>
        public GdiCompatibleContext(GraphicDeviceManager deviceManager)
        {
            this.deviceManager = deviceManager;
        }

        /// <summary>
        ///     ウィンドウのサイズを取得または設定して（再）初期化します．
        /// </summary>
        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                ResetTargets();
            }
        }

        /// <summary>
        ///     デバイスマネージャを取得します．
        /// </summary>
        public GraphicDeviceManager DeviceManager
        {
            get { return deviceManager; }
        }

        /// <summary>
        ///     Direct3Dレンダーターゲットを取得します．
        /// </summary>
        public D3D.Texture2D D3DRenderTarget
        {
            get { return d3dRenderTarget; }
        }

        /// <summary>
        ///     Direct3Dレンダーターゲットビューを取得します．
        /// </summary>
        public D3D.RenderTargetView D3DRenderTargetView
        {
            get { return d3dRenderTargetView; }
        }

        /// <summary>
        ///     DXGIサーフェスを取得します．
        /// </summary>
        public Surface DXGISurface
        {
            get { return dxgiSurface; }
        }

        /// <summary>
        ///     Direct2Dレンダーターゲットを取得します．
        /// </summary>
        public D2D.RenderTarget D2DRenderTarget
        {
            get { return d2dRenderTarget; }
        }

        /// <summary>
        ///     Direct2D GDI互換ターゲットを取得します．
        /// </summary>
        public D2D.GdiInteropRenderTarget D2DGdiInteropRenderTarget
        {
            get { return d2dGdiInteropRenderTarget; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     レンダーターゲットが削除される前に呼び出されます．
        /// </summary>
        public event EventHandler TargetsDestroying;

        /// <summary>
        ///     レンダーターゲットが初期化された際に呼び出されます．
        /// </summary>
        public event EventHandler TargetsInitialized;

        /// <summary>
        ///     レンダーターゲットを初期化し，描画が行えるようにします．
        /// </summary>
        /// <param name="size"></param>
        public void Initialize(Size size)
        {
            this.size = size;
            ResetTargets();
        }

        /// <summary>
        ///     レンダーターゲットなど一式リセットします．
        /// </summary>
        private void ResetTargets()
        {
            DestroyTargets();
            InitializeTargets();
        }

        /// <summary>
        ///     レンダーターゲットを初期化します．
        /// </summary>
        private void InitializeTargets()
        {
            {
                // Direct3D レンダーターゲットの作成
                var desc = new D3D.Texture2DDescription
                {
                    Width = size.Width,
                    Height = size.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = D3D.ResourceUsage.Default,
                    BindFlags = D3D.BindFlags.RenderTarget,
                    CpuAccessFlags = D3D.CpuAccessFlags.None,
                    OptionFlags = D3D.ResourceOptionFlags.GdiCompatible
                };
                d3dRenderTarget = new D3D.Texture2D(deviceManager.Device, desc);
            }
            {
                // レンダーターゲットビューの作成
                D3D.Texture2D target = D3DRenderTarget;
                var desc = new D3D.RenderTargetViewDescription
                {
                    Format = target.Description.Format,
                    Dimension =
                        target.Description.SampleDescription.Count > 1
                            ? D3D.RenderTargetViewDimension.Texture2DMultisampled
                            : D3D.RenderTargetViewDimension.Texture2D,
                    MipSlice = 0
                };
                d3dRenderTargetView = new D3D.RenderTargetView(deviceManager.Device, target, desc);
            }
            // DXGIサーフェスの取得
            dxgiSurface = d3dRenderTarget.AsSurface();
            {
                // レンダーターゲットの作成
                var properties = new D2D.RenderTargetProperties
                {
                    Type = D2D.RenderTargetType.Default,
                    PixelFormat = new D2D.PixelFormat(dxgiSurface.Description.Format, D2D.AlphaMode.Premultiplied),
                    HorizontalDpi = 0,
                    VerticalDpi = 0,
                    Usage = D2D.RenderTargetUsage.GdiCompatible,
                    MinimumFeatureLevel = D2D.FeatureLevel.Direct3D10
                };
                d2dRenderTarget = D2D.RenderTarget.FromDXGI(deviceManager.D2DFactory, dxgiSurface, properties);
                d2dGdiInteropRenderTarget = new D2D.GdiInteropRenderTarget(d2dRenderTarget);
            }

            if (TargetsInitialized != null)
                TargetsInitialized(this, new EventArgs());
        }

        /// <summary>
        ///     レンダーターゲットを破棄します．
        /// </summary>
        private void DestroyTargets()
        {
            if (DXGISurface != null && TargetsDestroying != null)
                TargetsDestroying(this, new EventArgs());

            SafeRelease(d2dGdiInteropRenderTarget);
            SafeRelease(d2dRenderTarget);
            SafeRelease(dxgiSurface);
            SafeRelease(d3dRenderTargetView);
            SafeRelease(d3dRenderTarget);

            d2dGdiInteropRenderTarget = null;
            d2dRenderTarget = null;
            dxgiSurface = null;
            d3dRenderTargetView = null;
            d3dRenderTarget = null;
        }

        ~GdiCompatibleContext()
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

                DestroyTargets();
            }
        }

        /// <summary>
        ///     引数がnullでない場合はDisposeします
        /// </summary>
        private static void SafeRelease(IDisposable disposable)
        {
            if (disposable != null)
                disposable.Dispose();
        }
    }
}