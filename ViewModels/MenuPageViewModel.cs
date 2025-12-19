using Acr.UserDialogs;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Enums;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using GrayWolf.Views;
using GrayWolf.Views.Popups;
using MvvmHelpers;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui;
using GrayWolf.Localization;
using ExpandMenuItem = GrayWolf.Models.Domain.MenuItem;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Commands=MvvmHelpers.Commands;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using LocalizationResourceManager.Maui;
using System.Resources;
using Microsoft.Maui.Platform;

namespace GrayWolf.ViewModels
{
    public  class MenuPageViewModel : BaseMenuViewModel
    {
        private const string UpArrow = "uparrow.png";
        private const string DownArrow = "downarrow.png";
        private const string VideoHelpUrl = "https://graywolfsensing.com/on-board-video-help/";

        #region Commands
        public ICommand LiveCommand { get; }
        public ICommand LogCommand { get; }
        public IAsyncRelayCommand StartLogPopupCommand { get; }
        public ICommand NoteCommand { get; }
        public ICommand StopLogCommand { get; }
        public ICommand PhotoLogCommand { get; }
        public ICommand AudioLogCommand { get; }
        public ICommand TextLogCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand VideoLogCommand { get; }
        public IAsyncRelayCommand DrawingNoteCommand { get; }
        public ICommand ReadingsCommand { get; }
        public ICommand VideoHelpCommand { get; }
        public ICommand CloseAppCommand { get; }
        public ICommand EncryptionCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand ViewCommand { get; }
        public IAsyncRelayCommand SelectDevicesCommand { get; }
        
        public ICommand StartDemoCommand { get; }
        public ICommand StopDemoCommand { get; }
        public ICommand ParameterNamesCommand { get; }
        public ICommand GalleryAttachmentsSettingsCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ProbeOptionsCommand { get; }
        public ICommand VOCCommand { get; }
        #endregion Commands

        #region Properties
        public ExpandMenuItem ExpandLog { get; } = GetDefaultExpandItem();

        public ExpandMenuItem ExpandNotes { get; } = GetDefaultExpandItem();

        public ExpandMenuItem ExpandReadings { get; } = GetDefaultExpandItem();

        public ExpandMenuItem ExpandSettings { get; } = GetDefaultExpandItem();

        public ExpandMenuItem ExpandView { get; } = GetDefaultExpandItem();

        public ExpandMenuItem ExpandExport { get; } = GetDefaultExpandItem();

        public bool CanStartDemo => !DeviceService.IsDemoMode && !LogService.IsLogging;

        public bool CanStopDemo => DeviceService.IsDemoMode;

        private ObservableCollection<(string name, string value)> _languageMapping;
        public ObservableCollection<(string name, string value)> LanguageMapping
        {
            get => _languageMapping;
            private set => SetProperty(ref _languageMapping, value);
        }

       // public string CurrentLanguage { get; set; }
        public LocalizedString CurrentLanguage { get; }
        //public LocalizedString Version { get; } = new(() => string.Format(Localization.Localization.Version, AppInfo.VersionString));
        private ILocalizationResourceManager LocalizationResourceManager;
        public ICommand ChangeLanguageCommand { get; }

