using GrayWolf.Models.DBO;
using GrayWolf.ViewModels;



namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogNotePopupPage
    {
        public LogNotePopupPage(AttachmentDBO attachment = null)
        {
            InitializeComponent();
            BindingContext = new LogNotePopupViewModel(attachment);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Content.HeightRequest = height * 0.7;
        }
    }
}