using GrayWolf.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectChartParameterPopupViewModel : BasePopupViewModel
    {
        #region variables
        private List<GraphParameter> _parameters;
        public List<GraphParameter> Parameters
        {
            get => _parameters;
            private set => SetProperty(ref _parameters, value);
        }

        private GraphParameter _parameter;
        public GraphParameter Parameter
        {
            get => _parameter;
            private set => SetProperty(ref _parameter, value);
        }

        private TaskCompletionSource<GraphParameter> TCS { get; }
        #endregion

        #region commands
        public ICommand SelectCommand { get; }

        public ICommand ConfirmCommand { get; }
        #endregion

        public SelectChartParameterPopupViewModel(
            List<GraphParameter> parameters,
            GraphParameter parameter,
            TaskCompletionSource<GraphParameter> tcs)
        {
            Parameters = parameters;
            Parameter = parameter;

            TCS = tcs;

            SelectCommand = new Command<GraphParameter>(SelectParameter);
            ConfirmCommand = new Command(Confirm);
        }

        public override Task OnBacksAsync()
        {
            TCS?.TrySetCanceled();
            return base.OnBacksAsync();
        }

        public void SelectParameter(GraphParameter parameter)
        {
            Parameter = parameter;
        }

        public async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }

            TCS.TrySetResult(Parameter);
            await OnBacksAsync();
            IsBusy = false;
        }
    }
}
