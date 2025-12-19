using GrayWolf.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.CustomControls
{
    public class BaseContentPage : ContentPage
    {
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
