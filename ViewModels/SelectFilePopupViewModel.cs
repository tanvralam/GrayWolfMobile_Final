using GrayWolf.Models.Domain;
using GrayWolf.Views;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectFilePopupViewModel : BasePopupViewModel
    {
        #region variables
        private List<LogFile> _logFiles;
        public List<LogFile> LogFiles
        {
            get => _logFiles;
            private set => SetProperty(ref _logFiles, value);
        }

        private LogFile _logFile;
        public LogFile LogFile
        {
            get => _logFile;
            private set => SetProperty(ref _logFile, value);
        }

        public bool IsListVisible => LogFiles?.Any() == true;

        public bool IsPlaceholderVisible => !IsListVisible;

        private TaskCompletionSource<LogFile> TCS { get; }
        #endregion

        #region commands
        public ICommand SelectLogCommand { get; }

        public ICommand ConfirmCommand { get; }
        #endregion

        public SelectFilePopupViewModel(List<LogFile> files, TaskCompletionSource<LogFile> tcs)
        {
            TCS = tcs;
            SelectLogCommand = new Command<LogFile>(SelectLog);
            ConfirmCommand = new Command(Confirm);
            LogFiles = files;
            Init();
        }

        #region override
        public override Task OnBacksAsync()
        {
            TCS.TrySetCanceled();
            return base.OnBacksAsync();
        }
        #endregion

        #region methods
        private async void Init()
        {
            try
            {
                RaisePropertyChanged(nameof(IsListVisible));
                RaisePropertyChanged(nameof(IsPlaceholderVisible));
                LogFile = LogFiles.FirstOrDefault(x => x.Id == LogService.CurrentFile?.Id);
            }
            catch (Exception ex)
            {
                await Alert.DisplayError(ex);
                await OnBacksAsync();
            }
        }

        private void SelectLog(LogFile file)
        {
            LogFile = file;
        }

        private async void Confirm()
        {
            if (LogFile == null || !SetBusy())
            {
                return;
            }

            TCS.TrySetResult(LogFile);
            await Navigation.PopPopupAsync();

            IsBusy = false;
        }
        #endregion
    }
}
