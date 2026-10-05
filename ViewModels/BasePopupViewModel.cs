using GrayWolf.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Exceptions;
using System;

namespace GrayWolf.ViewModels
{
    public class BasePopupViewModel : BaseViewModel
    {
        private bool _isClosing;

        public BasePopupViewModel() : base()
        {
        }
       
        public bool IsLogActive => LogService.IsLogging;
        public override async Task OnBacksAsync()
        {
            if (_isClosing)
            {
                return;
            }

            _isClosing = true;
            try
            {
                await NavigationService.Instance.Nav.PopPopupAsync();
            }
            catch (RGPopupStackInvalidException)
            {
                // Android can deliver a second Back/background event after the
                // popup has already been removed. There is nothing left to close.
            }
            finally
            {
                _isClosing = false;
            }
        }
    }
}
