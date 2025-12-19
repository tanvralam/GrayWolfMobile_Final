using GrayWolf.CustomControls;
using GrayWolf.ViewModels;
using System.Threading.Tasks;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttachmentsSizeWarningPopupPage
    {
        public AttachmentsSizeWarningPopupPage(TaskCompletionSource<bool> tcs)
        {
            InitializeComponent();
            BindingContext = new AttachmentsSizeWarningPopupViewModel(tcs);
        }
    }
}