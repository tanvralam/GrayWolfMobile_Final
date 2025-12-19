using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using GrayWolf.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace GrayWolf.Converters
{
    public class ReadingFromBleJsonSensorConverter : JsonConverter<Reading>
    {
        public override Reading ReadJson(JsonReader reader, Type objectType, Reading existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            var unitCode = token.SelectToken(nameof(BLEJsonSensor.unitCode)).Value<int>();
            var valueToken = token.SelectToken(nameof(BLEJsonSensor.value));

            // Always store sensor numeric value in invariant format ("3.9"), regardless of UI culture
            var invariantValue = GetInvariantValueString(valueToken);

            var originalUnit = new Models.Domain.SensorUnit
            {
                Code = unitCode,
                Value = invariantValue,
            };

            // IMPORTANT: don't point ConvertedUnit to the same instance
            // (same reference can cause accidental side-effects later)
            var convertedUnit = new Models.Domain.SensorUnit
            {
                Code = unitCode,
                Value = invariantValue,
            };

            return new Reading
            {
                Channel = token.SelectToken(nameof(BLEJsonSensor.channel)).Value<int>(),
                TimeStamp = token.SelectToken(nameof(BLEJsonSensor.timeStamp)).Value<DateTime>(),
                SensorId = token.SelectToken(nameof(BLEJsonSensor.id)).Value<int>(),
                OriginalUnit = originalUnit,
                ConvertedUnit = convertedUnit,
                SensorCode = (SensorType)token.SelectToken(nameof(BLEJsonSensor.code)).Value<int>(),
            };
        }

        public override void WriteJson(JsonWriter writer, Reading value, JsonSerializer serializer)
            => throw new NotImplementedException();

        private static string GetInvariantValueString(JToken valueToken)
        {
            if (valueToken == null || valueToken.Type == JTokenType.Null)
                return string.Empty;

            // If JSON contains a NUMBER: read as double and serialize invariant
            if (valueToken.Type == JTokenType.Float || valueToken.Type == JTokenType.Integer)
            {
                var d = valueToken.Value<double>();
                return d.ToString(CultureInfo.InvariantCulture);
            }

            // If JSON contains a STRING: normalize it to invariant
            if (valueToken.Type == JTokenType.String)
            {
                var s = valueToken.Value<string>()?.Trim();
                if (string.IsNullOrWhiteSpace(s))
                    return string.Empty;

                // Normalize Arabic separators
                s = s.Replace('٫', '.').Replace('،', ',');

                // If decimal comma and no dot -> convert comma to dot
                if (s.Contains(',') && !s.Contains('.'))
                    s = s.Replace(',', '.');

                // Parse without AllowThousands
                if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                    return d.ToString(CultureInfo.InvariantCulture);

                // Fallback: if string is localized already
                if (double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out d))
                    return d.ToString(CultureInfo.InvariantCulture);

                return string.Empty;
            }

            // Last resort
            if (double.TryParse(valueToken.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                return v.ToString(CultureInfo.InvariantCulture);
            }

            return valueToken.ToString();

        }
    }
}
