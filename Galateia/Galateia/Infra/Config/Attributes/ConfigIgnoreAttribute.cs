using System;

namespace Galateia.Infra.Config.Attributes
{
    /// <summary>
    ///     設定できないプロパティとして指定します．
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigIgnoreAttribute : Attribute
    {
    }
}