        private string _menu_Log;
        public string Menu_Log
        {
            get => _menu_Log;
            private set => SetProperty(ref _menu_Log, value);
        }
        private string _menu_StartLog;
        public string Menu_StartLog
        {
            get => _menu_StartLog;
            private set => SetProperty(ref _menu_StartLog, value);
        }
        private string _startLog_Snapshot;
        public string StartLog_Snapshot
        {
            get => _startLog_Snapshot;
            private set => SetProperty(ref _startLog_Snapshot, value);
        }
        private string _menu_StopLog;
        public string Menu_StopLog
        {
            get => _menu_StopLog;
            private set => SetProperty(ref _menu_StopLog, value);
        }
        private string _menu_Locations;
        public string Menu_Locations
        {
            get => _menu_Locations;
            private set => SetProperty(ref _menu_Locations, value);
        }
        private string _menu_Notes;
        public string Menu_Notes
        {
            get => _menu_Notes;
            private set => SetProperty(ref _menu_Notes, value);
        }
        private string _menu_ReviewNotes;
        public string Menu_ReviewNotes
        {
            get => _menu_ReviewNotes;
            private set => SetProperty(ref _menu_ReviewNotes, value);
        }
        private string _menu_Photo;
        public string Menu_Photo
        {
            get => _menu_Photo;
            private set => SetProperty(ref _menu_Photo, value);
        }
        //-------
        private string _menu_Audio;
        public string Menu_Audio
        {
            get => _menu_Audio;
            private set => SetProperty(ref _menu_Audio, value);
        }
        private string _menu_Text;
        public string Menu_Text
        {
            get => _menu_Text;
            private set => SetProperty(ref _menu_Text, value);
        }
        private string _menu_Event;
        public string Menu_Event
        {
            get => _menu_Event;
            private set => SetProperty(ref _menu_Event, value);
        }
        private string _menu_Video;
        public string Menu_Video
        {
            get => _menu_Video;
            private set => SetProperty(ref _menu_Video, value);
        }
        private string _menu_Readings;
        public string Menu_Readings
        {
            get => _menu_Readings;
            private set => SetProperty(ref _menu_Readings, value);
        }
        private string _menu_SelectDevices;
        public string Menu_SelectDevices
        {
            get => _menu_SelectDevices;
            private set => SetProperty(ref _menu_SelectDevices, value);
        }
        private string _menu_StartDemoIAQ;
        public string Menu_StartDemoIAQ
        {
            get => _menu_StartDemoIAQ;
            private set => SetProperty(ref _menu_StartDemoIAQ, value);
        }
        private string _menu_StartDemoTOX;
        public string Menu_StartDemoTOX
        {
            get => _menu_StartDemoTOX;
            private set => SetProperty(ref _menu_StartDemoTOX, value);
        }
        private string _menu_EndDemo;
        public string Menu_EndDemo
        {
            get => _menu_EndDemo;
            private set => SetProperty(ref _menu_EndDemo, value);
        }

