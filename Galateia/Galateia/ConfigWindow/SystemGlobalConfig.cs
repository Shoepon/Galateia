using System;
using System.Windows.Controls;
using System.Windows.Data;
using Galateia.Infra.Config;
using Galateia.Infra.Config.Attributes;
using Galateia.Infra.Config.ValidationRules;

namespace Galateia.ConfigWindow
{
    [ConfigurableObject(Title = "システム設定")]
    public class SystemGlobalConfig : ConfigBase
    {
        public SystemGlobalConfig()
        {
            ClientId = Guid.NewGuid();
            FpsLimit = 30;
        }

        [Configurable(typeof (TextBox), "Text", Label = "個人識別GUID番号",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("MinWidth", 100.0)]
        public Guid ClientId { get; set; }

        [Configurable(typeof(TextBox), "Text", Label = "FPS制限 (0:無制限)",
            ValidationRuleType = typeof(NonNegativeIntValidationRule),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Width", 100.0)]
        public int FpsLimit { get; set; }
    }
}