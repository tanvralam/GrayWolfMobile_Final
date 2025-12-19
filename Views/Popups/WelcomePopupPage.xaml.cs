using GrayWolf.CustomControls;
using GrayWolf.ViewModels;

using Microsoft.Maui.Controls.Xaml;
using RGPopup.Maui.Pages;
namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePopupPage
    {
        public WelcomePopupPage()
        {
           InitializeComponent();
           BindingContext = new WelcomePopupViewModel();
        }
    }
}