        private string _menu_VOC;
        public string Menu_VOC
        {
            get => _menu_VOC;
            private set => SetProperty(ref _menu_VOC, value);
        }
        private string _menu_View;
        public string Menu_View
        {
            get => _menu_View;
            private set => SetProperty(ref _menu_View, value);
        }
        private string _menu_Graph;
        public string Menu_Graph
        {
            get => _menu_Graph;
            private set => SetProperty(ref _menu_Graph, value);
        }
        private string _menu_Data;
        public string Menu_Data
        {
            get => _menu_Data;
            private set => SetProperty(ref _menu_Data, value);
        }
        private string _menu_Live;
        public string Menu_Live
        {
            get => _menu_Live;
            private set => SetProperty(ref _menu_Live, value);
        }
        private string _menu_Export;
        public string Menu_Export
        {
            get => _menu_Export;
            private set => SetProperty(ref _menu_Export, value);
        }
        private string _menu_Settings;
        public string Menu_Settings
        {
            get => _menu_Settings;
            private set => SetProperty(ref _menu_Settings, value);
        }
        private string _menu_Encryption;
        public string Menu_Encryption
        {
            get => _menu_Encryption;
            private set => SetProperty(ref _menu_Encryption, value);
        }
        private string _menu_ParameterNames;
        public string Menu_ParameterNames
        {
            get => _menu_ParameterNames;
            private set => SetProperty(ref _menu_ParameterNames, value);
        }
        private string _menu_GalleryAttachmentsSettings;
        public string Menu_GalleryAttachmentsSettings
        {
            get => _menu_GalleryAttachmentsSettings;
            private set => SetProperty(ref _menu_GalleryAttachmentsSettings, value);
        }
        private string _menu_ProbeOptions;
        public string Menu_ProbeOptions
        {
            get => _menu_ProbeOptions;
            private set => SetProperty(ref _menu_ProbeOptions, value);
        }
        private string _menu_Select_Language;
        public string Menu_Select_Language
        {
            get => _menu_Select_Language;
            private set => SetProperty(ref _menu_Select_Language, value);
        }
        private string _menu_About;
        public string Menu_About
        {
            get => _menu_About;
            private set => SetProperty(ref _menu_About, value);
        }
        private string _menu_VideoHelp;
        public string Menu_VideoHelp
        {
            get => _menu_VideoHelp;
            private set => SetProperty(ref _menu_VideoHelp, value);
        }
        private string _menu_CloseApp;
        public string Menu_CloseApp
        {
            get => _menu_CloseApp;
            private set => SetProperty(ref _menu_CloseApp, value);
        }
        #endregion Properties

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuPageViewModel"/> class.
        /// </summary>
        /// <param name="nav"></param>
        public MenuPageViewModel() : base()
        {
            LiveCommand = new Command(OnLive);
            LogCommand = new Command(OnLog);
            StartLogPopupCommand = new AsyncRelayCommand(OnStartLogPopupAsync);
            NoteCommand = new Command(OnNote);
            StopLogCommand = new Command(OnStopLog);
            PhotoLogCommand = new AsyncRelayCommand(OnPhotoLogAsync);
            AudioLogCommand = new AsyncRelayCommand(OnAudioLogAsync);
            TextLogCommand = new AsyncRelayCommand(OnTextLogAsync);
            VideoLogCommand = new AsyncRelayCommand(OnVideo);
            ReadingsCommand = new Command(OnReadings);
            SelectDevicesCommand = new AsyncRelayCommand(OnSelectDevices);
            DrawingNoteCommand = new AsyncRelayCommand(OnImageNote);
            AboutCommand = new Command(GoToAboutPage);
            VideoHelpCommand = new Command(OpenHelpVideo);
            CloseAppCommand = new Command(CloseApp);
            EncryptionCommand = new Command(GoToEncryptionMenu);
            SettingsCommand = new Command(OnSettings);
            ViewCommand = new Command(OnView);
            StartDemoCommand = new Command<DemoProbeType>(StartDemo);
            StopDemoCommand = new Command(StopDemo);
            ParameterNamesCommand = new Command(OpenParameterNamesMenu);
            GalleryAttachmentsSettingsCommand = new Command(OpenGalleryAttachmentsSettings);
            ExportCommand = new Command(() => OnLocation(true));
            ProbeOptionsCommand = new Command(GoToProbeOptions);
            VOCCommand = new Command(GoToVOC);
            MessengerInstance.Register<LogStatusChangedMessage>(this, m => RaiseLogStatusChanged());
            
            LocalizationResourceManager= Ioc.Default.GetService<ILocalizationResourceManager>();
            LanguageMapping = new()
            {
                ("English", "en"),
                ("Spanish/Española", "es"),
                ("Chinese/中文", "zh"),
                ("Portuguese/Portugues", "pt"),
                ("Arabic/اللغة العربية", "ar"),
                ("French/Français", "fr"),
                ("Italian/Italiana", "it"),
                ("Korean/한국어", "ko"),
                ("German/Deutsch", "de"),
                ("Romanian/Română", "ro"),
                //("Japanese/日本語", "ja"),
            };

           if (Preferences.ContainsKey("SelectedLanguage"))
            {
                string SelectLang = Preferences.Get("SelectedLanguage",string.Empty);//LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName;
               // CultureInfo.CurrentCulture = string.IsNullOrEmpty(SelectLang) ? CultureInfo.CurrentCulture : new CultureInfo(SelectLang);
                LocalizationResourceManager.CurrentCulture = string.IsNullOrEmpty(SelectLang) ? CultureInfo.CurrentCulture : new CultureInfo(SelectLang);

               // CultureInfo.DefaultThreadCurrentCulture = LocalizationResourceManager.CurrentCulture;
               // CultureInfo.DefaultThreadCurrentUICulture = LocalizationResourceManager.CurrentCulture;
            }

            
            CurrentLanguage = new(GetCurrentLanguageName);
            ChangeLanguageCommand = new Commands.AsyncCommand(ChangeLanguage);
            SetLocalizationData();

        }

        #endregion CONSTRUCTOR



