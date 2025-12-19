using GrayWolf.ViewModels;
//using MvvmHelpers;
using RGPopup.Maui.Pages;

namespace GrayWolf.CustomControls
{
    public class BasePopupPage : PopupPage
    {
        protected override bool OnBackButtonPressed()
        {
            if (BindingContext is BasePopupViewModel vm)
            {
                vm.BacksCommand.Execute(null);
                return true;
            }
            return base.OnBackButtonPressed();
        }

        protected override bool OnBackgroundClicked()
        {
            if (BindingContext is BasePopupViewModel vm)
            {
                vm.BacksCommand.Execute(null);
                return true;
            }
            return base.OnBackgroundClicked();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is BaseViewModel vm)
            {
                vm.OnAppearing();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is BaseViewModel vm)
            {
                vm.OnDisappearing();
            }
        }
    }
}
