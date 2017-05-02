using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Galateia.Infra.Config.Attributes;

namespace Galateia.Baloon
{
    /// <summary>
    ///     バルーンの背景デザインの設定です．
    /// </summary>
    [ConfigurableObject(NumberOfColumns = 1)]
    public class BaloonDesign
    {
        [Configurable(typeof (TextBox), "Text", Label = "ヘッダ画像",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public string HeaderImage { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "フッタ画像",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public string FooterImage { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "左側画像",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public string LeftSideImage { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "右側画像",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public string RightSideImage { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "背景ブラシ画像",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public string BodyBrushImage { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "テキスト表示部マージン",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public Thickness ViewerMargin { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "バルーンの端から頭ボーンまでのオフセット（ワールド座標）",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public float HorizontalWorldOffset { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "Bottomラインから頭ボーンまでのオフセット（スクリーン座標）",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public float VerticalScreenOffset { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "フォントファミリ",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public string FontFamily { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "フォントサイズ",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        public double FontSize { get; set; }
    }
}