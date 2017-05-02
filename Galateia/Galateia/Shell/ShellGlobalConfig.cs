using System.Windows.Controls;
using System.Windows.Data;
using Galateia.Infra.Config;
using Galateia.Infra.Config.Attributes;
using Galateia.Infra.Config.ValidationRules;

namespace Galateia.Shell
{
    [ConfigurableObject(Title = "シェル表示設定")]
    public class ShellGlobalConfig : ConfigBase
    {
        public ShellGlobalConfig()
        {
            PixelPerUnitLength = 15f;
            MultiSampleCount = 8;
            MultiSampleQuality = 10;
            PerviousMouse = false;
            Opacity = 255;
        }

        [Configurable(typeof (TextBox), "Text", Label = "単位ワールド長さあたりのピクセル数",
            ValidationRuleType = typeof (NonNegativeDoubleValidationRule),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Width", 100.0)]
        public float PixelPerUnitLength { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "マルチサンプル数",
            ValidationRuleType = typeof (PositiveIntValidationRule),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Width", 100.0)]
        public int MultiSampleCount { get; set; }

        [Configurable(typeof (TextBox), "Text", Label = "マルチサンプル品質",
            ValidationRuleType = typeof (NonNegativeIntValidationRule),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Width", 100.0)]
        public int MultiSampleQuality { get; set; }

        [Configurable(typeof(CheckBox), "IsChecked", UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Content", "マウスを透過させる")]
        public bool PerviousMouse { get; set; }

        [Configurable(typeof(TextBox), "Text", Label = "透過率",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Width", 100.0)]
        public byte Opacity { get; set; }
    }
}