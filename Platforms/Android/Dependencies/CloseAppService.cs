using System;
using GrayWolf.Droid.Dependencies;
using Java.Lang;
using Microsoft.Maui.Controls.Compatibility;

namespace GrayWolf.Droid.Dependencies
{
    public class CloseAppService
    {
        public CloseAppService()
        {
        }

        public void CloseApp()
        {
            var activity = (Android.App.Activity)Forms.Context;
            activity.FinishAffinity();
            JavaSystem.Exit(0);
        }
    }
}
