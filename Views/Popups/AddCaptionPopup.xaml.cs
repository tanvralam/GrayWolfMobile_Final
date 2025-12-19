using GrayWolf.Models.DBO;
using GrayWolf.ViewModels;
using System.IO;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCaptionPopup
    {
        public AddCaptionPopup(AttachmentDBO attachment)
        {
            InitializeComponent();
           BindingContext = new AddCaptionPopupViewModel(attachment);
        }

        protected override async void OnAppearing()
        {

        }
    }
}