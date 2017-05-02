using System;
using System.Drawing;
using MMF;
using MMF.DeviceManager;
using MMF.Matricies;
using MMF.Matricies.Camera;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace Galateia.Infra.Graphics
{
    public class TextureTargetContext : ITargetContext
    {
        private RenderContext context;
        private Texture2D depthTarget;
        private DepthStencilView depthTargetView;
        private bool disposed;
        private Texture2D renderTarget;
        private RenderTargetView renderTargetView;

        public TexturedBufferHitChecker HitChecker { get; private set; }

        private SampleDescription sampleDesc = new SampleDescription(1, 0);

        private Size size;

        public TextureTargetContext(RenderContext context, MatrixManager matrixManager, Size size,
            SampleDescription sampleDesc)
        {
            this.context = context;

            // サイズを設定（ターゲットは初期化しない）
            this.size = size;
            // マルチサンプルの設定（ターゲットも初期化する）
            SampleDesc = sampleDesc;

            HitChecker = new TexturedBufferHitChecker(context);

            // その他設定
            MatrixManager = matrixManager;
            WorldSpace = new WorldSpace(context);
            SetViewport();
        }

        /// <summary>
        ///     マルチサンプルの設定を取得または設定します．
        ///     可能な限り，指定された値に近い設定を採用します．
        /// </summary>
        public SampleDescription SampleDesc
        {
            get { return sampleDesc; }
            set
            {
                Device device = context.DeviceManager.Device;
                Format format = getRenderTargetTexture2DDescription().Format;
                int count = value.Count;
                do
                {
                    int msql = device.CheckMultisampleQualityLevels(format, count);
                    if (msql > 0)
                    {
                        int quality = Math.Min(msql - 1, value.Quality);
                        sampleDesc = new SampleDescription(count, quality);
                        break;
                    }

                    // マルチサンプル数がサポートされない場合
                    count--;
                } while (count > 0);
                ResetTargets();
            }
        }

        /// <summary>
        ///     レンダーターゲットのサイズを取得または設定します．
        /// </summary>
        public Size Size
        {
            get { return size; }
            set
            {
                if (size != value)
                {
                    size = value;
                    ResetTargets();
                }
            }
        }

        /// <summary>
        ///     レンダー ターゲットを取得します．
        /// </summary>
        public Texture2D RenderTarget
        {
            get { return renderTarget; }
        }

        /// <summary>
        ///     Gets the depth target.
        /// </summary>
        public Texture2D DepthTarget
        {
            get { return depthTarget; }
        }

        /// <summary>
        ///     背景色を取得または設定します．
        /// </summary>
        public Color4 BackgroundColor { get; set; }

        /// <summary>
        ///     レンダー コンテキストを取得または設定します．
        /// </summary>
        public RenderContext Context
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        ///     レンダー ターゲット ビューを取得します．
        /// </summary>
        public RenderTargetView RenderTargetView
        {
            get { return renderTargetView; }
        }

        /// <summary>
        ///     Gets the depth target view.
        /// </summary>
        public DepthStencilView DepthTargetView
        {
            get { return depthTargetView; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets or sets the matrix manager.
        /// </summary>
        /// <value>
        ///     The matrix manager.
        /// </value>
        public MatrixManager MatrixManager { get; set; }

        /// <summary>
        ///     カメラモーションの挙動を設定するプロパティ
        /// </summary>
        public ICameraMotionProvider CameraMotionProvider { get; set; }

        /// <summary>
        ///     このスクリーンに結び付けられているワールド空間
        /// </summary>
        public WorldSpace WorldSpace { get; set; }

        /// <summary>
        ///     MME特殊パラメータ
        ///     object_ssの時に取得するセルフシャドウモード
        ///     bool parthfにあたる
        /// </summary>
        public bool IsSelfShadowMode1 { get; private set; }

        /// <summary>
        ///     MME特殊パラメータ
        ///     bool transpにあたる値
        /// </summary>
        public bool IsEnabledTransparent { get; private set; }

        public void SetViewport()
        {
            Context.DeviceManager.Context.Rasterizer.SetViewports(getViewport());
        }

        /// <summary>
        ///     レンダーターゲットをリセットします．
        /// </summary>
        private void ResetTargets()
        {
            // ターゲットを破棄
            if (RenderTargetView != null && !RenderTargetView.Disposed)
                RenderTargetView.Dispose();
            if (RenderTarget != null && !RenderTarget.Disposed)
                RenderTarget.Dispose();
            if (DepthTargetView != null && !DepthTargetView.Disposed)
                DepthTargetView.Dispose();
            if (DepthTarget != null && !DepthTarget.Disposed)
                DepthTarget.Dispose();

            Device device = Context.DeviceManager.Device;
            //深度ステンシルバッファの初期化
            //レンダーターゲットの初期化
            renderTarget = new Texture2D(device, getRenderTargetTexture2DDescription());
            renderTargetView = new RenderTargetView(device, RenderTarget);
            //深度ステンシルバッファの初期化
            depthTarget = new Texture2D(device, getDepthBufferTexture2DDescription());
            depthTargetView = new DepthStencilView(device, DepthTarget);

            SetViewport();
        }

        ~TextureTargetContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context = null;
                }

                if (WorldSpace != null)
                {
                    WorldSpace.Dispose();
                    WorldSpace = null;
                }

                if (renderTargetView != null && !renderTargetView.Disposed)
                    renderTargetView.Dispose();
                if (renderTarget != null && !renderTarget.Disposed)
                    renderTarget.Dispose();
                if (depthTargetView != null && !depthTargetView.Disposed)
                    depthTargetView.Dispose();
                if (depthTarget != null && !depthTarget.Disposed)
                    depthTarget.Dispose();

                disposed = true;
            }
        }

        ///// <summary>
        ///// レンダーターゲットの設定を取得します．
        ///// </summary>
        protected virtual Texture2DDescription getRenderTargetTexture2DDescription()
        {
            return new Texture2DDescription
            {
                Width = Size.Width,
                Height = Size.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = SampleDesc,
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
        }

        /// <summary>
        ///     深度ステンシルバッファの設定を取得します。
        /// </summary>
        protected virtual Texture2DDescription getDepthBufferTexture2DDescription()
        {
            return new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float,
                Width = Size.Width,
                Height = Size.Height,
                MipLevels = 1,
                SampleDescription = SampleDesc
            };
        }

        public void MoveCameraByCameraMotionProvider()
        {
            if (CameraMotionProvider == null)
                return;

            CameraMotionProvider.UpdateCamera(MatrixManager.ViewMatrixManager, MatrixManager.ProjectionMatrixManager);
        }

        /// <summary>
        ///     ビューポートの内容を取得します。
        /// </summary>
        /// <returns>設定するビューポート</returns>
        protected virtual Viewport getViewport()
        {
            return new Viewport
            {
                Width = Size.Width,
                Height = Size.Height,
                MaxZ = 1
            };
        }

        /// <summary>
        ///     WorldSpaceの内容を描画します．
        /// </summary>
        public void Render()
        {
            Context.SetRenderScreen(this);
            MoveCameraByCameraMotionProvider();
            Context.ClearScreenTarget(BackgroundColor);
            WorldSpace.DrawAllResources(HitChecker);
        }
    }
}