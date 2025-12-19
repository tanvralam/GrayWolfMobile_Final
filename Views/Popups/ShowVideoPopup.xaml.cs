using GrayWolf.Models.DBO;
using GrayWolf.ViewModels;
using CommunityToolkit.Maui.Media;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowVideoPopup
    {
        public ShowVideoPopup(AttachmentDBO attachment)
        {
            InitializeComponent();
            BindingContext = new ShowVideoPopupViewModel(attachment);
        }
    }
}