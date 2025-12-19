using GrayWolf.ViewModels;
using System.IO;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;
using GrayWolf.Models.DBO;
using GrayWolf.CustomControls;
namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEventPopupPage
    {
        public AddEventPopupPage(DateTime dateTime)
        {
            BindingContext = new AddEventPopupViewModel(dateTime);
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Content.HeightRequest = height * 0.7;
        }
    }
}