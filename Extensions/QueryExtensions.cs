using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace GrayWolf.Extensions
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Convert NameValueCollection to string with correct urlquery format
        /// </summary>
        /// <param name="nvc">Collection of query parameters</param>
        /// <returns>Converted query string that to put in url</returns>
        public static string ToQuery(this NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
            "{0}={1}",
            HttpUtility.UrlEncode(key),
            HttpUtility.UrlEncode(value))
                ).ToArray();
            return "?" + string.Join("&", array);
        }

        /// <summary>
        /// Encodes a URL string
        /// </summary>
        /// <returns>An encoded URL string</returns>
        public static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }
    }
}
