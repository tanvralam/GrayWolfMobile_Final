using Acr.UserDialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Enums;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using GrayWolf.Views;
using GrayWolf.Views.Popups;
using RGPopup.Maui.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class BaseMenuViewModel : BaseViewModel
    {
        #region variables
     
        public LogStatus LogStatus => LogService.Status;

        public bool CanSelectDevice => !LogService.IsLogging;// && !DeviceService.IsDemoMode;

        public bool CanStartLog => LogStatus == Enums.LogStatus.DevicesSelected;

        public bool CanStopLog => LogStatus == LogStatus.Logging;

        public bool CanSelectLogger => LogStatus != LogStatus.Logging;

        public bool IsLogActive => LogService.IsLogging;

        public bool IsLocationSelected => LogService.LogFileId > 0;

        public bool AreGalleryAttachmentsEnabled => IsLocationSelected;

        public bool CanOpenGraph => LogService?.CurrentFile?.IsGraphAvailable == true;

        IAlertSoundService AlertSound => Ioc.Default.GetService<IAlertSoundService>();
        #endregion

        #region commands
        public ICommand SnapshotCommand { get; }

        public ICommand LocationCommand { get; }

        public ICommand EventsCommand { get; }
        public ICommand GraphCommand { get; }
        public ICommand SelectFileForDataGridCommand { get; }
        #endregion

        public BaseMenuViewModel() : base()
        {
            LocationCommand = new Command(() => OnLocation(false));
            SnapshotCommand = new Command(OnSnapshot);
            EventsCommand = new Command(OnEvents);
            GraphCommand = new Command(OnGraphClicked);
            SelectFileForDataGridCommand = new Command(SelectFileForDataGrid);
        }

        #region override
        public override void OnAppearing()
        {
            base.OnAppearing();
            RaiseLogStatusChanged();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            //MessengerInstance.Unregister(this);
        }
        #endregion

        #region methods
        protected void OnLogStatusChanged(LogStatusChangedMessage obj)
        {
            RaiseLogStatusChanged();
        }

        protected virtual void RaiseLogStatusChanged()
        {
            RaisePropertyChanged(nameof(LogStatus));
            RaisePropertyChanged(nameof(CanStartLog));
            RaisePropertyChanged(nameof(CanStopLog));
            RaisePropertyChanged(nameof(CanSelectDevice));
            RaisePropertyChanged(nameof(CanSelectLogger));
            RaisePropertyChanged(nameof(IsLogActive));
            RaisePropertyChanged(nameof(CanOpenGraph));
        }

        private async void OnSnapshot()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var logFile = LogService.CurrentFile;
                if (logFile == null)
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
        /// To Perform Export Log Operations...
        /// </summary>
        protected async void OnLocation(bool exportButtonsOnly)
        {
            if (!SetBusy())
            {
                return;
            }
            
            try
            {
                if(LogService.Status == LogStatus.Logging)
                {
                    await Alert.ShowAlert(Localization.Localization.Log_SelectionWhileLoggingMessage);
                }
                else
                {
                    await NavigationService.Instance.Nav.PushPopupAsync(new LocationShare(exportButtonsOnly));
                }
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
                App.FlyoutPage.IsPresented = false;
            }
        }

        private async void OnEvents()
        {
            if (!SetBusy())
            {
                return;
            }
            if (!IsLogActive)
            {
                await Alert.ShowAlert(Localization.Localization.Log_Events_NotRunningMessage);
            }
            else
            {
                await Navigation.PushPopupAsync(new AddEventPopupPage(DateTime.UtcNow));
            }

            IsBusy = false;
        }

        private async void OnGraphClicked()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                LogFile file = null;

                if (LogService.IsLogging)
                {
                    file = LogService.CurrentFile;
                }
                else
                {
                    var tcs = new TaskCompletionSource<LogFile>();
                    var files = (await LogService.GetLogFilesAsync()).Where(x => x.IsGraphAvailable).ToList();
                    await Navigation.PushPopupAsync(new SelectFilePopupPage(files, tcs));
                    try
                    {
                        file = await tcs.Task;
                    }
                    catch (TaskCanceledException) 
                    {
                        IsBusy = false;
                        return;
                    }
                    catch (Exception ex)
                    {
                        await Alert.DisplayError(ex);
                        IsBusy = false;
                        return;
                    }
                }

                App.FlyoutPage.IsPresented = false;
                await NavigationService.Instance.NavigateTo(new ChartPage(file));
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void SelectFileForDataGrid()
        {
            if (!SetBusy())
            {
                return;
            }

            LogFile file = null; ;
            if (LogService.IsLogging)
            {
                file = LogService.CurrentFile;
            }
            else
            {
                var tcs = new TaskCompletionSource<LogFile>();
                var files = (await LogService.GetLogFilesAsync()).Where(x => x.IsGraphAvailable).ToList();
                await Navigation.PushPopupAsync(new SelectFilePopupPage(files, tcs));
                try
                {
                    file = await tcs.Task;
                }
                catch (TaskCanceledException) { }
                catch(Exception ex)
                {
                    await Alert.DisplayError(ex);
                }
            }
            if(file != null)
            {
                await Navigation.PushAsync(new DataGridPage(file));
                App.FlyoutPage.IsPresented = false;
            }

            IsBusy = false;
        }
        #endregion
    }
}
