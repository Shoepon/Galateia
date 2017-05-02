using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using SlimDX;
using D2D = SlimDX.Direct2D;
using DW = SlimDX.DirectWrite;

namespace Galateia.Infra.Graphics
{
    public class DWOutlinedText : IDisposable
    {
        private readonly D2D.Factory _d2DFactory;
        private readonly DW.Factory _dwFactory;
        private readonly int _fontFaceIndex;
        private readonly DW.FontFaceType _fontFaceType;
        private readonly DW.FontFile _fontFile;
        private bool _disposed;
        private DW.FontFace _fontFace;

        private DW.FontSimulations fontSimulations;

        public DWOutlinedText(D2D.Factory d2dFactory, DW.Factory dwFactory, string fontfile, int fontFaceIndex,
            DW.FontFaceType fontFaceType)
        {
            _d2DFactory = d2dFactory;
            _dwFactory = dwFactory;
            _fontFile = _dwFactory.CreateFontFileReference(fontfile);
            _fontFaceIndex = fontFaceIndex;
            _fontFaceType = fontFaceType;
        }

        /// <summary>
        ///     フォントフェイスに適用する，アルゴリズムによるスタイルシミュレーションを指定します
        /// </summary>
        public DW.FontSimulations FontSimulations
        {
            get { return fontSimulations; }
            set
            {
                fontSimulations = value;
                DisposeFontFace();
            }
        }

        private DW.FontFace FontFace
        {
            get { return _fontFace ?? UpdateFontFace(); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DWOutlinedText()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _fontFile.Dispose();

            _disposed = true;
        }

        private DW.FontFace UpdateFontFace()
        {
            _fontFace = _dwFactory.CreateFontFace(_fontFaceType, new[] {_fontFile}, _fontFaceIndex, FontSimulations);
            return _fontFace;
        }

        private void DisposeFontFace()
        {
            if (_fontFace == null) return;
            _fontFace.Dispose();
            _fontFace = null;
        }

        public unsafe D2D.Geometry CreateGeometry(float fontSize, string text, float[] glyphAdvances = null,
            DW.GlyphOffset[] glyphOffsets = null)
        {
            // USC4のコードポイント配列を取得
            byte[] utf32 = Encoding.UTF32.GetBytes(text);
            var cps = new int[text.Length];
            fixed (int* p = cps)
                Marshal.Copy(utf32, 0, (IntPtr) p, utf32.Length);

            // GlyphIndexの配列に変換
            short[] indices = FontFace.GetGlyphIndicesW(cps);

            // ジオメトリの作成
            var geometry = new D2D.PathGeometry(_d2DFactory);
            using (D2D.GeometrySink sink = geometry.Open())
            {
                _fontFace.GetGlyphRunOutline(fontSize, indices, glyphAdvances, glyphOffsets, false, false, sink);
                sink.Close();
            }

            return geometry;
        }

        public void Draw(D2D.RenderTarget target, float fontSize, string text, PointF point, DW.TextAlignment alignment,
            D2D.SolidColorBrush fillBrush, D2D.SolidColorBrush edgeBrush, float edgeWidth)
        {
            D2D.Geometry geometry = CreateGeometry(fontSize, text);

            switch (alignment)
            {
                case DW.TextAlignment.Leading:
                    target.Transform = Matrix3x2.Translation(point);
                    break;
                case DW.TextAlignment.Center:
                {
                    RectangleF bounds = D2D.Geometry.GetBounds(geometry);
                    target.Transform = Matrix3x2.Translation(point.X - bounds.Width/2f, point.Y);
                }
                    break;
                case DW.TextAlignment.Trailing:
                {
                    RectangleF bounds = D2D.Geometry.GetBounds(geometry);
                    target.Transform = Matrix3x2.Translation(point.X - bounds.Width, point.Y);
                }
                    break;
            }

            target.DrawGeometry(geometry, edgeBrush, edgeWidth);
            target.FillGeometry(geometry, fillBrush);
        }
    }
}