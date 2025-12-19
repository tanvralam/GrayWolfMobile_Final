using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectSensorTipPopupViewModel : BasePopupViewModel
    {
        #region variables
        private List<SensorTip> _sensorTips;
        public List<SensorTip> SensorTips
        {
            get => _sensorTips;
            private set => SetProperty(ref _sensorTips, value);
        }

        private SensorTip _selectedSensorTip;
        public SensorTip SelectedSensorTip
        {
            get => _selectedSensorTip;
            private set => SetProperty(ref _selectedSensorTip, value);
        }

        private TaskCompletionSource<SensorTip> TCS { get; }
        #endregion

        #region commands
        public ICommand SelectTipCommand { get; }
        public ICommand ConfirmCommand { get; }
        #endregion

        public SelectSensorTipPopupViewModel(List<SensorTip> sensorTips, TaskCompletionSource<SensorTip> tcs)
        {
            SelectTipCommand = new Command<SensorTip>(SelectSensorTip);
            ConfirmCommand = new Command(Confirm);
            SensorTips = sensorTips;
            TCS = tcs;
        }

        public override Task OnBacksAsync()
        {
            TCS?.TrySetCanceled();
            return base.OnBacksAsync();
        }

        private void SelectSensorTip(SensorTip sensorTip)
        {
            if (!SetBusy())
            {
                return;
            }

            SelectedSensorTip = sensorTip;

            IsBusy = false;
        }

        private async void Confirm()
        {
            if (SelectedSensorTip == null || !SetBusy())
            {
                return;
            }

            TCS.TrySetResult(SelectedSensorTip);
            await OnBacksAsync();

            IsBusy = false;
        }
    }
}
