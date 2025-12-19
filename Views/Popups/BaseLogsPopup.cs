using GrayWolf.CustomControls;
using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    public class BaseLogsPopup : BasePopupPage
    {
        private CollectionView _logsCollectionView;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!(BindingContext is BaseLogsPopupViewModel vm))
            {
                return;
            }
            if (vm.LocationList == null)
            {
                return;
            }
            TryToScrollToLog(vm.LocationList.IndexOf(vm.SelectedLog));
        }

        protected void VM_SelectedLogInitialized(object sender, EventArg<int> e)
        {
            TryToScrollToLog(e.Data);
        }

        protected void VM_LogFileAdded(object sender, EventArg<int> e)
        {
            TryToScrollToLog(e.Data);
        }

        protected void ContentView_ChildAdded(object sender, ElementEventArgs e)
        {
            if (!(e.Element is CollectionView newCollectionView))
            {                
                return;
            }

            _logsCollectionView = newCollectionView;
            if (!(BindingContext is BaseLogsPopupViewModel vm))
            {
                return;
            }
            TryToScrollToLog(vm.LocationList.IndexOf(vm.SelectedLog));
        }

        protected void TryToScrollToLog(int index)
        {
            if (_logsCollectionView == null)
            {
                return;
            }
            if (index == -1)
            {
                return;
            }

            if (!(BindingContext is BaseLogsPopupViewModel vm) || !vm.IsActive)
            {
                return;
            }
            _logsCollectionView.ScrollTo(index);
        }
    }
}
