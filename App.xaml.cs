using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Common;
using GrayWolf.Extensions;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Services;
using GrayWolf.Views;
using GrayWolf.Views.Popups;
using Microsoft.Extensions.DependencyInjection;
using RGPopup.Maui.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CommunityToolkit.Maui;
using FileSystem = GrayWolf.Services.FileSystem;
using CommunityToolkit.Mvvm.ComponentModel;
using GrayWolf.Models.Domain;
using System.Globalization;
using GalaSoft.MvvmLight.Ioc;
using LocalizationResourceManager.Maui;



namespace GrayWolf
{
    public partial class App : Application
    {
        public static FlyoutPage FlyoutPage = new FlyoutPage();
        private HomePage navPage;
        private readonly ILocalizationResourceManager LocalizationResourceManager;
        public App()
        {
           
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Constants.SYNC_FUSION_LICENSE);
            LocalizationResourceManager = Ioc.Default.GetService<ILocalizationResourceManager>();

            if (Preferences.ContainsKey("SelectedLanguage"))
            {
                string SelectLang = Preferences.Get("SelectedLanguage", string.Empty);
                LocalizationResourceManager.CurrentCulture = Localization.Localization.Culture=SelectLang == string.Empty ? CultureInfo.CurrentCulture : new CultureInfo(SelectLang);
                              
            }

            InitializeComponent();
             Rebex.Licensing.Key = Constants.REBEX_LICENSE;

          
            navPage = new HomePage();           
            InitializeMainPage(navPage);
        }


     
        private async void InitializeMainPage(Page rootPage)
        {



            var navPage = new NavigationPage(rootPage);
            if (SimpleIoc.Default.IsRegistered<INavigation>())
            {
                SimpleIoc.Default.Unregister<INavigation>();
            }

            SimpleIoc.Default.Register<INavigation>(() => navPage.Navigation);

            App.FlyoutPage.Flyout = new MenuPage();
            App.FlyoutPage.Detail = navPage;
            App.Current.MainPage = App.FlyoutPage;


            // Adjust Flyout behavior for platforms
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                App.FlyoutPage.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
            }
            else if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                App.FlyoutPage.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
            }


        }

        protected async override void OnStart()
        {
            base.OnStart();
            await Task.Delay(500);
            var settings = Ioc.Default.GetService<ISettingsService>();
            if (settings.ShowWelcomeMessage)
            {                
                await navPage.Navigation.PushPopupAsync(new WelcomePopupPage());
            }

        }
      

    }
}
