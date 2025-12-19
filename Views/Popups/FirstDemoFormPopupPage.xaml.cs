using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirstDemoFormPopupPage
    {
        public FirstDemoFormPopupPage()
        {
            InitializeComponent();
            BindingContext = new FirstDemoFormPopupViewModel();
        }
    }
}