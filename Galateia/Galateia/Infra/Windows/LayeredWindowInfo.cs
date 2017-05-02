using System;
using Galateia.Infra.WindowsAPI;

namespace Galateia.Infra.Windows
{
    /// <summary>
    ///     Layered Window の更新を行います．
    /// </summary>
    public class LayeredWindowInfo
    {
        private readonly COLORREF crKey;
        private readonly IntPtr hWnd;
        private BLENDFUNCTION blend;
        private POINT ptDst, ptSrc;
        private SIZE size;

        public LayeredWindowInfo(IntPtr hWnd, int x, int y, int width, int height)
        {
            this.hWnd = hWnd;

            crKey = new COLORREF(0, 0, 0, 0);
            blend.BlendOp = 0; // AC_SRC_OVER
            blend.BlendFlags = 0;
            blend.SourceConstantAlpha = 255;
            blend.AlphaFormat = 1; // AC_SRC_ALPHA
            ptDst = new POINT(x, y);
            ptSrc = new POINT(0, 0);
            size = new SIZE(width, height);
        }

        public POINT Position
        {
            get { return ptDst; }
            set { ptDst = value; }
        }

        public SIZE Size
        {
            get { return size; }
            set { size = value; }
        }

        public byte Opacity
        {
            get { return blend.SourceConstantAlpha; }
            set { blend.SourceConstantAlpha = value; }
        }

        public void Update(IntPtr hdcSource)
        {
            User.UpdateLayeredWindow(hWnd, IntPtr.Zero, ref ptDst, ref size, hdcSource, ref ptSrc, crKey, ref blend,
                UpdateLayeredWindowFlags.Alpha);
        }
    }
}