using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Return whether or not string is null or empty
        /// </summary>
        public static bool IsNullOrEmpty(this string source) => string.IsNullOrEmpty(source);

        /// <summary>
        /// Return whether or not string is null/empty/contains only spaces
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string source) => string.IsNullOrWhiteSpace(source);

        /// <summary>
        /// Return whether or not string is not null and not empty
        /// </summary>
        public static bool NotNullOrEmpty(this string source) => !string.IsNullOrEmpty(source);

        /// <summary>
        /// Return whether or not string is not null/not empty/contains other symbols besides spaces
        /// </summary>
        public static bool NotNullOrWhiteSpace(this string source) => !string.IsNullOrWhiteSpace(source);
    }
}
