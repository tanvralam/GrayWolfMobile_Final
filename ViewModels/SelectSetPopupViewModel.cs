using GrayWolf.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectSetPopupViewModel : BasePopupViewModel
    {
        public List<DataGridSet> Sets { get; }

        private DataGridSet _selectedSet;
        public DataGridSet SelectedSet
        {
            get => _selectedSet;
            private set => SetProperty(ref _selectedSet, value);
        }

        private TaskCompletionSource<DataGridSet> TCS { get; }

        public ICommand SelectSetCommand { get; }

        public ICommand ConfirmCommand { get; }

        public SelectSetPopupViewModel(
            List<DataGridSet> sets,
            TaskCompletionSource<DataGridSet> tcs,
            DataGridSet currentSet)
        {
            Sets = sets;
            TCS = tcs;
            SelectSet(currentSet);

            SelectSetCommand = new Command<DataGridSet>(SelectSet);
            ConfirmCommand = new Command(Confirm);
        }

        public override Task OnBacksAsync()
        {
            TCS?.TrySetCanceled();
            return base.OnBacksAsync();
        }

        private void SelectSet(DataGridSet set)
        {
            if (!SetBusy())
            {
                return;
            }

            SelectedSet = set;

            IsBusy = false;
        }

        private async void Confirm()
        {
            if (SelectedSet == null || !SetBusy())
            {
                return;
            }

            TCS.TrySetResult(SelectedSet);
            await OnBacksAsync();

            IsBusy = false;
        }
    }
}
