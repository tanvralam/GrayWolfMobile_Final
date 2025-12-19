using GrayWolf.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectVOCItemPopupViewModel : BasePopupViewModel
    {
        #region variables
        private List<VOCItem> _items;
        public List<VOCItem> Items
        {
            get => _items;
            private set => SetProperty(ref _items, value);
        }

        public List<VOCItem> FilteredItems => string.IsNullOrWhiteSpace(Filter) ? Items : Items.Where(x => x.Name.ToLowerInvariant().StartsWith(Filter.ToLowerInvariant())).ToList();

        private VOCItem _selectedItem;
        public VOCItem SelectedItem
        {
            get => _selectedItem;
            private set => SetProperty(ref _selectedItem, value);
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        private TaskCompletionSource<VOCItem> TCS { get; }
        #endregion

        #region commands
        public ICommand SelectItemCommand { get; }
        public ICommand ConfirmCommand { get; }
        #endregion

        public SelectVOCItemPopupViewModel(List<VOCItem> items, TaskCompletionSource<VOCItem> tcs)
        {
            SelectItemCommand = new Command<VOCItem>(SelectItem);
            ConfirmCommand = new Command(Confirm);
            Items = items;
            TCS = tcs;
        }

        public override Task OnBacksAsync()
        {
            TCS.TrySetCanceled();
            return base.OnBacksAsync();
        }

        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Items):
                case nameof(Filter):
                    RaisePropertyChanged(nameof(FilteredItems));
                    break;
            }
        }

        private void SelectItem(VOCItem item)
        {
            if (!SetBusy())
            {
                return;
            }

            SelectedItem = item;

            IsBusy = false;
        }

        private async void Confirm()
        {
            if (SelectedItem == null || !SetBusy())
            {
                return;
            }

            TCS.TrySetResult(SelectedItem);
            await OnBacksAsync();

            IsBusy = false;
        }
    }
}
