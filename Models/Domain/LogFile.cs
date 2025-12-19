using GrayWolf.Enums;
using MvvmHelpers;

namespace GrayWolf.Models.Domain
{
    public class LogFile : ObservableObject
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _trendLoggingActive;
        public bool TrendLoggingActive
        {
            get => _trendLoggingActive;
            set => SetProperty(ref _trendLoggingActive, value);
        }

        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        private string _LjhFilePath;
        public string LjhFilePath
        {
            get => _LjhFilePath;
            set => SetProperty(ref _LjhFilePath, value);
        }

        private string _LcvFilePath;
        public string LcvFilePath
        {
            get => _LcvFilePath;
            set => SetProperty(ref _LcvFilePath, value);
        }

        private int _loggingInterval;
        public int LoggingInterval
        {
            get => _loggingInterval;
            set => SetProperty(ref _loggingInterval, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _hasContent;
        public bool HasContent
        {
            get => _hasContent;
            set => SetProperty(ref _hasContent, value);
        }

        private bool _isGraphAvailable;
        public bool IsGraphAvailable
        {
            get => _isGraphAvailable;
            set => SetProperty(ref _isGraphAvailable, value);
        }

        private ParameterNameDisplayOption _parameterNameDisplayMode;
        public ParameterNameDisplayOption ParameterNameDisplayMode
        {
            get => _parameterNameDisplayMode;
            set => SetProperty(ref _parameterNameDisplayMode, value);
        }
    }
}
