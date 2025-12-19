using Android;
using Android.Content.PM;
using GrayWolf.Interfaces;

namespace GrayWolf.Droid.Dependencies
{
    public class PrepareRecording_Droid : IPrepareRecording
    {
        public void PrepareRecording()
        {
            //if (AndroidX.Core.Content.ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.RecordAudio) != Permission.Granted)
            //{
            //    AndroidX.Core.App.ActivityCompat.RequestPermissions(Platform.CurrentActivity, new[] { Manifest.Permission.RecordAudio }, 1);
            //}

        }
    }
}