using GrayWolf.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectChartTimeOptionPopupViewModel : BasePopupViewModel
    {
        #region variables
        public List<GraphTimeOption> Options { get; } = new List<GraphTimeOption>
        {
            GraphTimeOption.FifteenMinutes,
            GraphTimeOption.OneHour,
            GraphTimeOption.EightHours,
            GraphTimeOption.Day,
            GraphTimeOption.Everything
        };

        private GraphTimeOption _option;
        public GraphTimeOption Option
        {
            get => _option;
            private set => SetProperty(ref _option, value);
        }

        private TaskCompletionSource<GraphTimeOption> TCS { get; }
        #endregion

        #region commands
        public ICommand SelectCommand { get; }

        public ICommand ConfirmCommand { get; }
        #endregion

        public SelectChartTimeOptionPopupViewModel(
            GraphTimeOption selectedOption,
            TaskCompletionSource<GraphTimeOption> tcs)
        {
            TCS = tcs;
            Option = selectedOption;
            SelectCommand = new Command<GraphTimeOption>(OnOptionSelected);
            ConfirmCommand = new Command(Confirm);
        }

        public override Task OnBacksAsync()
        {
            TCS?.TrySetCanceled();
            return base.OnBacksAsync();
        }

        public void OnOptionSelected(GraphTimeOption option)
        {
            Option = option;
        }

        public async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }

            TCS?.TrySetResult(Option);
            await OnBacksAsync();

            IsBusy = false;
        }
    }
}
