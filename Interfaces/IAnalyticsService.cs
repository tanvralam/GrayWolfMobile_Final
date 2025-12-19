using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GrayWolf.Interfaces
{
    public interface IAnalyticsService
    {
        void TrackError(Exception ex, string tag = "", Dictionary<string, object> parameters = null, [CallerMemberName] string caller = "");

        void TrackEvent(string eventName, string tag = "", Dictionary<string, object> parameters = null, [CallerMemberName] string caller = "");
    }
}
