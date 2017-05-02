using System;
using System.Windows.Controls;

namespace Galateia.Infra.Config.Controls
{
    /// <summary>
    ///     SelectedItemに値をバインドしてください．
    /// </summary>
    public class EnumComboBox<T> : ComboBox
    {
        public EnumComboBox()
        {
            if (!typeof (T).IsSubclassOf(typeof (Enum)))
                throw new ArgumentException("T is not subclass of enum.");

            base.ItemsSource = Enum.GetValues(typeof (T));
            base.IsEditable = false;
        }
    }
}