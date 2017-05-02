using System;

namespace Galateia.Infra.Config.Attributes
{
    /// <summary>
    ///     設定値がバインドされるコントロールのプロパティに値を設定します．
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SetterAttribute : Attribute
    {
        /// <summary>
        ///     プロパティと値を指定して新しいインスタンスを初期化します．
        /// </summary>
        /// <param name="propertyName">値を設定するプロパティ</param>
        /// <param name="value">設定する値</param>
        public SetterAttribute(string propertyName, object value)
        {
            this.PropertyName = propertyName;
            this.Value = value;
        }

        /// <summary>
        ///     値を関連付けるコントロールのプロパティを取得します．
        ///     プロパティは依存関係プロパティとして登録されている必要があります．
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        ///     設定する値を取得します．
        /// </summary>
        public object Value { get; private set; }
    }
}