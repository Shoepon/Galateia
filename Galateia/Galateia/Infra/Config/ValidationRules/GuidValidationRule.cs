using System;
using System.Globalization;
using System.Windows.Controls;

namespace Galateia.Infra.Config.ValidationRules
{
    public class GuidValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var sz = value as string;
            if (sz == null) return new ValidationResult(false, "不明なエラーです");
            Guid id;
            return Guid.TryParse(sz, out id) ? new ValidationResult(true, null) : new ValidationResult(false, "GUIDを入力して下さい");
        }
    }
}