using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;


namespace GrayWolf.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        public void TrackError(Exception ex, string tag = "", Dictionary<string, object> parameters = null, [CallerMemberName] string caller = "")
        {
            var message = GetMessage(ex.Message, tag, caller);
            PrintMessage(message, parameters);
            if (Device.RuntimePlatform != Device.WinUI)
            {
                Crashes.TrackError(ex, GetParametersForAppCenter(parameters));
            }
        }

        public void TrackEvent(string eventName, string tag = "", Dictionary<string, object> parameters = null, [CallerMemberName] string caller = "")
        {
            var message = GetMessage(eventName, tag, caller);
            PrintMessage(message, parameters);
        }

        private string GetParametersString(Dictionary<string, object> parameters)
        {
            var result = "";
            foreach (var key in parameters)
            {
                result += $"{key}: {parameters}\n";
            }
            return result;
        }

        private string GetMessage(string message, string tag, string caller)
        {
            var prefix = tag.NotNullOrEmpty() ? $"{tag} - " : "";
            return $"{prefix}{caller} {message}";
        }

        private void PrintMessage(string message, Dictionary<string, object> parameters)
        {
            if (parameters != null && parameters.Any())
            {
                message += $"\n{GetParametersString(parameters)}";
            }
            Debug.WriteLine($"{message}");
        }

        private Dictionary<string, string> GetParametersForAppCenter(Dictionary<string, object> parameters)
        {
            parameters = parameters ?? new Dictionary<string, object>();
            var result = new Dictionary<string, string>();
            foreach (var key in parameters.Keys)
            {
                result.Add(key, $"{parameters[key]}");
            }
            return result;
        }
    }
}
