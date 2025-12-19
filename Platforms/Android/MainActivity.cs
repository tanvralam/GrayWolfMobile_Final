using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using GalaSoft.MvvmLight.Ioc;
using MediaManager;
using Microsoft.AppCenter;
using Plugin.CurrentActivity;
using GrayWolf.Extensions;
using Microsoft.AppCenter.Crashes;
using GrayWolf.Droid.Dependencies;
using Microsoft.Maui.Controls.Compatibility;
namespace GrayWolf
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

    //[Activity(Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

    public class MainActivity : MauiAppCompatActivity
    {
        public const int SaveFileRequestCode = 12;
        // Storage Permissions
        private static int REQUEST_EXTERNAL_STORAGE = 1;
        private static String[] PERMISSIONS_STORAGE = {
        Manifest.Permission.ReadExternalStorage,
        Manifest.Permission.WriteExternalStorage };

        private TaskCompletionSource<Android.Net.Uri> _createFileTCS;
        public static MainActivity instance { set; get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppCenter.Start("dcfda6a9-8466-421a-a935-773f50e47d2e", typeof(Crashes));
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#303030"));
            CrossMediaManager.Current.Init(this);
           CrossCurrentActivity.Current.Init(this, savedInstanceState);
            // Request microphone permissions at runtime
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RecordAudio }, 1);
            }
            SetTheme(Resource.Style.MainTheme);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == SaveFileRequestCode)
            {
                if (data?.Data != null)
                {
                    var result = data.Data as Android.Net.Uri;
                    _createFileTCS.TrySetResult(result);
                }
                _createFileTCS = null;
            }
        }

     
        public Task<Android.Net.Uri> CreateFileAsync(Intent intent)
        {
            _createFileTCS = new TaskCompletionSource<Android.Net.Uri>();
            StartActivityForResult(intent, SaveFileRequestCode);
            return _createFileTCS.Task;
        }


    }
}
