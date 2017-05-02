using System.Windows.Controls;
using System.Windows.Data;
using Galateia.Infra.Config;
using Galateia.Infra.Config.Attributes;
using Galateia.Infra.Config.ValidationRules;

namespace Galateia.Baloon
{
    [ConfigurableObject(Title = "バルーン設定")]
    public class BaloonGlobalConfig : ConfigBase
    {
        public BaloonGlobalConfig()
        {
            TimeoutInSecond = 5.0;
        }

        [Configurable(typeof (TextBox), "Text", Label = "タイムアウト時間（秒）",
            ValidationRuleType = typeof (NonNegativeDoubleValidationRule),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Width", 100.0)]
        public double TimeoutInSecond { get; set; }
    }
}