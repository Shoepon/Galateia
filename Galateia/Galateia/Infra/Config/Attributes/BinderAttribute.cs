using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Galateia.Infra.Config.Attributes
{
    /// <summary>
    ///     設定値がバインドされるコントロールのプロパティを，この属性を指定するプロパティを所有するクラスの他のプロパティとバインドします．
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BinderAttribute : Attribute
    {
        private Type converterType;
        private Type validationRuleType;

        /// <summary>
        ///     バインド元のプロパティ名と，バインド先のプロパティを指定して新しいインスタンスを初期化します．
        /// </summary>
        /// <param name="srcPropertyName">バインド元のプロパティ名</param>
        /// <param name="dstPropertyName">バインド先のコントロールのプロパティ</param>
        public BinderAttribute(string srcPropertyName, string dstPropertyName)
        {
            Mode = BindingMode.Default;
            UpdateSourceTrigger = UpdateSourceTrigger.Default;
            StringFormat = null;
            this.SrcPropertyName = srcPropertyName;
            this.DstPropertyName = dstPropertyName;
        }

        /// <summary>
        ///     コントロールにバインドする元のプロパティ名を指定します
        /// </summary>
        public string SrcPropertyName { get; private set; }

        /// <summary>
        ///     値を関連付けるコントロールのプロパティを取得します．
        ///     プロパティは依存関係プロパティとして登録されている必要があります．
        /// </summary>
        public string DstPropertyName { get; private set; }

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