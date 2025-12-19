using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;



namespace GrayWolf.ViewModels
{
    public class ProbeOptionsPopupViewModel : BasePopupViewModel
    {
        #region variables
        private PopupOption AllowSnapshotButtonOption { get; }

        private List<PopupOption> _options;
        public List<PopupOption> Options
        {
            get => _options;
            private set => SetProperty(ref _options, value);
        }

        public MockBleService FakeBleInstance { get; }
        #endregion

        #region commands
        public ICommand ConfirmCommand { get; }
        public ICommand UpdateSnapshotButtonEnabledCommand { get; }
        #endregion

        public ProbeOptionsPopupViewModel()
        {
            AllowSnapshotButtonOption = new PopupOption
            {
                IsChecked = Settings.EnableSnapshotFromDSIILogButton,
                OptionName = Localization.Localization.ProbeOptions_EnableSnapshotButtonFromDSIIDevice,
                Command = new Command(UpdateSnapshotButtonEnabled)
            };
            Options = new List<PopupOption>
            {
                AllowSnapshotButtonOption
            };
            ConfirmCommand = new Command(Confirm);
            UpdateSnapshotButtonEnabledCommand = new Command(UpdateSnapshotButtonEnabled);
            FakeBleInstance = SimpleIoc.Default.GetInstance<IBleService>(Constants.BLE_FACTORY_MOCK) as MockBleService;
        }

        private void UpdateSnapshotButtonEnabled()
        {
            AllowSnapshotButtonOption.IsChecked = !AllowSnapshotButtonOption.IsChecked;
        }

        public async Task<bool> GetStoragePermissionsAsync()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    var statuses = await Ioc.Default.GetService<IStoragePermissionService>().RequestStoragePermissions();
                    return statuses;
                case Device.iOS:
                    var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    if (status != PermissionStatus.Granted)
                    {
                        return false;
                    }
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    return status == PermissionStatus.Granted;
                default:
                    return false;
            }
        }

        private async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }
            
            Settings.EnableSnapshotFromDSIILogButton = AllowSnapshotButtonOption.IsChecked;
            if(Settings.EnableSnapshotFromDSIILogButton)
            {
                byte b = 240;
                
                RealBleService device = new();
                
               var dev = device.GetBleDevice(null);
               dev.SendLogButtonMessageIfClicked(b);
            }
            await OnBacksAsync();

            IsBusy = false;
        }
    }
}
