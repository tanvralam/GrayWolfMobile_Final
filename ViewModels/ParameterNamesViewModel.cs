using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class ParameterNamesViewModel : BasePopupViewModel
    {
        #region variables
        private PopupOption ShortNamesOption { get; }

        private PopupOption LongNamesOption { get; }

        private ObservableCollection<PopupOption> _options;
        public ObservableCollection<PopupOption> Options
        {
            get => _options;
            private set => SetProperty(ref _options, value);
        }
        #endregion

        #region commands
        public ICommand ConfirmCommand { get; }
        #endregion

        public ParameterNamesViewModel()
        {
            ShortNamesOption = new PopupOption
            {
                IsChecked = Settings.ParameterNameDisplayMode == ParameterNameDisplayOption.Short,
                OptionName = Localization.Localization.ParameterNames_UseShort,
                Command = new Command(SetShortNames)
            };

            LongNamesOption = new PopupOption
            {
                IsChecked = Settings.ParameterNameDisplayMode == ParameterNameDisplayOption.Long,
                OptionName = Localization.Localization.ParameterNames_UseLong,
                Command = new Command(SetLongNames)
            };
            Options = new ObservableCollection<PopupOption>
            {
                ShortNamesOption,
                LongNamesOption
            };

            ConfirmCommand = new Command(Confirm);
        }

        private void SetShortNames()
        {
            ShortNamesOption.IsChecked = true;
            LongNamesOption.IsChecked = false;
        }

        private void SetLongNames()
        {
            LongNamesOption.IsChecked = true;
            ShortNamesOption.IsChecked = false;
        }

        private async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }

            Settings.ParameterNameDisplayMode = ShortNamesOption.IsChecked ? ParameterNameDisplayOption.Short : ParameterNameDisplayOption.Long;
            DeviceService.NotifyDevicesUpdated();
            await OnBacksAsync();

            IsBusy = false;
        }
    }
}
