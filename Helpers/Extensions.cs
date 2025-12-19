using GrayWolf.Enums;
using GrayWolf.Models;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using GrayWolf.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace GrayWolf.Helpers
{
    public static class Extensions
    {

        /// <summary>
        /// TODO : To Get Bar Colors according to Ids...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetBarColor(int id)
        {
            string c = string.Empty;
            if (id == 13) c = Colors.CadetBlue.ToHex();
            if (id == 14) c = Colors.Firebrick.ToHex();
            if (id == 15) c = Colors.Gold.ToHex();
            if (id == 16) c = Colors.Aqua.ToHex();
            if (id == 17) c = Colors.Chartreuse.ToHex();
            if (id == 18) c = Colors.Peru.ToHex();
            if (id == 19) c = Colors.MediumPurple.ToHex();
            if (id == 20) c = Colors.MintCream.ToHex();
            if (id == 21) c = Colors.Navy.ToHex();
            if (id == 22) c = Colors.PapayaWhip.ToHex();
            if (id == 23) c = Colors.Plum.ToHex();
            if (id == 24) c = Colors.LawnGreen.ToHex();
            if (id == 25) c = Colors.Silver.ToHex();
            return c;
        }

        /// <summary>
        /// TODO : To Get Random Hex Colors...
        /// </summary>
        /// <returns></returns>
        public static string GetRandomHexColor()
        {
            var random = new Random();
            var color = $"#{random.Next(0x1000000):X6}";
            return color;
        }


        public static byte[] ConverteStreamToByteArray(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static string GetOnlyDigits(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;
            var onlyDigits = phoneNumber.Where(char.IsDigit)
                .ToArray();
            return new string(onlyDigits);
        }


        public static double? ToNullableDouble(this string str)
        {
            if (!double.TryParse(str, out var parsedValue))
            {
                return null;
            }

            return parsedValue;
        }
        public static double ToDouble(this string str)
        {
            if (!double.TryParse(str, out var parsedValue))
            {
                return 0;
            }

            return parsedValue;
        }
        public static DateTime ToDateTime(this string str)
        {
            DateTime.TryParse(str, out var date);
            return date;

        }
        public static DateTime? ToDateNullableTime(this string str)
        {
            if (DateTime.TryParse(str, out var date))
            {
                return date;

            }

            return null;
        }
        public static long ToLong(this string str)
        {
            long.TryParse(str, out var parsedResult);
            return parsedResult;

        }
        public static int ToInt(this string str)
        {
            int.TryParse(str, out var parsedResult);
            return parsedResult;

        }
        public static bool IsValidEmail(this string emailStr)
        {
            if (emailStr == null) return false;
            bool isEmail = Regex.IsMatch(emailStr,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isEmail;
        }


        public static T ObjectToEnum<T>(this object o)
        {
            return (T)Enum.ToObject(typeof(T), o);
        }

        public static T IntToEnum<T>(this int value)
        {
            try
            {
                return (T)Enum.ToObject(typeof(T), value);

            }
            catch (Exception)
            {
                return default(T);
            }

        }

        // <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            /// var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            Newtonsoft.Json.JsonSerializerSettings deserializeSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static Enums.SensorUnit GetSensorUnit(this int sensorUnitCode)
        {
            return Enum.IsDefined(typeof(Enums.SensorUnit), sensorUnitCode) ? (Enums.SensorUnit)sensorUnitCode : Enums.SensorUnit.PRBUNT_UNDEF;
        }

        public static SensorType GetSensorType(this int type)
        {
            return Enum.IsDefined(typeof(SensorType), type) ? (SensorType)type : SensorType.PRBSEN_UNDEF;
        }

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        //    this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}
    }
}
