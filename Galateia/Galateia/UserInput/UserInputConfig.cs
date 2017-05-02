using System.Windows.Data;
using System.Windows.Forms;
using Galateia.Infra.Config;
using Galateia.Infra.Config.Attributes;
using Galateia.Infra.Config.Controls;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Galateia.UserInput
{
    [ConfigurableObject(Title = "ユーザー入力設定")]
    public class UserInputConfig : ConfigBase
    {
        public UserInputConfig()
        {
            InputVirtualKeyCode = Keys.Insert;
            InterceptKey = true;
        }

        /// <summary>
        ///     インプットを行うショートカットキーの仮想キーコード．
        /// </summary>
        [Configurable(typeof (EnumComboBox<Keys>), "SelectedItem", Label = "短絡キー",
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Width", 150.0)]
        public Keys InputVirtualKeyCode { get; set; }

        /// <summary>
        ///     ショートカットキーの処理を中断させるかどうかの値を取得または設定します．
        /// </summary>
        [Configurable(typeof (CheckBox), "IsChecked", UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
        [Setter("Content", "短絡キーを他のアプリケーションに処理させない")]
        public bool InterceptKey { get; set; }
    }
}