        private void SetLocalizationData()
        {
            if (CurrentLanguage.Localized == "Spanish" || CurrentLanguage.Localized == "Romanian")
            {
                Settings.ParameterNameDisplayMode = ParameterNameDisplayOption.Short;
            }
            Menu_Log = Localization.Localization.Menu_Log;
            Menu_StartLog = Localization.Localization.Menu_StartLog;
            StartLog_Snapshot = Localization.Localization.StartLog_Snapshot;
            Menu_StopLog = Localization.Localization.Menu_StopLog;
            Menu_Locations = Localization.Localization.Menu_Locations;
            Menu_Notes = Localization.Localization.Menu_Notes;
            Menu_ReviewNotes = Localization.Localization.Menu_ReviewNotes;
            Menu_Photo = Localization.Localization.Menu_Photo;
            Menu_Audio = Localization.Localization.Menu_Audio;
            Menu_Text = Localization.Localization.Menu_Text;
            Menu_Event = Localization.Localization.Menu_Event;
            Menu_Video = Localization.Localization.Menu_Video;
            Menu_Readings = Localization.Localization.Menu_Readings;
            Menu_SelectDevices = Localization.Localization.Menu_SelectDevices;
            Menu_StartDemoIAQ = Localization.Localization.Menu_StartDemoIAQ;
            Menu_StartDemoTOX = Localization.Localization.Menu_StartDemoTOX;
            Menu_EndDemo = Localization.Localization.Menu_EndDemo;
            Menu_VOC = Localization.Localization.Menu_VOC;
            Menu_View = Localization.Localization.Menu_View;
            Menu_Graph = Localization.Localization.Menu_Graph;
            Menu_Data = Localization.Localization.Menu_Data;
            Menu_Live = Localization.Localization.Menu_Live;
            Menu_Export = Localization.Localization.Menu_Export;
            Menu_Settings = Localization.Localization.Menu_Settings;
            Menu_Encryption = Localization.Localization.Menu_Encryption;
            Menu_ParameterNames = Localization.Localization.Menu_ParameterNames;
            Menu_GalleryAttachmentsSettings = Localization.Localization.Menu_GalleryAttachmentsSettings;
            Menu_ProbeOptions = Localization.Localization.Menu_ProbeOptions;
            Menu_Select_Language = Localization.Localization.Select_Language;
            Menu_About = Localization.Localization.Menu_About;
            Menu_VideoHelp = Localization.Localization.Menu_VideoHelp;
            Menu_CloseApp = Localization.Localization.Menu_CloseApp;
        }

        protected override void RaiseLogStatusChanged()
        {
            base.RaiseLogStatusChanged();
            RaisePropertyChanged(nameof(CanStartDemo));
        }

        #region Methods


        private string GetCurrentLanguageName()
        {
          
            
            var (knownName, _) = _languageMapping.SingleOrDefault(m => m.value == LocalizationResourceManager.CurrentCulture.TwoLetterISOLanguageName);
            return knownName != null ? knownName : LocalizationResourceManager.CurrentCulture.DisplayName;
        }

        async Task ChangeLanguage()
        {
            try
            {
                string selectedName = await Application.Current.MainPage.DisplayActionSheet(
                    Localization.Localization.ChangeLanguage,
                    null, null,
                    _languageMapping.Select(m => m.name).ToArray());
                if (selectedName == null)
                {
                    return;
                }

                string selectedValue = _languageMapping.Single(m => m.name == selectedName).value;

                Preferences.Set("SelectedLanguage", selectedValue);
                LocalizationResourceManager.CurrentCulture = Localization.Localization.Culture = string.IsNullOrEmpty(selectedValue) ? CultureInfo.CurrentCulture : new CultureInfo(selectedValue);
                HomePage._homeVm.UpdateCurrentLogName();
                HomePage.homePage.BindingContext = null;
                HomePage.homePage.BindingContext = HomePage._homeVm;
                SetLocalizationData();
            }
            catch (Exception ex)
            {
                var d = ex.Message;
            }
        }

        private async Task OnVideo()
        {
            if (!SetBusy())
            {
                return;
            }
            await LogService.AddVideo();
            IsBusy = false;
        }

