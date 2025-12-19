using GrayWolf.CustomControls;
using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleryAttachmentsSettingsPopupPage
    {
        public GalleryAttachmentsSettingsPopupPage()
        {
            InitializeComponent();
            BindingContext = new GalleryAttachmentsSettingsPopupViewModel();
        }
    }
}