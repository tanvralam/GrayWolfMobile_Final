using GrayWolf.ViewModels;
using RGPopup.Maui.Pages;


namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectDevicesPopup : PopupPage 
    {
        private readonly SelectDevicesPageViewModel _vm;

        public SelectDevicesPopup()
        {
            InitializeComponent();
            BindingContext = _vm = new SelectDevicesPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _vm.OnDisappearing();
        }
    }
}