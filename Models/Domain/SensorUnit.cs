using MvvmHelpers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace GrayWolf.Models.Domain
{
    public class SensorUnit : ObservableObject
    {
        private int _code;
        public int Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _value;
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private string _decimals;
        public string Decimals
        {
            get => _decimals;
            set => SetProperty(ref _decimals, value);
        }

        private NegativeValuesHandleStrategy _negativeValuesHandleStrategy;
        public NegativeValuesHandleStrategy NegativeValuesHandleStrategy
        {
            get => _negativeValuesHandleStrategy;
            set => SetProperty(ref _negativeValuesHandleStrategy, value);
        }

        public string FormattedValue => GetFormattedValue();

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Value):
                case nameof(Decimals):
                    OnPropertyChanged(nameof(FormattedValue));
                    break;
            }
        }

        //private string GetFormattedValue()
        //{
        //    double nancheck;

        //    // ALWAYS parse sensor raw value using invariant culture
        //    if (double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out nancheck))
        //    {
        //        // suppress NON NUMBERS
        //        if (nancheck < Services.SensorsService.NUMBER_NONNUMBER_CUTOFF)
        //            return "--";
        //    }

        //    // CO2 handling
        //    if (NegativeValuesHandleStrategy == NegativeValuesHandleStrategy.ClampCO2 &&
        //        double.Parse(Value, CultureInfo.InvariantCulture) < GrayWolf.Helpers.Constants.CO2_CUTOFF)
        //    {
        //        Value = GrayWolf.Helpers.Constants.CO2_CUTOFF.ToString("N0", CultureInfo.InvariantCulture);
        //    }

        //    // Negative values
        //    if (NegativeValuesHandleStrategy == NegativeValuesHandleStrategy.ClampToZero &&
        //        double.Parse(Value, CultureInfo.InvariantCulture) < 0)
        //    {
        //        Value = "0";
        //    }

        //    var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        //    decimal parsed;

        //    decimal.TryParse(Value, style, CultureInfo.InvariantCulture, out parsed);

        //    return parsed.ToString($"F{Decimals}", CultureInfo.InvariantCulture);
        //}


     

private string GetFormattedValue()
    {
        try
        {
            // 0. No value -> nothing to show
            if (string.IsNullOrWhiteSpace(Value))
                return string.Empty;

            // IMPORTANT:
            // - Do NOT use AllowThousands here, otherwise "3,3" in Spanish can become 33.
            const NumberStyles style = NumberStyles.Float;

            // 1) Normalize input (Arabic-Indic digits, Arabic decimal separator, etc.)
            var raw = Value.Trim();
            var normalized = NormalizeDigitsAndSeparators(raw);

            // 2) Handle decimal-comma values safely (Spanish etc.)
            // If we have a comma and no dot, treat comma as decimal separator.
            // e.g. "3,3" -> "3.3"
            if (normalized.Contains(',') && !normalized.Contains('.'))
            {
                normalized = normalized.Replace(',', '.');
            }

            // Optional: remove spaces (some locales use non-breaking spaces)
            normalized = normalized.Replace("\u00A0", "").Replace(" ", "");

            // 3) Parse using InvariantCulture (device-safe)
            if (!double.TryParse(normalized, style, CultureInfo.InvariantCulture, out var numeric))
            {
                Debug.WriteLine(
                    $"[SensorUnit] Could not parse Value='{Value}' normalized='{normalized}'. " +
                    $"CurrentCulture='{CultureInfo.CurrentCulture.Name}', UICulture='{CultureInfo.CurrentUICulture.Name}'");
                return "--";
            }

            // 4) Suppress NON NUMBERS based on your existing cutoff
            if (numeric < Services.SensorsService.NUMBER_NONNUMBER_CUTOFF)
            {
                return "--";
            }

            // 5) Apply negative / CO2 clamping on numeric
            switch (NegativeValuesHandleStrategy)
            {
                case NegativeValuesHandleStrategy.ClampCO2:
                    if (numeric < GrayWolf.Helpers.Constants.CO2_CUTOFF)
                        numeric = GrayWolf.Helpers.Constants.CO2_CUTOFF;
                    break;

                case NegativeValuesHandleStrategy.ClampToZero:
                    if (numeric < 0)
                        numeric = 0;
                    break;

                case NegativeValuesHandleStrategy.ShowNegativeValues:
                default:
                    // leave as-is
                    break;
            }

            // 6) Safely parse decimals count
            int decimalsCount;
            if (!int.TryParse(Decimals, NumberStyles.Integer, CultureInfo.InvariantCulture, out decimalsCount))
                decimalsCount = 0;

            // Keep decimalsCount in a sane range (optional safety)
            if (decimalsCount < 0) decimalsCount = 0;
            if (decimalsCount > 6) decimalsCount = 6;

                // 7) Format final number
                // If you want output to match the selected language (Spanish uses comma), use CurrentCulture.
                // If you want output always with dot, keep InvariantCulture.
                var result = numeric.ToString($"F{decimalsCount}", CultureInfo.InvariantCulture);

                Debug.WriteLine(
                    $"[FormattedValue] Reading? SensorUnitHash={GetHashCode()} " +
                    $"Culture={CultureInfo.CurrentCulture.Name} " +
                    $"ValueRaw='{Value}' numeric={numeric} decimals={decimalsCount} " +
                    $"RETURN='{result}'");

                return result;
            }
        catch (Exception ex)
        {
            // FINAL SAFETY NET: never let this method throw to the UI binding
            Debug.WriteLine(
                $"[SensorUnit] GetFormattedValue ERROR. " +
                $"Value='{Value}', Decimals='{Decimals}', " +
                $"Strategy='{NegativeValuesHandleStrategy}', " +
                $"CurrentCulture='{CultureInfo.CurrentCulture.Name}', " +
                $"UICulture='{CultureInfo.CurrentUICulture.Name}', " +
                $"Exception={ex}");

            return "--";
        }
    }

    private static string NormalizeDigitsAndSeparators(string input)
    {
        // Converts Arabic-Indic digits (٣٫٣) to Latin digits (3.3)
        // Normalizes Arabic decimal separator '٫' to '.'
        // Also normalizes Arabic comma '،' to ','
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new StringBuilder(input.Length);

        foreach (var ch in input)
        {
            // Arabic decimal separator
            if (ch == '٫')
            {
                sb.Append('.');
                continue;
            }

            // Arabic comma
            if (ch == '،')
            {
                sb.Append(',');
                continue;
            }

            // Convert Arabic-Indic digits and other unicode digits to ASCII digits
            // char.GetNumericValue returns 0-9 for many unicode digit characters.
            var nv = char.GetNumericValue(ch);
            if (nv >= 0 && nv <= 9 && !char.IsDigit(ch))
            {
                sb.Append((int)nv);
                continue;
            }

            sb.Append(ch);
        }

        return sb.ToString();
    }

}

public enum NegativeValuesHandleStrategy
    {
        ClampToZero,
        ShowNegativeValues,
        ClampCO2
    }
}
