using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Galateia.Infra.Config.Attributes
{
    /// <summary>
    ///     ユーザーが設定可能なプロパティとして指定します．
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigurableAttribute : Attribute
    {
        private Type converterType;
        private Type validationRuleType;

        /// <summary>
        ///     設定コントロールの生成に必要な情報を指定してインスタンスを初期化します．
        /// </summary>
        /// <param name="label">設定のラベル</param>
        /// <param name="controlType">値を関連付けるコントロールの型</param>
        /// <param name="propertyName">値を関連付けるコントロールのプロパティ</param>
        public ConfigurableAttribute(Type controlType, string propertyName)
        {
            Mode = BindingMode.Default;
            UpdateSourceTrigger = UpdateSourceTrigger.Default;
            Span = 1;
            StringFormat = null;
            Label = null;
            Group = null;
            if (!controlType.IsSubclassOf(typeof (FrameworkElement)))
                throw new ArgumentException(@"'controlType' is not a subclass of System.Windows.FrameworkElement.",
                    "controlType");
            this.ControlType = controlType;

            this.PropertyName = propertyName;
        }

        /// <summary>
        ///     属性を付与した対象のオブジェクトに対して，設定コントロールの自動生成を試みます．
        ///     この属性を付与する対象のクラスは，ConfigurableObject属性を持っている必要があります．
        /// </summary>
        public ConfigurableAttribute()
        {
            Mode = BindingMode.Default;
            UpdateSourceTrigger = UpdateSourceTrigger.Default;
            Span = 1;
            StringFormat = null;
            Label = null;
            Group = null;
            ControlType = null;
            PropertyName = "DataContext";
        }

        /// <summary>
        ///     設定の所属するグループの名前を取得または設定します．
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     設定のラベルを取得または設定します．
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     この要素がレイアウト グリッド上で占める列数を取得または設定します．
        /// </summary>
        public int Span { get; set; }

        /// <summary>
        ///     値を関連付けるコントロールの型を取得します．
        /// </summary>
        public Type ControlType { get; private set; }

        /// <summary>
        ///     値を関連付けるコントロールのプロパティ名を取得します．
        ///     プロパティは依存関係プロパティとして登録されている必要があります．
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        ///     ユーザー入力の有効性を確認する規則を取得または設定します．
        /// </summary>
        public Type ValidationRuleType
        {
            get { return validationRuleType; }
            set
            {
                if (!value.IsSubclassOf(typeof (ValidationRule)))
                    throw new ArgumentException("value is not a subclass of System.Windows.Controls.ValidationRule.");
                validationRuleType = value;
            }
        }

        /// <summary>
        ///     バインドされている値が文字列として表示される場合に，バインディングの書式を指定する文字列を取得または設定します．
        /// </summary>
        public string StringFormat { get; set; }

        /// <summary>
        ///     バインディング ソースを更新するタイミングを決定する値を取得または設定します．
        /// </summary>
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        /// <summary>
        ///     使用するコンバーターを取得または設定します．
        /// </summary>
        public Type ConverterType
        {
            get { return converterType; }
            set
            {
                if (!value.GetInterfaces().Contains(typeof (IValueConverter)))
                    throw new ArgumentException("value is not a subclass of System.Windows.Data.IValueConverter.");
                converterType = value;
            }
        }

        /// <summary>
        ///     バインディングモードを取得または設定します．
        /// </summary>
        public BindingMode Mode { get; set; }
    }
}