        private async Task OnImageNote()
        {
            if (!SetBusy())
            {
                return;
            }
            if (LogService.CurrentFile == null)
            {
                Ioc.Default.GetService<IUserDialogs>().Alert("No logger location is selected. Please select a location before adding a note", "Alert", "Ok");
            }
            else
            {
                await NavigationService.Instance.Nav.PushPopupAsync(new DrawingNotePopup());
                App.FlyoutPage.IsPresented = false;
            }
            IsBusy = false;
        }

        public async void CloseApp()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await MenuPage.menuPage.DisplayAlert("", "Would you like to exit from application?", "Yes", "No");
                    if (result)
                    {
                       System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                });
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnSelectDevices()
        {
            if (!SetBusy())
            {
                return;
            }
            if (CanSelectDevice)
            {
                await NavigationService.Instance.Nav.PushPopupAsync(new SelectDevicesPopup());
                App.FlyoutPage.IsPresented = false;
            }
            else
            {
                if (LogService.IsLogging)
                {
                    await Alert.ShowAlert(Localization.Localization.SelectDevices_LogIsRunningMessage);
                }
                else if (DeviceService.IsDemoMode)
                {
                    await Alert.ShowAlert(Localization.Localization.SelectDevices_CannotOpenInDemo);
                }
            }
            IsBusy = false;
        }

        private void OnReadings()
        {
            if (!SetBusy())
            {
                return;
            }
            OnExpandItemTapped(ExpandReadings);
            IsBusy = false;
        }

        private void OnSettings()
        {
            if (!SetBusy())
            {
                return;
            }
            OnExpandItemTapped(ExpandSettings);
            IsBusy = false;
        }

        private void OnView()
        {
            if (!SetBusy())
            {
                return;
            }

            OnExpandItemTapped(ExpandView);
            IsBusy = false;
        }

        private async void OnLive()
        {
            App.FlyoutPage.IsPresented = false;
            await Navigation.PopToRootAsync();
        }

        /// <summary>
        ///To Perform Log Operations (Open log Sub Mune)...
        /// </summary>
        private void OnLog()
        {
            if (!SetBusy())
            {
                return;
            }
            OnExpandItemTapped(ExpandLog);
            IsBusy = false;
        }

