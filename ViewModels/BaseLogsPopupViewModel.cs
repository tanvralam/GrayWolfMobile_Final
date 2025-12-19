using GrayWolf.Models.Domain;
using System;
using System.Collections.ObjectModel;

namespace GrayWolf.ViewModels
{
    public class BaseLogsPopupViewModel : BasePopupViewModel
    {
        #region variables
        private ObservableCollection<LogFile> _locationList;

        public ObservableCollection<LogFile> LocationList
        {
            get => _locationList;
            set => SetProperty(ref _locationList, value);
        }

        private LogFile _selectedLog;
        public LogFile SelectedLog
        {
            get => _selectedLog;
            set => SetProperty(ref _selectedLog, value);
        }
        #endregion

        public event EventHandler<EventArg<int>> SelectedLogInitialized;

        public event EventHandler<EventArg<int>> LogFileAdded;

        protected void InvokeLogFileAdded()
        {
            LogFileAdded?.Invoke(this, new EventArg<int>(LocationList.IndexOf(SelectedLog)));
        }

        protected void InvokeSelectedLogInitialized()
        {
            SelectedLogInitialized?.Invoke(this, new EventArg<int>(LocationList.IndexOf(SelectedLog)));
        }
    }
}
