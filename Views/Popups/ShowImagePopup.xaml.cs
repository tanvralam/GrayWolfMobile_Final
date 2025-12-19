using GrayWolf.Models.DBO;
using GrayWolf.ViewModels;



namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowImagePopup
    {
        public ShowImagePopup(AttachmentDBO attachment)
        {
            InitializeComponent();
            BindingContext = new ShowImagePopupViewModel(attachment);
        }
    }
}