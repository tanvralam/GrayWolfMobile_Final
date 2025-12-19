using GrayWolf.ViewModels;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BatteryStatusPopupPage
    {
        private readonly HomePageViewModel _homeVm;

        public BatteryStatusPopupPage(HomePageViewModel homeVm)
        {
            InitializeComponent();
            _homeVm = homeVm;
            this.BindingContext = _homeVm;
        }

       
    }
}