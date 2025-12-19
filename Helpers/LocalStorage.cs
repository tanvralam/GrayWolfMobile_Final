using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Helpers
{
    public class LocalStorage
    {
        private static ISettings AppSettings => CrossSettings.Current; // ข้อมูล Setting

        #region Local Constants

        #region LogFile 
        private const string Token = "Token";
        private static readonly string TokenDefault = string.Empty;
        public static string GeneralToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(Token, TokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(Token, value);
            }
        }
        #endregion
        #endregion
    }
}
