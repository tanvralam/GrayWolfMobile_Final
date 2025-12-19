using GrayWolf.ViewModels;

namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private readonly MenuPageViewModel _menuVm;
        public static MenuPage menuPage;
        public MenuPage()
        {
            InitializeComponent();

           
            NavigationPage.SetHasNavigationBar(this, false);
            _menuVm = new MenuPageViewModel();
            this.BindingContext = _menuVm;
            menuPage = this;
        }

        public static void ChangeContext()
        {
            menuPage.BindingContext = null;
            menuPage.BindingContext = menuPage._menuVm;
        }
    }
}