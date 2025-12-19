using GrayWolf.Models.Domain;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class GalleryAttachmentsSettingsPopupViewModel : BasePopupViewModel
    {
        #region variables
        public PopupOption EnableOption { get; }

        public PopupOption DisableOption { get; }

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

        public GalleryAttachmentsSettingsPopupViewModel()
        {
            EnableOption = new PopupOption
            {
                OptionName = Localization.Localization.GalleryAttachmentsSettings_Enabled,
                IsChecked = Settings.AreGalleryAttachmentsEnabled,
                Command = new Command(SetEnable)
            };
            DisableOption = new PopupOption
            {
                OptionName = Localization.Localization.GalleryAttachmentsSettings_Disabled,
                IsChecked = !Settings.AreGalleryAttachmentsEnabled,
                Command = new Command(SetDisable)
            };

            Options = new ObservableCollection<PopupOption>
            {
                EnableOption,
                DisableOption
            };

            ConfirmCommand = new Command(Confirm);
        }

        private void SetEnable()
        {
            EnableOption.IsChecked = true;
            DisableOption.IsChecked = false;
        }

        private void SetDisable()
        {
            DisableOption.IsChecked = true;
            EnableOption.IsChecked = false;
        }

        private async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }

            Settings.AreGalleryAttachmentsEnabled = EnableOption.IsChecked;
            await OnBacksAsync();

            IsBusy = false;
        }
    }
}
