using MvvmHelpers;
using System.ComponentModel;
using System.Windows.Input;

namespace GrayWolf.Models.Domain
{
    public class PopupOption : ObservableObject, IPopupOption
    {
        private string _optionName;
        public string OptionName
        {
            get => _optionName;
            set => SetProperty(ref _optionName, value);
        }

        private ICommand _command;
        public ICommand Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }

    public interface IPopupOption : INotifyPropertyChanged
    {
        string OptionName { get; set; }

        ICommand Command { get; set; }

        bool IsChecked { get; set; }
    }
}
