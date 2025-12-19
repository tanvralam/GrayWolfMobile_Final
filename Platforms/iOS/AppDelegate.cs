using Foundation;
using MediaManager;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
//using Plugin.BluetoothLE;
using UIKit;

namespace GrayWolf
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppCenter.Start("8da0444f-5bbc-4ea8-9130-964d92534605", typeof(Crashes));
            CrossMediaManager.Current.Init();            
            
            RGPopup.Maui.IOS.Popup.Init();
            
            UINavigationBar.Appearance.BarTintColor = Color.FromHex("#303030").ToUIColor();
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });

            //UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
            //if (statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
            //{
            //    statusBar.BackgroundColor = Color.FromHex("#303030").ToUIColor();
            //}

            return base.FinishedLaunching(app, options);
        }
    }
}
