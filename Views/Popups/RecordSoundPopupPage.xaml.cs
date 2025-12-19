using GrayWolf.ViewModels;
using RGPopup.Maui.Pages;
using System;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordSoundPopupPage
    {

        public RecordSoundPopupPage()
        {
            InitializeComponent();
            this.BindingContext = new RecordSoundPopupViewModel();
        }
    }
} 