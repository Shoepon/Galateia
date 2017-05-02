using System;

namespace Galateia.Infra.Config.Attributes
{
    /// <summary>
    ///     設定可能なオブジェクトとして指定します．
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ConfigurableObjectAttribute : Attribute
    {
        public ConfigurableObjectAttribute()
        {
            NumberOfColumns = 1;
            Title = null;
        }

        /// <summary>
        ///     カラム数を取得または設定します．
        /// </summary>
        public int NumberOfColumns { get; set; }

        /// <summary>
        ///     タブの名前として使用されるタイトルを取得または設定します．
        ///     UserControlの生成には使用されません．
        /// </summary>
        public string Title { get; set; }
    }
}