using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Galateia.Infra.Config.Controls
{
    /// <summary>
    ///     このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///     手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    ///     この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    ///     追加します:
    ///     xmlns:MyNamespace="clr-namespace:MMSA.Common.Config"
    ///     手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    ///     この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    ///     追加します:
    ///     xmlns:MyNamespace="clr-namespace:MMSA.Common.Config;assembly=MMSA.Common.Config"
    ///     また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    ///     リビルドして、コンパイル エラーを防ぐ必要があります:
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///     手順 2)
    ///     コントロールを XAML ファイルで使用します。
    ///     <MyNamespace:ListStyleTabControl />
    /// </summary>
    public class VerticalTabControl : TabControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty HeaderWidthProperty = DependencyProperty.Register(
            "HeaderWidth",
            typeof (GridLength),
            typeof (VerticalTabControl),
            new PropertyMetadata(HeaderWidth_PropertyChangedCallback)
            );

        public static readonly DependencyProperty BottomOfContentToolPanelProperty = DependencyProperty.Register(
            "BottomOfContentToolPanel",
            typeof (Panel),
            typeof (VerticalTabControl),
            new PropertyMetadata(BottomOfContentToolPanel_PropertyChangedCallback)
            );

        static VerticalTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (VerticalTabControl),
                new FrameworkPropertyMetadata(typeof (VerticalTabControl)));
        }

        public GridLength HeaderWidth
        {
            get { return (GridLength) GetValue(HeaderWidthProperty); }
            set { SetValue(HeaderWidthProperty, value); }
        }

        public Panel BottomOfContentToolPanel
        {
            get { return (Panel) GetValue(BottomOfContentToolPanelProperty); }
            set { SetValue(BottomOfContentToolPanelProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void HeaderWidth_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VerticalTabControl) d).RaisePropertyChanged("HeaderWidth");
        }

        private static void BottomOfContentToolPanel_PropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((VerticalTabControl) d).RaisePropertyChanged("BottomOfContentToolPanel");
        }

        protected void RaisePropertyChanged(string name)
        {
            var pc = PropertyChanged;
            if (pc != null)
                pc(this, new PropertyChangedEventArgs(name));
        }
    }
}