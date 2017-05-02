using System;
using D2D = SlimDX.Direct2D;

namespace Galateia.Infra.Graphics
{
    /// <summary>
    ///     Direct2DのGDI互換レンダーターゲットからデバイスコンテキストを取得します．
    /// </summary>
    public class RenderTargetDC : IDisposable
    {
        private readonly IntPtr hdc;
        private bool disposed;
        private D2D.GdiInteropRenderTarget renderTarget;

        public RenderTargetDC(D2D.GdiInteropRenderTarget renderTarget)
        {
            this.renderTarget = renderTarget;
            hdc = this.renderTarget.GetDC(D2D.DeviceContextInitializeMode.Copy);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RenderTargetDC()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                renderTarget.ReleaseDC();

                if (disposing)
                    renderTarget = null;

                disposed = true;
            }
        }

        public static implicit operator IntPtr(RenderTargetDC rtdc)
        {
            return rtdc.hdc;
        }
    }
}