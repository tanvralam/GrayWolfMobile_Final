using GrayWolf.Models;
using GrayWolf.ViewModels;
using GrayWolf.Views.Popups;
//using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrayWolf.Services;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using GrayWolf.Interfaces;

namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        //private readonly HomePageViewModel _homeVm;
        public static HomePage homePage;
        public static HomePageViewModel _homeVm;
        private readonly IAuthService AuthService;
        public HomePage()
        {
            InitializeComponent();
            // iOS Platform      
            //this.On<iOS>().SetUseSafeArea(true);
            this.BindingContext = _homeVm = new HomePageViewModel();
            homePage = this;
        }

        #region Event Handlers

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _homeVm.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _homeVm.OnDisappearing();
        }

        #endregion
    }
}