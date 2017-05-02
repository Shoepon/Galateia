using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMF.DeviceManager;
using MMF.Model.PMX;

namespace Galateia.CSC
{
    /// <summary>
    /// シェルの挙動を司る部分のインタフェースです．
    /// </summary>
    public interface IDemeanor
    {
        /// <summary>
        /// システムのロードが完了したときに呼び出されます．
        /// </summary>
        /// <param name="shell">シェルのインタフェースを指定します．</param>
        void Loaded(IShell shell);

        /// <summary>
        /// システムが終了しようとする際に呼び出されます．
        /// </summary>
        void Disposing();

        /// <summary>
        /// 動作に関するコマンドを受け付けます．
        /// </summary>
        /// <param name="command">入力された文字列を指定します．</param>
        void Command(string command);

        /// <summary>
        /// レンダリング作業の直前に呼び出されます．
        /// </summary>
        /// <param name="rectangles">描画するX-Y平面上の矩形を指定します．</param>
        void PreRender(out RectangleF rectangles);

        /// <summary>
        /// バルーンを表示する際の基準点のワールド座標を取得します．
        /// </summary>
        PointF BaloonReferencePoint { get; }
    }
}
