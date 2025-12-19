using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProbeOptionsPopupPage
    {
        public ProbeOptionsPopupPage()
        {
            InitializeComponent();
            BindingContext = new ProbeOptionsPopupViewModel();
        }
    }
}