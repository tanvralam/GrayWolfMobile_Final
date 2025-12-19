using GrayWolf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmCustomPasswordPopupPage
    {
        public ConfirmCustomPasswordPopupPage(string password, IOnCustomPasswordConfirmedListener listener)
        {
            InitializeComponent();
            BindingContext = new ConfirmCustomPasswordPopupPageViewModel(password, listener);
        }
    }
}