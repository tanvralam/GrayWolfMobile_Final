using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Common;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
#if ANDROID
using Android.Content.Res;
#endif
namespace GrayWolf.ViewModels
{
    public class StartLogPopupPageViewModel : BaseLogsPopupViewModel
    {
        #region PROPERTIES 
        private int _minCount = 0;
        public int MinCount
        {
            get { return _minCount; }
            set
            => SetProperty(ref _minCount, value);
        }

        private int _secCount;
        public int SecCount
        {
            get { return _secCount; }
            set => SetProperty(ref _secCount, value);
        }

        private bool _logFilesAvailable;
        public bool LogFilesAvailable
        {
            get { return _logFilesAvailable; }
            set => SetProperty(ref _logFilesAvailable, value);
        }

        private bool _logFilesNotAvailable;
        public bool LogFilesNotAvailable
        {
            get { return _logFilesNotAvailable; }
            set => SetProperty(ref _logFilesNotAvailable, value);
        }

        private bool _isLogging;

        public bool IsLogging
        {
            get { return _isLogging; }
            set
            {
                _isLogging = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region DELEGATECOMMAND  


        public ICommand AddFileCommand { get; }
        public ICommand MinMinusCommand { get; }
        public ICommand MinPlusCommand { get; }
        public ICommand SecMinusCommand { get; }
        public ICommand SecPlusCommand { get; }

        public IAsyncRelayCommand StartLogCommand { get; }

        private IAlertSoundService AlertSound = Ioc.Default.GetService<IAlertSoundService>();

        public ICommand SnapshotCommand { get; }
        public ICommand SelectFileCommand { get; }
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="StartLogPopupPageViewModel"/> class.
        /// </summary>
        /// <param name="nav"></param>
        public StartLogPopupPageViewModel()
        {
//#if ANDROID
//                if (DeviceInfo.Platform==DevicePlatform.Android)
//                                {
//                                    var dialogStyle = Resource.Style.
//                                    AlertConfig.DefaultAndroidStyleId = dialogStyle;
//                                }
//#endif
            MinMinusCommand = new Command(OnMinMinus);
            MinPlusCommand = new Command(OnMinPlus);
            SecMinusCommand = new Command(OnSecMinus);
            SecPlusCommand = new Command(OnSecPlus);
            AddFileCommand = new AsyncRelayCommand(OnAddFile);

            StartLogCommand = new AsyncRelayCommand(OnStartLog);
            SnapshotCommand = new Command(OnSnapshot);
            SelectFileCommand = new Command<LogFile>(SelectFile);
        }
        #endregion

        #region override
        public override void OnAppearing()
        {
            base.OnAppearing();
            Load();
        }

        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
            if (propertyName == nameof(SelectedLog))
            {
                OnFileSelected();
                UpdateLogInterval();
            }
        }
        #endregion

        #region methods
        public async void Load()
        {
            try
            {
                var logFilesList = await LogService.GetLogFilesAsync();

                if (!logFilesList.Any())
                {
                    LogFilesAvailable = false;
                    LogFilesNotAvailable = true;
                    UpdateLogInterval();

                    return;
                }

                LocationList = new ObservableCollection<LogFile>(logFilesList);
                //Select the last used logfile

                SelectedLog = LocationList.FirstOrDefault(p =>
                    p.IsSelected);
                InvokeSelectedLogInitialized();

                LogFilesAvailable = true;
                LogFilesNotAvailable = false;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }

        private void SelectFile(LogFile logFile)
        {
            SelectedLog = logFile;
        }

        private void UpdateLogInterval()
        {
            var totalMS = Constants.INTERVAL_LOG_MS_DEFAULT;
            if (SelectedLog != null)
            {
                totalMS = SelectedLog.LoggingInterval;
            }
            var totalSec = totalMS / 1000;
            MinCount = totalSec / 60;
            SecCount = totalSec - MinCount * 60;
        }

        private async Task OnStartLog()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                if (SelectedLog == null)
                {
                    Ioc.Default.GetService<IUserDialogs>().Alert(Localization.Localization.StartLog_SelectLocationMessage, "Alert", "Ok");
                    return;
                }
                if (LogService.IsLogging)
                {
                    Ioc.Default.GetService<IUserDialogs>().Alert("Logger is already running.", "Alert", "Ok");
                    await OnBacksAsync();
                    return;
                }

                var logFile = LocationList.FirstOrDefault(p => p.IsSelected);
                int totalMS = _minCount * 60 + _secCount;
                logFile.LoggingInterval = totalMS * 1000;

                var fileMode = LogFileWriteMode.Default;
                var isExist = await LogService.IsLogFileExistAsync(logFile.Id);
                if (isExist)
                {
                    fileMode = await LogService.GetLogWriteModeAsync();
                }

                await LogService.StartLog(logFile);
                IsLogging = true;
                await OnBacksAsync();

            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                await Alert.DisplayError(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnFileSelected()
        {
            if (SelectedLog == null)
            {
                return;
            }
            foreach (var location in LocationList)
            {
                location.IsSelected = location == SelectedLog;
            }
            await LogService.SetFileAsync(SelectedLog.Id);
        }

        private async Task OnAddFile()
        {
            try
            {




                string logFileName = string.Empty;
                var pResult = await Ioc.Default.GetService<IUserDialogs>().PromptAsync(new PromptConfig
                {
                    InputType =Acr.UserDialogs. InputType.Name,
                    OkText = "Ok",
                    Title = Localization.Localization.Log_EnterFileName,
                });

             

                if (pResult.Ok && !string.IsNullOrWhiteSpace(pResult.Text))
                { logFileName = pResult.Text; }

                if (string.IsNullOrEmpty(logFileName)) return;
                //Check if the File Name is valid or not... 
                bool isValidName = LoggerControl.IsSafeName(logFileName);

                if (!isValidName)
                {
                    Ioc.Default.GetService<IUserDialogs>().Alert(Localization.Localization.Log_EnterValidFileName, "Alert", "Ok");
                    return;
                }

                //To Calculate The Log intervals (InSeconds)...
                int totalSeconds = _minCount * 60 + _secCount;

                totalSeconds = totalSeconds < 5 ? 5 : totalSeconds;

                var isCreated = await LogService.AddLogFileAsync(logFileName, totalSeconds * 1000);
                if (isCreated)
                {
                    var logs = await LogService.GetLogFilesAsync();
                    var log = logs.FirstOrDefault(x => x.Name == logFileName);

                    await LogService.SetFileAsync(log.Id);
                    var locations = await LogService.GetLogFilesAsync();
                    LocationList = new ObservableCollection<LogFile>(locations);
                    SelectedLog = LocationList.FirstOrDefault(x => x.IsSelected);
                    LogFilesAvailable = true;
                    LogFilesNotAvailable = false;
                    InvokeLogFileAdded();
                }
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                await Alert.DisplayError(e);
            }
            finally
            {
                IsBusy = false;
            }

        }

        private async void OnSnapshot()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var logFile = SelectedLog;
                if (logFile == null)
                {
                    await Alert.ShowAlert(Localization.Localization.StartLog_SelectLocationMessage);
                    return;
                }

                await LogService.SetFileAsync(logFile);

                var isCreated = await LogService.CreateSnapshotAsync(logFile);

                var message = "";
                if (isCreated)
                {
                    message = string.Format(Localization.Localization.Log_SnapshotCreated_Format, logFile.Name);
                    AlertSound.PlaySystemSound(false);
                }
                else
                {
                    message = Localization.Localization.Log_SnapshotNotCreated;
                    AlertSound.PlaySystemSound(true);
                }
                var tst = new ToastConfig(Localization.Localization.RecordSound_AttachmentAdded)
                {
                    Duration = new TimeSpan(0, 0, 4),
                    Message = message,
                    MessageTextColor = System.Drawing.Color.White,
                    Position = ToastPosition.Bottom,
                };
                Ioc.Default.GetService<IUserDialogs>().Toast(tst);
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
        ///  To Minus number in count of min. ...
        /// </summary>
        private void OnMinMinus()
        {
            var valueMin = MinCount;
            if (valueMin > 0)
            {
                valueMin--;
                if (valueMin == 0 && SecCount < 5)
                {
                    SecCount = 5;
                }
                MinCount = valueMin;
            }
            else if (valueMin == 0)
            {
                MinCount = 30;
            }

        }

        /// <summary>
        /// To Plus number in count of min. ...
        /// </summary>
        private void OnMinPlus()
        {
            var valueMin = MinCount;
            if (valueMin < 30)
            {
                valueMin++;
                MinCount = valueMin;
            }
            else if (valueMin == 30)
            {
                valueMin = 0;
                if (SecCount < 5)
                {
                    SecCount = 5;
                }
                MinCount = valueMin;
            }
        }

        /// <summary>
        /// To Minus number in count of Sec. ...
        /// </summary>
        private void OnSecMinus()
        {
            var valueSec = SecCount;
            if (MinCount == 0 && valueSec <= 5)
            {
                SecCount = 60;
            }
            else if (MinCount > 0 && valueSec <= 0)
            {
                SecCount = 59;
            }
            else
            {
                valueSec--;
                SecCount = valueSec;
            }
        }

        /// <summary>
        ///To Plus number in count of Sec. ...
        /// </summary>
        private void OnSecPlus()
        {
            var valueSec = SecCount;
            if (valueSec < 60)
            {
                valueSec++;
                SecCount = valueSec;
            }

            if (valueSec != 60) return;
            SecCount = MinCount == 0 ? 5 : 0;
        }



        #endregion
    }
}
