using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Enums;
using GrayWolf.Extensions;
using GrayWolf.Helpers;
using GrayWolf.Messages;
using System.Threading.Tasks;


namespace GrayWolf.Services
{
    public class SettingsService : ISettingsService
    {
        public static ISettingsService Instance => (ISettingsService)Ioc.Default.GetService(typeof(ISettingsService));

        public SettingsService()
        {

#if DEBUG
            Email = "demo@graywolfsensing.com";
            Password = "vocprobe";
#endif

        }

        public string BaseUrlPrefix
        {
            set { Preferences.Set(nameof(BaseUrlPrefix), value); }
            get { return Preferences.Get(nameof(BaseUrlPrefix), "us"); }
        }

        public string BaseUrl => $"https://{BaseUrlPrefix}.graywolflive.com";



        public string Email
        {
            set { Preferences.Set(nameof(Email), value); }
            get { return Preferences.Get(nameof(Email), string.Empty); }
        }

        public string Password
        {
            set { Preferences.Set(nameof(Password), value); }
            get { return Preferences.Get(nameof(Password), string.Empty); }
        }

        public int LoggerIntervalMs
        {
            set { Preferences.Set(nameof(LoggerIntervalMs), value); }
            get { return Preferences.Get(nameof(LoggerIntervalMs), -1); }
        }

        public int LogFileId
        {
            set { Preferences.Set(nameof(LogFileId), value); }
            get { return Preferences.Get(nameof(LogFileId), -1); }
        }

        public GraphTimeOption GraphRange
        {
            set => Preferences.Set(nameof(GraphRange), (int)value);
            get => (GraphTimeOption)Preferences.Get(nameof(GraphRange), (int)GraphTimeOption.Everything);
        }

        public string SelectedGraphParameterId
        {
            set => Preferences.Set(nameof(SelectedGraphParameterId), value);
            get => Preferences.Get(nameof(SelectedGraphParameterId), "");
        }

        public bool IsReadingsMigrationFinished
        {
            set => Preferences.Set(nameof(IsReadingsMigrationFinished), value);
            get => Preferences.Get(nameof(IsReadingsMigrationFinished), false);
        }

        public ZipProtectionMode ProtectionMode
        {
            set => Preferences.Set(nameof(ZipProtectionMode), (int)value);
            get => (ZipProtectionMode)Preferences.Get(nameof(ZipProtectionMode), (int)ZipProtectionMode.DefaultPassword);
        }

        public bool IncludeCsvIntoExport
        {
            get => Preferences.Get(nameof(IncludeCsvIntoExport), true);
            set => Preferences.Set(nameof(IncludeCsvIntoExport), value);
        }

        public bool IsDemoMode
        {
            get => Preferences.Get(nameof(IsDemoMode), false);
            set => Preferences.Set(nameof(IsDemoMode), value);
        }

        public bool IsFirstDemoRun
        {
            get => Preferences.Get(nameof(IsFirstDemoRun), true);
            set => Preferences.Set(nameof(IsFirstDemoRun), value);
        }

        public Task SetCustomZipPasswordAsync(string password)
        {
            if (password.IsNullOrEmpty())
            {
                SecureStorage.Remove(Constants.CUSTOM_ZIP_PASSWORD_KEY);
                return Task.CompletedTask;
            }
            return SecureStorage.SetAsync(Constants.CUSTOM_ZIP_PASSWORD_KEY, password);
        }

        public Task<string> GetCustomZipPasswordAsync()
        {
            return SecureStorage.GetAsync(Constants.CUSTOM_ZIP_PASSWORD_KEY);
        }

        public ParameterNameDisplayOption ParameterNameDisplayMode
        {
            get => (ParameterNameDisplayOption)Preferences.Get(nameof(ParameterNameDisplayMode), (int)ParameterNameDisplayOption.Short);
            set => Preferences.Set(nameof(ParameterNameDisplayMode), (int)value);
        }

        public bool AreGalleryAttachmentsEnabled
        {
            get => Preferences.Get(nameof(AreGalleryAttachmentsEnabled), false);
            set
            {
                Preferences.Set(nameof(AreGalleryAttachmentsEnabled), value);
            }
        }

        public bool ShowWelcomeMessage
        {
            get => Preferences.Get(nameof(ShowWelcomeMessage), true);
            set => Preferences.Set(nameof(ShowWelcomeMessage), value);
        }

        public bool EnableSnapshotFromDSIILogButton
        {
            get => Preferences.Get(nameof(EnableSnapshotFromDSIILogButton), false);
            set => Preferences.Set(nameof(EnableSnapshotFromDSIILogButton), value);
        }
    }
}
