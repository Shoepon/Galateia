using System.Globalization;
using System.Windows.Controls;

namespace Galateia.Infra.Config.ValidationRules
{
    public class PositiveIntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var sz = value as string;
            if (sz == null) return new ValidationResult(false, "不明なエラーです");
            int i;
            if (!int.TryParse(sz, out i)) return new ValidationResult(false, "整数値を入力して下さい");
            return i > 0 ? new ValidationResult(true, null) : new ValidationResult(false, "正の数値を入力して下さい");
        }
    }
}