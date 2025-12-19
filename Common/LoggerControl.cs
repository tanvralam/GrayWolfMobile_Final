using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Common
{
    public class LoggerControl
    {
        public LoggerControl()
        {
            LocationDataFileShortName = "";
            LocationDataFilePath = "";
            LoggingInterval = TimeSpan.Zero;
        }
      
        public string LocationDataFileShortName
        {
            get;
            set;
        }
        public string LocationDataFilePath
        {
            get;
            set;
        }
        public TimeSpan LoggingInterval
        {
            get;
            set;
        }
        public static bool IsSafeName(string name)
        {
            char[] badchars = new char[13] { '.', '\\', '/', ':', '*', '?', '"', '<', '>', '|', '&', '(', ')' };
            if (name.IndexOfAny(badchars) >= 0) return false;
            return true;
        }
        
    }
}