        /// <summary>
        /// To Open Start Log Popup Page...
        /// </summary>
        private async Task OnStartLogPopupAsync()
        {
            //Show Popup;
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var location = LogService.CurrentFile;
                if (location == null)
                {
                    await Alert.ShowAlert(Localization.Localization.StartLog_SelectLocationMessage);
                    return;
                }
                switch (LogService.Status)
                {
                    case Enums.LogStatus.NoDevicesSelected:
                        await Alert.ShowAlert(Localization.Localization.StartLog_SelectDevicesMessage);
                        break;
                    case Enums.LogStatus.Logging:
                        await Alert.ShowAlert(Localization.Localization.StartLog_Snahshot_LogIsAlreadyRunning);
                        break;
                    case Enums.LogStatus.DevicesSelected:
                        await Navigation.PushPopupAsync(new StartLogPopupPage());
                        App.FlyoutPage.IsPresented = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Open notes submenu
        /// </summary>
        private void OnNote()
        {
            if (!SetBusy())
            {
                return;
            }
            OnExpandItemTapped(ExpandNotes);
            IsBusy = false;
        }

        /// <summary>
        /// To Perform Stop Log Operations...
        /// </summary>
        private async void OnStopLog()
        {
            if (!CanStopLog || !SetBusy())
            {
                return;
            }
            await LogService.StopLog();
            IsBusy = false;
        }

        /// <summary>
        ///  To Perform Photo Log Operations...
        /// </summary>
        private async Task OnPhotoLogAsync()
        {
            if (!SetBusy())
            {
                return;
            }
            await LogService.TakePhoto();
            IsBusy = false;
        }

        /// <summary>
        /// To Perform Audio Log Operations...
        /// </summary>
        private async Task OnAudioLogAsync()
        {
            if (!SetBusy())
            {
                return;
            }
            await LogService.RecordSound();
            IsBusy = false;
        }

        /// <summary>
        ///  To Perform Text Log Operations...
        /// </summary>
        private async Task OnTextLogAsync()
        {
            if (!SetBusy())
            {
                return;
            }
            await LogService.AddNote();
            IsBusy = false;
        }

        private async void GoToAboutPage()
        {
            if (!SetBusy())
            {
                return;
            }
            App.FlyoutPage.IsPresented = false;            
            await NavigationService.Instance.NavigateTo(new AboutPage());
            IsBusy = false;
        }

        private void OnExpandItemTapped(ExpandMenuItem item)
        {
            item.IsExpanded = !item.IsExpanded;
            item.ImageSource = item.IsExpanded ? UpArrow : DownArrow;
        }

        private static ExpandMenuItem GetDefaultExpandItem()
        {
            return new ExpandMenuItem
            {
                ImageSource = DownArrow,
                IsExpanded = false
            };
        }

        private async void OpenHelpVideo()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                await Launcher.OpenAsync(VideoHelpUrl);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void GoToEncryptionMenu()
        {
            if (!SetBusy())
            {
                return;
            }

            await Navigation.PushPopupAsync(new ZipPasswordPopup());
            App.FlyoutPage.IsPresented = false;

            IsBusy = false;
        }

        private async void StartDemo(DemoProbeType mode)
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                if (DeviceService.IsDemoMode)
                {
                    await Alert.ShowAlert(Localization.Localization.Menu_AlreadyInDemo);
                    return;
                }
                else if (LogService.IsLogging)
                {
                    await Alert.ShowAlert(Localization.Localization.SelectDevices_LogIsRunningMessage);
                    return;
                }
                await DeviceService.StartDemoModeAsync(mode);
                OnDemoStatusChanged();

                await OpenDemoFormIfNeededAsync();
                App.FlyoutPage.IsPresented = false;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void StopDemo()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                if (!DeviceService.IsDemoMode)
                {
                    await Alert.ShowAlert(Localization.Localization.Menu_AlreadyStoppedDemo);
                    return;
                }
                await DeviceService.StopDemoModeAsync();
                OnDemoStatusChanged();
                App.FlyoutPage.IsPresented = false;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenDemoFormIfNeededAsync()
        {
            if (Settings.IsFirstDemoRun)
            {
                await Navigation.PushPopupAsync(new FirstDemoFormPopupPage());
            }
        }

        private void OnDemoStatusChanged()
        {
            RaisePropertyChanged(nameof(CanStartDemo));
            RaisePropertyChanged(nameof(CanStopDemo));
        }

        private async void OpenParameterNamesMenu()
        {
            if (!SetBusy())
            {
                return;
            }

            App.FlyoutPage.IsPresented = false;
            await Navigation.PushPopupAsync(new ParameterNamesPopupPage());

            IsBusy = false;
        }

        private async void OpenGalleryAttachmentsSettings()
        {
            if (!SetBusy())
            {
                return;
            }

            App.FlyoutPage.IsPresented = false;
            await Navigation.PushPopupAsync(new GalleryAttachmentsSettingsPopupPage());

            IsBusy = false;
        }

        private async void GoToProbeOptions()
        {
            if (!SetBusy())
            {
                return;
            }

            App.FlyoutPage.IsPresented = false;
            await Navigation.PushPopupAsync(new ProbeOptionsPopupPage());

            IsBusy = false;
        }

        private async void GoToVOC()
        {
            if (!SetBusy())
            {
                return;
            }


            try
            {
                var vocService = Ioc.Default.GetService<IVOCService>();

                var items = await vocService.GetVOCItemsAsync();

                var tcs = new TaskCompletionSource<VOCItem>();
                await Navigation.PushPopupAsync(new SelectVOCItemPopupPage(items, tcs));
                var item = await tcs.Task;

                App.FlyoutPage.IsPresented = false;
               await Navigation.PushAsync(new VOCListPage(items, item));
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                await Alert.DisplayError(ex);
            }

            IsBusy = false;
        }
        #endregion Methods
    }
}