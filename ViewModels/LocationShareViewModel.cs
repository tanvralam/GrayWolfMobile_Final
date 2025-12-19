using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using GrayWolf.Common;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using GrayWolf.Views;
using RGPopup.Maui.Extensions;
//using Syncfusion.Maui.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class LocationShareViewModel : BaseLogsPopupViewModel
    {
        #region Properties
        public bool LogFilesAvailable => LocationList?.Any() ?? false;

        public bool ExportButtonsOnly { get; }
        #endregion Properties

        #region Commands
        public IAsyncRelayCommand CancelCommand { get; }
        public ICommand FileSelectedCommand { get; }
        public IAsyncRelayCommand ShareFileCommand { get; }
        public IAsyncRelayCommand DeleteFileCommand { get; }
        public IAsyncRelayCommand SaveFileCommand { get; }
        public ICommand AddFileCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand SendFileByEmailCommand { get; }
        public ICommand OpenDataGridCommand { get; }
        #endregion Commands

        public LocationShareViewModel(bool exportButtonsOnly)
        {
            ExportButtonsOnly = exportButtonsOnly;
            FileSelectedCommand = new Command<LogFile>(OnFileSelected);
            CancelCommand = new AsyncRelayCommand(OnCancel);
            DeleteFileCommand = new AsyncRelayCommand<LogFile>(OnDeleteFile);
            ShareFileCommand = new AsyncRelayCommand<LogFile>(OnShareFileAsync);
            SaveFileCommand = new AsyncRelayCommand<LogFile>(OnSaveFile);
            AddFileCommand = new AsyncRelayCommand(OnAddFile);
            SendFileByEmailCommand = new AsyncRelayCommand<LogFile>(OnSendFileByEmail);
            ConfirmCommand = new Command(Confirm);
            OpenDataGridCommand = new Command<LogFile>(OpenDataGrid);
        }

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
                OnFileSelected(SelectedLog);
            }
            if (propertyName == nameof(LocationList))
            {
                RaisePropertyChanged(nameof(LogFilesAvailable));
            }
        }

        private async Task OnAddFile()
        {
            try
            {
                string logFileName = string.Empty;
                var pResult = await Ioc.Default.GetService<IUserDialogs>().PromptAsync(new PromptConfig
                {
                    InputType = Acr.UserDialogs.InputType.Name,
                    OkText = "Ok",
                    Title = Localization.Localization.Log_EnterFileName
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

                var isCreated = await LogService.AddLogFileAsync(logFileName, Constants.INTERVAL_LOG_MS_DEFAULT);
                if (isCreated)
                {
                    var locations = await LogService.GetLogFilesAsync();
                    LocationList = new ObservableCollection<LogFile>(locations);
                    SelectedLog = LocationList.FirstOrDefault(x => x.Name == logFileName);
                    InvokeLogFileAdded();
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Ioc.Default.GetService<IUserDialogs>().AlertAsync(new AlertConfig
                {
                    Message = "Failed to add Location Log File",
                });
            }
        }

        private async Task OnShareFileAsync(LogFile item)
        {
            if(item == null)
            {
                return;
            }
            if (LogService.IsLogging)
            {
                await Alert.ShowAlert(Localization.Localization.Logger_NotWhileLogging);
                return;
            }
            try
            {
                var result = await Ioc.Default.GetService<IUserDialogs>().ConfirmAsync(Localization.Localization.Confirm_Export, null, 
                    Localization.Localization.Button_Yes, Localization.Localization.Button_No);

                if (!result) return;
                await LogService.ExportLogAsync(item.Id);
            }
            catch (TaskCanceledException)
            {
                await Alert.ShowAlert(Localization.Localization.OperationCancelled);
            }
            catch (NotSupportedException)
            {
                await Alert.ShowAlert( Localization.Localization.Platform_NotSupported);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }

        private async Task OnSaveFile(LogFile item)
        {
            try
            {
                var result = await Ioc.Default.GetService<IUserDialogs>().ConfirmAsync(Localization.Localization.Confirm_Export, null, 
                    Localization.Localization.Button_Yes, Localization.Localization.Button_No);
                if (!result) return;

                await LogService.ExportLogAsync(item.Id, ExportLogOptions.SaveToFile);

                if (Device.RuntimePlatform != Device.Android)
                {
                    await Alert.ShowMessage(Localization.Localization.Export_Success);
                }
            }
            catch (TaskCanceledException)
            {
                await Alert.ShowAlert(Localization.Localization.OperationCancelled);
            }
            catch (NotSupportedException)
            {
                await Alert.ShowAlert(Localization.Localization.Platform_NotSupported);
            }
            catch (Exception ex)
            {
                await Alert.DisplayError(ex);
                AnalyticsService.TrackError(ex);
            }
        }

        private async Task OnSendFileByEmail(LogFile item)
        {

            var result = await Ioc.Default.GetService<IUserDialogs>().ConfirmAsync(Localization.Localization.Confirm_Export_Zip, null, 
                Localization.Localization.Button_Yes, Localization.Localization.Button_No);
            if (!result) return;

            try
            {
                await LogService.ExportLogAsync(item.Id, ExportLogOptions.SendByEmail);
            }
            catch (TaskCanceledException)
            {
                await Alert.ShowAlert(Localization.Localization.OperationCancelled);
            }
            catch (NotSupportedException)
            {
                await Alert.ShowAlert(Localization.Localization.Platform_NotSupported);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }


        private async Task OnDeleteFile(LogFile item)
        {
            try
            {
                if (LogService.IsLogging)
                {
                    await Alert.ShowAlert(Localization.Localization.Logger_NotWhileLogging);
                    return;
                }
                string[] buttons = { Localization.Localization.Button_Delete, Localization.Localization.Button_Clear };

                var result = await Ioc.Default.GetService<IUserDialogs>().ActionSheetAsync(Localization.Localization.SelectOption,
                    Localization.Localization.Button_Cancel, null,
                    null, buttons);
                int val = Array.IndexOf(buttons, result);// buttons.IndexOf(result);
                var isConfirmed = false;
                switch (val)
                {
                    case -1:
                    return;

                    case 0:
                        {
                            isConfirmed = await Alert.ShowMessageConfirmation(
                                Localization.Localization.Locations_DeleteConfirmationText,
                                Localization.Localization.Locations_AreYouSure,
                                Localization.Localization.Button_Yes,
                                Localization.Localization.Button_No);
                            if (isConfirmed)
                            {
                                await LogService.DeleteLocationAsync(item.Id);
                                Load();
                            }
                        }
                        break;

                    case 1:
                        isConfirmed = await Alert.ShowMessageConfirmation(
                            Localization.Localization.Locations_ClearConfirmationText,
                            Localization.Localization.Locations_AreYouSure,
                            Localization.Localization.Button_Yes,
                            Localization.Localization.Button_No);
                        if (isConfirmed)
                        {
                            await LogService.ClearLocationAsync(item.Id);
                            var updatedItem = await LogService.GetLogFileAsync(item.Id);
                            var index = LocationList.IndexOf(item);
                            var isSelected = SelectedLog?.Id == updatedItem.Id;
                            if (index != -1)
                            {
                                LocationList[index] = updatedItem;
                            }
                            if(isSelected)
                            {
                                SelectedLog = updatedItem;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }

        private async Task OnCancel()
        {
            await CloseDialog();
        }

        private void OnFileSelected(LogFile item)
        {
            SelectedLog = item;
            foreach (var itemLog in LocationList)
            {
                itemLog.IsSelected = itemLog.Id == item?.Id;
            }
        }

        private async Task CloseDialog()
        {
            await NavigationService.Instance.Nav.PopPopupAsync();
        }

        private async void Confirm()
        {
            if (SelectedLog == null || !SetBusy())
            {
                return;
            }

            await LogService.SetFileAsync(SelectedLog.Id);
            await OnBacksAsync();

            IsBusy = false;
        }

        public async void Load()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                var logs = await LogService.GetLogFilesAsync();
                foreach (var logFileModel in logs)
                {
                    if (!Directory.Exists(logFileModel.FolderPath))
                    {
                        continue;
                    }
                    var files = Directory.EnumerateFiles(logFileModel.FolderPath);
                    logFileModel.HasContent = files.Any();
                    logFileModel.IsSelected = logFileModel.Id == LogService.LogFileId;
                }

                LocationList = new ObservableCollection<LogFile>(logs);
                SelectedLog = LocationList.FirstOrDefault(x => x.IsSelected);
                InvokeSelectedLogInitialized();
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

        private async void OpenDataGrid(LogFile logFile)
        {
            if (!logFile.IsGraphAvailable || !SetBusy())
            {
                return;
            }

            await Navigation.PushAsync(new DataGridPage(logFile));
            await Navigation.PopPopupAsync();

            IsBusy = false;
        }
    }
}
