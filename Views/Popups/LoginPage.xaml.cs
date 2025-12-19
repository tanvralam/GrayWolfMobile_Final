using GrayWolf.ViewModels;
using GrayWolf.Views.Popups;



namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        private readonly LoginViewModel _vm;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = _vm = new LoginViewModel();
        }
    }
}