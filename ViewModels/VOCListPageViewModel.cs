using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Views.Popups;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class VOCListPageViewModel : BaseViewModel
    {
        #region variables
        private List<VOCItem> _items;
        public List<VOCItem> Items
        {
            get => _items;
            private set => SetProperty(ref _items, value);
        }

        private VOCItem _item;
        public VOCItem Item
        {
            get => _item;
            private set => SetProperty(ref _item, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }
        #endregion

        #region commands
        public ICommand SelectVOCItemCommand { get; }
        #endregion

        public VOCListPageViewModel(List<VOCItem> items, VOCItem item)
        {
            SelectVOCItemCommand = new Command(SelectVOCItem);
            Items = items;
            Item = item;
        }
        
        private async void SelectVOCItem()
        {
            if(!SetBusy())
            {
                return;
            }

            try
            {
                var tcs = new TaskCompletionSource<VOCItem>();
                await Navigation.PushPopupAsync(new SelectVOCItemPopupPage(Items, tcs));
                Item = await tcs.Task;
            }
            catch (TaskCanceledException) { }
            catch(Exception ex)
            {
                await Alert.DisplayError(ex);
            }

            IsBusy = false;
        }
    }
}
