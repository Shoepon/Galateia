using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Galateia.Infra.Config.ValidationRules
{
    public class NonNegativeDoubleValidationRule : ValidationRule
    {
        private static readonly Regex regex = new Regex(@"\.0*$");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var sz = value as string;
            if (sz == null) return new ValidationResult(false, "不明なエラーです");
            double d;
            if (regex.IsMatch(sz) || !double.TryParse(sz, out d)) return new ValidationResult(false, "浮動小数点値を入力して下さい");
            return d < 0.0 ? new ValidationResult(false, "非負の数値を入力して下さい") : new ValidationResult(true, null);
        }
    }
}