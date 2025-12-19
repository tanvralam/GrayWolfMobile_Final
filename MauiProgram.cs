using Acr.UserDialogs;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Common;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Services;
using GrayWolf.ViewModels;
using GrayWolf.Views;
using Microsoft.Extensions.Logging;
using RGPopup.Maui.Extensions;

using System.Net.Http.Headers;
using System.Text;
using FileSystem = GrayWolf.Services.FileSystem;
using RGPopup.Maui;
//using Plugin.Maui.Audio;
using System.Net.Http;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Extensions;
using LocalizationResourceManager.Maui;
using Plugin.Media.Abstractions;
using Telerik.Maui.Controls;
using Telerik.Maui.Controls.Compatibility;



#if IOS
using GrayWolf.iOS.Dependencies;
#endif
#if ANDROID
using GrayWolf.Droid.Dependencies;
#endif

namespace GrayWolf
{
    public static class MauiProgram
    {
    
        public static MauiApp CreateMauiApp()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUserDialogs()              
                .UseMauiCommunityToolkitMediaElement()
                 .UseTelerik()
                .UseLocalizationResourceManager(settings =>
                {
                    settings.AddResource(Localization.Localization.ResourceManager);
                    settings.RestoreLatestCulture(true);
                })                
                .UseMauiRGPopup(config =>
                {
                    config.BackPressHandler =  null;
                    config.FixKeyboardOverlap = true;
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });



           
            builder.Services.RegisterServices();

            builder.Services.RegisterAndroidServices();
            builder .Services.RegisterIOSServices();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            Ioc.Default.ConfigureServices(app.Services);

            SimpleIoc.Default.TryRegister(() => GetHttpClient(Constants.HTTP_CLIENT_KEY, ConfigureDefaultHttpClient), Constants.HTTP_CLIENT_KEY);
            var defaultClient = SimpleIoc.Default.GetInstance<HttpClient>(Constants.HTTP_CLIENT_KEY);
            SimpleIoc.Default.TryRegister<IApiService>(() => new ApiService(defaultClient), Constants.GRAYWOLF_API_SERVICE_KEY);

            SimpleIoc.Default.TryRegister(() => GetHttpClient(Constants.WUFOO_HTTP_CLIENT_KEY, ConfigureWufooHttpClient), Constants.WUFOO_HTTP_CLIENT_KEY);
            var wufooClient = SimpleIoc.Default.GetInstance<HttpClient>(Constants.WUFOO_HTTP_CLIENT_KEY);
            SimpleIoc.Default.TryRegister<IApiService>(() => new ApiService(wufooClient), Constants.WUFOO_API_SERVICE_KEY);

            SimpleIoc.Default.TryRegister(() => GetHttpClient(Constants.SENSOR_TIPS_HTTP_CLIENT_KEY, ConfigureSensorTipsHttpClient), Constants.SENSOR_TIPS_HTTP_CLIENT_KEY);
            var sensorTipsClient = SimpleIoc.Default.GetInstance<HttpClient>(Constants.SENSOR_TIPS_HTTP_CLIENT_KEY);
            SimpleIoc.Default.TryRegister<IApiService>(() => new ApiService(sensorTipsClient), Constants.SENSOR_TIPS_API_SERVICE_KEY);


#if ANDROID
            SimpleIoc.Default.Register<IPrepareRecording>(() => new PrepareRecording_Droid());
#endif
#if IOS
                        SimpleIoc.Default.Register<IPreparePlayback>(() => new PreparePlayback_iOS());
                        SimpleIoc.Default.Register<IPrepareRecording>(() => new PrepareRecording_iOS());
#endif


            return app;
        }

        public static HttpClient GetHttpClient(string clientKey, Action<HttpClient> configureAction)
        {
            //default http client have some dns issues when used as singleton
            //IHttpClientFactory creates client with correct configuration
            var services = new ServiceCollection();
            services.AddHttpClient(clientKey, configureAction);

            var provider = services.BuildServiceProvider();
            var factory = ServiceProviderServiceExtensions.GetService<IHttpClientFactory>(provider);
            return factory.CreateClient(clientKey);
        }



        private static void ConfigureDefaultHttpClient(HttpClient _client)
        {
            _client
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(SettingsService.Instance.BaseUrl);
        }

        private static void ConfigureSensorTipsHttpClient(HttpClient _client)
        {
            _client
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(Constants.SENSOR_TIPS_BASE_URL);
        }

        private static void ConfigureWufooHttpClient(HttpClient _client)
        {
            _client
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var authenticationString = $"{Constants.WUFOO_USERNAME}:{Constants.WUFOO_SECRET}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            _client.BaseAddress = new Uri(Constants.WUFOO_BASE_URL);
        }
        private static void RegisterServices(this IServiceCollection services)
        {
           
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddTransient <GrayWolf.Interfaces. IFileSystem,FileSystem> ();           
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IAuthService, AuthService>();
            

            services.AddSingleton<IVOCService, VOCService>();
            services.AddSingleton<IWufooService, WufooService>();
            services.AddSingleton<IDeviceAPI, DeviceAPI>();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<ISensorTipsService, SensorTipsService>();
            services.AddSingleton<ISQLite, SQLiteConn>();
            services.AddSingleton<IDatabase, Database>();
            services.AddSingleton<IReadingService, ReadingService>();
            services.AddSingleton<ISensorsService, SensorsService>();

            SimpleIoc.Default.TryRegister<IBleService>(() => new RealBleService(), Constants.BLE_FACTORY_REAL);
            SimpleIoc.Default.TryRegister<IBleService>(() => new MockBleService(), Constants.BLE_FACTORY_MOCK);            
           
            services.AddSingleton<IDeviceService, DeviceService>();
            services.AddSingleton<IGeolocationService, GeolocationService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<INavigationService, NavigationService>();
          


            

        }


        private static void RegisterAndroidServices(this IServiceCollection services)
        {
#if ANDROID
                services.AddSingleton<IStoragePermissionService, StoragePermissionsService>();
                services.AddSingleton<IMediaService, MediaService>();
                services.AddSingleton<IPermissionsService, PermissionsService>();
                services.AddSingleton<IThumbnailService, ThumbnailService>();
                services.AddSingleton<IAlertSoundService, AlertSoundService>();
                services.AddSingleton<ISaveFile, SaveFileAndroid>();
#endif


        }

        private static void RegisterIOSServices(this IServiceCollection services)
        {
#if IOS
             services.AddSingleton<IMediaService, MediaService>();
             services.AddSingleton<IPermissionsService, PermissionsService>();
             services.AddSingleton<IThumbnailService, ThumbnailService>();
            services.AddSingleton<IAlertSoundService, AlertSoundService>();

#endif
        }

    }
}
