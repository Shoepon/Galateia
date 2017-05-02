using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Galateia.Infra.Config;
using Galateia.Infra.Config.Attributes;

namespace Galateia.ConfigWindow
{
    /// <summary>
    ///     GlobalConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class GlobalConfigWindow : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ConfigurationsProperty = DependencyProperty.Register(
            "Configurations",
            typeof (GlobalConfig),
            typeof (GlobalConfigWindow),
            new PropertyMetadata(ConfigurationsProperty_PropertyChangedCallback)
            );

        /// <summary>
        ///     変更を適用するコマンド
        /// </summary>
        public static readonly RoutedCommand ApplyCommand = new RoutedCommand();

        /// <summary>
        ///     変更前のコンフィグと，編集用の一時コンフィグ
        /// </summary>
        private readonly GlobalConfig _origConf;

        /// <summary>
        ///     変更前のコンフィグと，編集用の一時コンフィグ
        /// </summary>
        private readonly GlobalConfig _tempConf;

        public GlobalConfigWindow(GlobalConfig config)
        {
            // 設定のディープコピーを作成し，編集用として設定する
            _origConf = config;
            _tempConf = new GlobalConfig(config);
            Configurations = _tempConf;

            InitializeComponent();
            foreach (TabItem item in GenerateTabItems(new Thickness(5)))
                tabControl.Items.Add(item);
        }

        public GlobalConfig Configurations
        {
            get { return (GlobalConfig) GetValue(ConfigurationsProperty); }
            set { SetValue(ConfigurationsProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void ConfigurationsProperty_PropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((GlobalConfigWindow) d).RaisePropertyChanged("Configurations");
        }

        /// <summary>
        ///     Config.ConfigurableObjectAttribute 属性のついたデータ型をプロパティに持つオブジェクトの型から，各プロパティに対応するTabItemを取得します．
        ///     TabControlにメソッドの戻り値を追加し，TabControlのDataContextに<paramref name="type" />型オブジェクトのインスタンスを設定すると，値を編集できるようになります．
        /// </summary>
        /// <param name="defaultMargin">コンポーネントの既定のマージン</param>
        /// <returns>属性にしたがって<code>Header</code>と<code>Content</code>が設定されたTabItem</returns>
        private static IEnumerable<TabItem> GenerateTabItems(Thickness defaultMargin)
        {
            // コントロールのインスタンス化
            foreach (PropertyInfo propertyInfo in typeof (GlobalConfig).GetProperties())
            {
                object[] configurables =
                    propertyInfo.PropertyType.GetCustomAttributes(typeof (ConfigurableObjectAttribute), true);
                if (configurables.Length > 0)
                {
                    // Configurableとしてマークされている
                    var item = new TabItem {Header = ((ConfigurableObjectAttribute) configurables[0]).Title};
                    UserControl content = ConfigTools.GenerateUserControl(propertyInfo.PropertyType, defaultMargin);
                    content.Margin = defaultMargin;
                    item.Content = content;

                    // Binding の作成
                    var binding = new Binding(propertyInfo.Name)
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    content.SetBinding(DataContextProperty, binding);

                    yield return item;
                }
            }
        }

        /// <summary>
        ///     OK ボタンが押されたときの処理：変更を適用してウィンドウを閉じる
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyCommand.Execute(null, this);
            Close();
        }

        /// <summary>
        ///     Cancel ボタンが押されたときの処理：何もせずにウィンドウを閉じる
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExecutedApplyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RaiseBeforeSubstitute();
            _origConf.Substitute(_tempConf);
            RaiseAfterSubstitute();
        }

        private void CanExecutedApplyCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !_origConf.Equals(_tempConf);
        }

        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler pc = PropertyChanged;
            if (pc != null)
                pc(this, new PropertyChangedEventArgs(name));
        }

        public event EventHandler BeforeSubstitute;
        public event EventHandler AfterSubstitute;

        protected void RaiseBeforeSubstitute()
        {
            var handler = BeforeSubstitute;
            if (handler != null)
                handler(this, new EventArgs());
        }

        protected void RaiseAfterSubstitute()
        {
            var handler = AfterSubstitute;
            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}